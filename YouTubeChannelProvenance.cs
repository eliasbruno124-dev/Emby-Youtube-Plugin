using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Emby.YouTubePlugin;

// Persist provenance in provider IDs, not in a process-local ownership guess.
// Unmarked historical roots need positive proof from current admin settings.
internal static class YouTubeChannelProvenance
{
    internal const string Prefix = "personalshared_x_";
    private sealed record Proof(string ConfigurationHash, long Revision, string[] Shared, string[] SharedTokens);
    private static Proof? _proof;
    internal static void Reset()
    {
        Volatile.Write(ref _proof, null);
        YouTubeAdminProvenance.Reset();
    }
    internal static PluginConfiguration Copy(PluginConfiguration config) =>
        JsonSerializer.Deserialize<PluginConfiguration>(JsonSerializer.Serialize(config))!;
    internal static string Token(string term) => Convert.ToHexString(SHA256.HashData(
        Encoding.UTF8.GetBytes(term.StartsWith("@", StringComparison.Ordinal) ? term.ToLowerInvariant() : term))).Substring(0, 32).ToLowerInvariant();
    internal static string Wrap(string token, string inner) => Prefix + token + "_" + inner;
    internal static bool TryParse(string? id, out string token, out string inner)
    {
        token = inner = "";
        if (id == null || !id.StartsWith(Prefix, StringComparison.Ordinal) || id.Length <= Prefix.Length + 33
            || id[Prefix.Length + 32] != '_') return false;
        token = id.Substring(Prefix.Length, 32);
        if (token.Any(c => !(c >= '0' && c <= '9' || c >= 'a' && c <= 'f'))) return false;
        inner = id.Substring(Prefix.Length + 33);
        return inner.Length > 0 && !inner.StartsWith(Prefix, StringComparison.Ordinal)
            && !inner.StartsWith(YouTubePersonalCatalog.Prefix, StringComparison.Ordinal);
    }
    internal static bool SharedAllowed(string token, PluginConfiguration config) =>
        YouTubePersonalCatalog.SharedChannels(config).Any(term => Token(term) == token);
    internal static bool LegacyAllowed(string id, PluginConfiguration config)
    {
        if (!id.StartsWith("channel_x_", StringComparison.Ordinal)) return false;
        var term = id.Substring("channel_x_".Length);
        // A configured canonical UC ID is self-proving. Handles use the dedicated
        // verified map bound to current admin SavedItems, including after restart.
        if (YouTubeChannel.IsCanonicalChannelId(term) && YouTubeChannel.ConfiguredTerms(config.SavedItems).Contains(term, StringComparer.Ordinal)) return true;
        if (YouTubeAdminProvenance.Allows(id, config)) return true;
        var proof = Volatile.Read(ref _proof);
        if (proof == null || proof.ConfigurationHash != PluginEntryPoint.ComputeConfigHash(config)) return false;
        // Admin aliases are authorized only by their exact verified handle map,
        // never by a previous root-build union of canonical IDs.
        return proof.Revision == Interlocked.Read(ref YouTubePersonalCatalog.Revision)
                && config.EnablePersonalSubscriptions && config.PersonalChannelVisibility == "Shared"
                && proof.SharedTokens.Length > 0
                && proof.SharedTokens.SequenceEqual(YouTubePersonalCatalog.SharedChannels(config).Select(Token).Distinct().OrderBy(value => value, StringComparer.Ordinal))
                && proof.Shared.Contains(id, StringComparer.Ordinal);
    }
    internal static void Publish(string hash, long revision, IEnumerable<string> admin, IEnumerable<string> shared, IEnumerable<string> sharedTerms)
    {
        if (YouTubePersonalRequestContext.Current != null) return;
        EnsureCurrent(hash, revision);
        // The legacy admin argument is not an identity proof; exact handle
        // resolutions are recorded separately by the global admin builder.
        Volatile.Write(ref _proof, new Proof(hash, revision, shared.Distinct(StringComparer.Ordinal).ToArray(),
            sharedTerms.Select(Token).Distinct().OrderBy(value => value, StringComparer.Ordinal).ToArray()));
        EnsureCurrent(hash, revision);
    }
    internal static void EnsureCurrent(string hash, long revision)
    {
        var current = Plugin.Instance?.Options;
        if (current == null || PluginEntryPoint.ComputeConfigHash(current) != hash
            || Interlocked.Read(ref YouTubePersonalCatalog.Revision) != revision)
            throw new OperationCanceledException("YouTube catalog policy changed during refresh.");
    }
}
