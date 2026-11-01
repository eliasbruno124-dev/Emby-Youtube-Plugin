using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin;

[Route("/YouTubePlugin/MySettings", "GET")]
[Authenticated]
public sealed class GetYouTubePersonalSettings : IReturn<YouTubePersonalSettingsView> { }

[Route("/YouTubePlugin/MySettings", "POST")]
[Authenticated]
public sealed class SaveYouTubePersonalSettings : IRequiresRequestStream, IReturn<YouTubePersonalSettingsView>
{
    public Stream RequestStream { get; set; } = null!;
}

public sealed class YouTubePersonalSettingsView
{
    public Guid UserId { get; set; }
    public long Revision { get; set; }
    public bool HasApiKey { get; set; }
    public string[] Channels { get; set; } = Array.Empty<string>();
    public bool EnablePersonalApiKeys { get; set; }
    public bool EnablePersonalSubscriptions { get; set; }
    public string PersonalChannelVisibility { get; set; } = "OwnUser";
}

public sealed class YouTubePersonalService : IService, IRequiresRequest
{
    private readonly IAuthorizationContext _authorization;
    private readonly IHttpResultFactory _results;
    public IRequest Request { get; set; } = null!;
    public YouTubePersonalService(IAuthorizationContext authorization, IHttpResultFactory results)
    { _authorization = authorization; _results = results; }

    private sealed class Failure : Exception
    {
        internal HttpStatusCode Status { get; }
        internal string Code { get; }
        internal Failure(HttpStatusCode status, string code, string message) : base(message)
        { Status = status; Code = code; }
    }
    private static Failure Invalid() => new(HttpStatusCode.BadRequest, "InvalidSettings", "Invalid personal settings update.");

    private Guid Owner()
    {
        var user = _authorization.GetAuthorizationInfo(Request)?.User;
        if (user == null || user.InternalId <= 0 || user.Id == Guid.Empty)
            throw new Failure(HttpStatusCode.Unauthorized, "AuthenticationRequired", "An enabled user session is required.");
        var policy = user.Policy;
        if (policy == null || policy.IsDisabled)
            throw new Failure(HttpStatusCode.Unauthorized, "AuthenticationRequired", "An enabled user session is required.");
        return user.Id;
    }

    private async Task<object> Respond(Func<Task<YouTubePersonalSettingsView>> action)
    {
        object payload;
        var status = HttpStatusCode.OK;
        try { payload = await action().ConfigureAwait(false); }
        catch (Failure failure)
        { status = failure.Status; payload = new { ErrorCode = failure.Code, Message = failure.Message }; }
        catch (ArgumentException)
        { status = HttpStatusCode.BadRequest; payload = new { ErrorCode = "InvalidSettings", Message = "Invalid personal settings update." }; }
        catch (InvalidOperationException)
        { status = HttpStatusCode.Conflict; payload = new { ErrorCode = "RevisionConflict", Message = "Settings changed. Reload and try again." }; }
        catch (Exception error) when (error is IOException or InvalidDataException or CryptographicException or UnauthorizedAccessException or PlatformNotSupportedException or OverflowException)
        { status = HttpStatusCode.ServiceUnavailable; payload = new { ErrorCode = "SettingsUnavailable", Message = "Personal settings are unavailable." }; }
        var result = _results.GetResult(Request, JsonSerializer.SerializeToUtf8Bytes(payload), "application/json",
            new Dictionary<string, string> { ["Cache-Control"] = "no-store, private", ["Pragma"] = "no-cache" });
        Request.Response.StatusCode = (int)status;
        return result;
    }

    public Task<object> Get(GetYouTubePersonalSettings request) => Respond(() =>
    {
        var owner = Owner();
        Request.CancellationToken.ThrowIfCancellationRequested();
        lock (YouTubePersonalProfiles.SyncRoot)
        {
            var config = Configuration();
            return Task.FromResult(View(Store().Get(owner), config));
        }
    });

