using System.Security.Cryptography;
using System.Text;

namespace Emby.YouTubePlugin;

/// <summary>Explicit profile work, never a process-wide current browser user.</summary>
internal sealed class YouTubePersonalRequestContext : IDisposable
{
    private static readonly AsyncLocal<YouTubePersonalRequestContext?> Slot = new();
    private readonly YouTubePersonalRequestContext? _previous;
    private readonly Func<bool> _isCurrent;
    internal static YouTubePersonalRequestContext? Current => Slot.Value;
    internal string CacheNamespace { get; }
    internal bool UsesPersonalKey { get; }
    private int _disposed;

    internal YouTubePersonalRequestContext(Guid owner, long revision, string credential,
        bool usesPersonalKey, Func<bool> isCurrent)
    {
        if (owner == Guid.Empty || string.IsNullOrWhiteSpace(credential))
            throw new ArgumentException("A valid personal metadata context is required.");
        _isCurrent = isCurrent;
        UsesPersonalKey = usesPersonalKey;
        CacheNamespace = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            owner.ToString("N") + "|" + revision + "|" + credential)));
        _previous = Slot.Value;
        Slot.Value = this;
    }

    internal void EnsureCurrent()
    {
        // A coalesced request can outlive its first waiter while another waiter
        // with the same owner/revision/key still needs it. Scope disposal only
        // restores AsyncLocal; profile validity and shared-flight cancellation
        // decide whether the work may continue.
        if (!_isCurrent())
            throw new OperationCanceledException("Personal YouTube settings changed; refresh the page.");
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 0) Slot.Value = _previous;
    }
}