    public Task<object> Post(SaveYouTubePersonalSettings request) => Respond(async () =>
    {
        var owner = Owner(); // Reject userless API tokens before reading a submitted body.
        var input = await ReadInput(request.RequestStream, Request.CancellationToken).ConfigureAwait(false);
        lock (YouTubePersonalProfiles.SyncRoot)
        {
            Request.CancellationToken.ThrowIfCancellationRequested();
            // Re-resolve after the asynchronous read so disabled/revoked identities
            // cannot borrow an earlier identity snapshot for a later mutation.
            if (Owner() != owner) throw new Failure(HttpStatusCode.Unauthorized, "AuthenticationRequired", "An enabled user session is required.");
            var config = Configuration();
            if (input.Channels != null && !config.EnablePersonalSubscriptions
                || (input.ApiKey != null || input.HasClearApiKey) && !config.EnablePersonalApiKeys)
                throw new Failure(HttpStatusCode.Forbidden, "CapabilityDisabled", "This personal capability is disabled.");
            return View(Store().Update(owner, input.ExpectedRevision, input.Channels, input.ApiKey,
                input.ClearApiKey, config.EnablePersonalApiKeys, config.EnablePersonalSubscriptions), config);
        }
    });

    private static YouTubePersonalProfiles Store() => YouTubePersonalProfiles.Current
        ?? throw new Failure(HttpStatusCode.ServiceUnavailable, "SettingsUnavailable", "Personal settings are unavailable.");
    private static PluginConfiguration Configuration() => Plugin.Instance?.Options
        ?? throw new Failure(HttpStatusCode.ServiceUnavailable, "SettingsUnavailable", "Personal settings are unavailable.");
    private static YouTubePersonalSettingsView View(YouTubePersonalProfiles.Snapshot profile, PluginConfiguration config) => new()
    {
        UserId = profile.UserId, Revision = profile.Revision, HasApiKey = profile.ApiKey != null,
        Channels = profile.Channels, EnablePersonalApiKeys = config.EnablePersonalApiKeys,
        EnablePersonalSubscriptions = config.EnablePersonalSubscriptions,
        PersonalChannelVisibility = config.PersonalChannelVisibility == "Shared" ? "Shared" : "OwnUser"
    };

    private sealed class Input
    {
        internal long ExpectedRevision;
        internal string[]? Channels;
        internal string? ApiKey;
        internal bool ClearApiKey;
        internal bool HasClearApiKey;
    }
    private static async Task<Input> ReadInput(Stream stream, CancellationToken cancellationToken)
    {
        if (stream == null) throw Invalid();
        using var buffer = new MemoryStream();
        var bytes = new byte[4096];
        try
        {
            while (true)
            {
                var count = await stream.ReadAsync(bytes.AsMemory(), cancellationToken).ConfigureAwait(false);
                if (count == 0) break;
                if (buffer.Length + count > 32768)
                    throw new Failure(HttpStatusCode.RequestEntityTooLarge, "SettingsTooLarge", "Personal settings body is too large.");
                buffer.Write(bytes, 0, count);
            }
            using var document = JsonDocument.Parse(buffer.GetBuffer().AsMemory(0, (int)buffer.Length), new JsonDocumentOptions { MaxDepth = 8 });
            if (document.RootElement.ValueKind != JsonValueKind.Object) throw Invalid();
            var input = new Input();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var field in document.RootElement.EnumerateObject())
            {
                if (!seen.Add(field.Name)) throw Invalid();
                switch (field.Name.ToUpperInvariant())
                {
                    case "EXPECTEDREVISION":
                        if (field.Value.ValueKind != JsonValueKind.Number || !field.Value.TryGetInt64(out input.ExpectedRevision) || input.ExpectedRevision < 0) throw Invalid();
                        break;
                    case "CHANNELS":
                        if (field.Value.ValueKind != JsonValueKind.Array || field.Value.GetArrayLength() > YouTubePersonalProfiles.MaxChannels) throw Invalid();
                        var channels = new List<string>();
                        foreach (var value in field.Value.EnumerateArray())
                        {
                            if (value.ValueKind != JsonValueKind.String) throw Invalid();
                            channels.Add(value.GetString()!);
                        }
                        input.Channels = channels.ToArray();
                        break;
                    case "APIKEY":
                        if (field.Value.ValueKind != JsonValueKind.String) throw Invalid();
                        input.ApiKey = field.Value.GetString();
                        break;
                    case "CLEARAPIKEY":
                        if (field.Value.ValueKind is not (JsonValueKind.True or JsonValueKind.False)) throw Invalid();
                        input.HasClearApiKey = true;
                        input.ClearApiKey = field.Value.GetBoolean();
                        break;
                    default: throw Invalid();
                }
            }
            if (!seen.Contains("ExpectedRevision") || input.ApiKey != null && input.ClearApiKey) throw Invalid();
            return input;
        }
        catch (JsonException) { throw Invalid(); }
        finally
        {
            CryptographicOperations.ZeroMemory(bytes);
            CryptographicOperations.ZeroMemory(buffer.GetBuffer().AsSpan());
        }
    }
}
