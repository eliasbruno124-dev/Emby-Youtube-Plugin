using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Emby.YouTubePlugin;

// Private server state, never part of PluginConfiguration or an HTTP response.
internal sealed class YouTubePersonalProfiles
{
    internal static readonly object SyncRoot = new();
    internal static YouTubePersonalProfiles? Current { get; private set; }
    internal static event Action? Changed;
    internal const int MaxChannels = 100;
    internal const int MaxProfiles = 1024;
    internal const int MaxStorageBytes = 4 * 1024 * 1024;
    private static readonly byte[] Header = Encoding.ASCII.GetBytes("YTPROF01");
    private static readonly Regex ChannelId = new(@"\AUC[A-Za-z0-9_-]{22}\z", RegexOptions.CultureInvariant);
    private static readonly Regex Handle = new(@"\A@[A-Za-z0-9_][A-Za-z0-9_.-]{1,28}[A-Za-z0-9_]\z", RegexOptions.CultureInvariant);
    private readonly string _directory;
    private readonly string _keyPath;
    private readonly string _dataPath;
    private Dictionary<Guid, Snapshot> _profiles = new();
    private byte[] _key = Array.Empty<byte>();
    private bool _writeFailed;

    // Do not use a record: its generated ToString would expose the API key.
    internal sealed class Snapshot
    {
        public Guid UserId { get; set; }
        public string ScopeId { get; set; } = string.Empty;
        public long Revision { get; set; }
        public string[] Channels { get; set; } = Array.Empty<string>();
        public string? ApiKey { get; set; }
        internal Snapshot Copy() => new()
        {
            UserId = UserId, ScopeId = ScopeId, Revision = Revision,
            Channels = Channels.ToArray(), ApiKey = ApiKey
        };
    }

    private sealed class Document
    {
        public int Version { get; set; }
        public Snapshot[] Profiles { get; set; } = Array.Empty<Snapshot>();
    }

    internal static void Initialize(string dataPath)
    {
        lock (SyncRoot)
        {
            var directory = Path.Combine(Path.GetFullPath(dataPath), "youtube-personal");
            if (Current != null)
            {
                if (!string.Equals(Current._directory, directory, StringComparison.Ordinal))
                    throw new InvalidOperationException("Personal profile storage is already initialized.");
                return;
            }
            Current = new YouTubePersonalProfiles(directory);
        }
    }

    // Internal constructor permits isolated tests; production uses Initialize once.
    internal YouTubePersonalProfiles(string directory)
    {
        _directory = Path.GetFullPath(directory);
        _keyPath = Path.Combine(_directory, "encryption.key");
        _dataPath = Path.Combine(_directory, "profiles.dat");
        lock (SyncRoot)
        {
            PrivateFiles.EnsureDirectory(_directory);
            var havePendingWrite = ValidateLayout();
            var haveData = PrivateFiles.Exists(_dataPath);
            if (PrivateFiles.Exists(_keyPath))
            {
                _key = PrivateFiles.Read(_keyPath, 32);
                if (_key.Length != 32) throw StorageError();
            }
            else
            {
                // Never silently generate a replacement key for existing records.
                if (haveData || havePendingWrite) throw StorageError();
                var key = RandomNumberGenerator.GetBytes(32);
                try { PrivateFiles.WriteNew(_keyPath, key); _key = key; }
                catch { CryptographicOperations.ZeroMemory(key); throw; }
            }
            if (haveData) _profiles = Load();
        }
    }

    internal Snapshot Get(Guid userId)
    {
        RequireUser(userId);
        lock (SyncRoot)
        {
            ValidateFiles();
            return _profiles.TryGetValue(userId, out var profile)
                ? profile.Copy() : new Snapshot { UserId = userId };
        }
    }

    internal Snapshot[] List()
    {
        lock (SyncRoot)
        {
            ValidateFiles();
            return _profiles.Values.Select(value => value.Copy()).ToArray();
        }
    }

    internal Snapshot Update(Guid userId, long expectedRevision, string[]? channels,
        string? apiKey, bool clearApiKey, bool enablePersonalApiKeys, bool enablePersonalSubscriptions)
    {
        RequireUser(userId);
        if (expectedRevision < 0) throw new ArgumentException("Invalid profile revision.");
        if (channels != null && !enablePersonalSubscriptions || (apiKey != null || clearApiKey) && !enablePersonalApiKeys)
            throw new UnauthorizedAccessException("Personal capability is disabled.");
        if (channels != null)
        {
            channels = channels.ToArray();
            ValidateChannels(channels);
        }
        if (apiKey != null && !ValidKey(apiKey) || clearApiKey && apiKey != null)
            throw new ArgumentException("Invalid API key update.");
        Snapshot result;
        var changed = false;
        lock (SyncRoot)
        {
            ValidateFiles();
            _profiles.TryGetValue(userId, out var old);
            if ((old?.Revision ?? 0) != expectedRevision)
                throw new InvalidOperationException("Personal profile revision changed.");
            if (channels == null && apiKey == null && !clearApiKey)
                return old?.Copy() ?? new Snapshot { UserId = userId };
            if (old == null && _profiles.Count >= MaxProfiles) throw StorageError();
            var updated = old?.Copy() ?? new Snapshot
            {
                UserId = userId, ScopeId = Guid.NewGuid().ToString("N")
            };
            updated.Revision = checked(updated.Revision + 1);
            if (channels != null) updated.Channels = channels.ToArray();
            if (apiKey != null || clearApiKey) updated.ApiKey = clearApiKey ? null : apiKey;
            var next = new Dictionary<Guid, Snapshot>(_profiles) { [userId] = updated };
            try { Persist(next); }
            catch
            {
                // A failure after rename (for example directory fsync failure)
                // has uncertain durability. Do not serve a stale in-memory view
                // or overwrite that potentially committed revision on a retry.
                _writeFailed = true;
                throw;
            }
            _profiles = next; // Publish only after durable atomic replacement succeeded.
            result = updated.Copy();
            changed = true;
        }
        if (changed)
        {
            // An observer cannot turn a committed write into a reported failure.
            foreach (var handler in Changed?.GetInvocationList() ?? Array.Empty<Delegate>())
                try { ((Action)handler)(); } catch { /* Refresh is best-effort; never log private state. */ }
        }
        return result;
    }

    internal static void ValidateChannels(string[] channels)
    {
        if (channels.Length > MaxChannels) throw new ArgumentException("Invalid channel list.");
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var handles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var channel in channels)
        {
            if (channel == null || channel.Length > 31
                || !(ChannelId.IsMatch(channel) ? ids.Add(channel) : Handle.IsMatch(channel) && handles.Add(channel)))
                throw new ArgumentException("Invalid or duplicate channel identifier.");
        }
    }

    private static bool ValidKey(string key) => key.Length is >= 1 and <= 256
        && key.All(value => value is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '-');
    private static void RequireUser(Guid userId)
    {
        if (userId == Guid.Empty) throw new ArgumentException("An authenticated user is required.");
    }
    private static InvalidDataException StorageError() => new("Personal profile storage is unavailable.");

    private bool ValidateLayout()
    {
        var count = 0;
        var pendingWrite = false;
        foreach (var path in Directory.EnumerateFileSystemEntries(_directory))
        {
            if (++count > 18) throw StorageError();
            var name = Path.GetFileName(path);
            if (name is "encryption.key" or "profiles.dat") continue;
            // Never reinterpret a foreign/legacy store as an empty installation.
            // A crash may leave one encrypted temporary file; preserve it for
            // recovery, and never generate a new key while one exists.
            if (!name.StartsWith(".write-", StringComparison.Ordinal)
                || !Guid.TryParseExact(name.Substring(7), "N", out _)) throw StorageError();
            PrivateFiles.ValidateFile(path);
            pendingWrite = true;
        }
        return pendingWrite;
    }

    private void ValidateFiles()
    {
        if (_writeFailed) throw StorageError();
        PrivateFiles.ValidateDirectory(_directory);
        var diskKey = PrivateFiles.Read(_keyPath, 32);
        try
        {
            if (!CryptographicOperations.FixedTimeEquals(_key, diskKey)) throw StorageError();
        }
        finally { CryptographicOperations.ZeroMemory(diskKey); }
        if (PrivateFiles.Exists(_dataPath)) PrivateFiles.ValidateFile(_dataPath);
        else if (_profiles.Count != 0) throw StorageError();
    }

    private Dictionary<Guid, Snapshot> Load()
    {
        var encrypted = PrivateFiles.Read(_dataPath, MaxStorageBytes);
        if (encrypted.Length < Header.Length + 12 + 16 || !encrypted.AsSpan(0, Header.Length).SequenceEqual(Header))
            throw StorageError();
        var plain = new byte[encrypted.Length - Header.Length - 12 - 16];
        try
        {
            using var aes = new AesGcm(_key);
            aes.Decrypt(encrypted.AsSpan(Header.Length, 12), encrypted.AsSpan(Header.Length + 28),
                encrypted.AsSpan(Header.Length + 12, 16), plain, Header);
            var document = JsonSerializer.Deserialize<Document>(plain);
            if (document == null || document.Version != 1 || document.Profiles == null || document.Profiles.Length > MaxProfiles)
                throw StorageError();
            var result = new Dictionary<Guid, Snapshot>();
            var scopes = new HashSet<string>(StringComparer.Ordinal);
            foreach (var profile in document.Profiles)
            {
                if (profile == null || profile.UserId == Guid.Empty || profile.Revision <= 0 || profile.Channels == null
                    || !Guid.TryParseExact(profile.ScopeId, "N", out var scope) || scope == Guid.Empty
                    || profile.ScopeId != scope.ToString("N") || !scopes.Add(profile.ScopeId)
                    || profile.ApiKey != null && !ValidKey(profile.ApiKey) || !result.TryAdd(profile.UserId, profile))
                    throw StorageError();
                ValidateChannels(profile.Channels);
            }
            return result;
        }
        catch (Exception error) when (error is JsonException or CryptographicException or ArgumentException)
        { throw StorageError(); }
        finally { CryptographicOperations.ZeroMemory(plain); }
    }

    private void Persist(Dictionary<Guid, Snapshot> profiles)
    {
        var plain = JsonSerializer.SerializeToUtf8Bytes(new Document { Version = 1, Profiles = profiles.Values.ToArray() });
        try
        {
            if (plain.Length > MaxStorageBytes - Header.Length - 28) throw StorageError();
            var encrypted = new byte[plain.Length + Header.Length + 28];
            Header.CopyTo(encrypted, 0);
            RandomNumberGenerator.Fill(encrypted.AsSpan(Header.Length, 12));
            using var aes = new AesGcm(_key);
            aes.Encrypt(encrypted.AsSpan(Header.Length, 12), plain, encrypted.AsSpan(Header.Length + 28),
                encrypted.AsSpan(Header.Length + 12, 16), Header);
            PrivateFiles.Replace(_dataPath, encrypted);
        }
        finally { CryptographicOperations.ZeroMemory(plain); }
    }

    // .NET 6 has no File.SetUnixFileMode. Native creation applies 0600/0700
    // before the first secret byte is written, independent of the host umask.
    // The private directory is the trust boundary; its ancestors must not be links.
    private static class PrivateFiles
    {
        private static bool Windows => OperatingSystem.IsWindows();
        private static bool Mac => OperatingSystem.IsMacOS();
        internal static bool Exists(string path)
        {
            var info = new FileInfo(path);
            if (info.LinkTarget != null || (info.Exists && (info.Attributes & FileAttributes.ReparsePoint) != 0)) throw StorageError();
            if (Directory.Exists(path)) throw StorageError();
            return info.Exists;
        }

        private static void NoLinkedAncestors(string path)
        {
            for (var directory = new DirectoryInfo(path); directory != null; directory = directory.Parent)
                if (directory.LinkTarget != null || directory.Exists && (directory.Attributes & FileAttributes.ReparsePoint) != 0)
                    throw StorageError();
        }

        internal static void EnsureDirectory(string path)
        {
            NoLinkedAncestors(path);
            if (!Directory.Exists(Path.GetDirectoryName(path))) throw StorageError();
            if (!Directory.Exists(path))
            {
                if (Windows) CreateWindowsDirectory(path);
                else if (OperatingSystem.IsLinux() || Mac)
                {
                    if (Mkdir(path, 448) != 0) throw StorageError(); // 0700
                }
                else throw new PlatformNotSupportedException("Private profile storage requires Windows, Linux or macOS.");
            }
            ValidateDirectory(path);
        }

        internal static void ValidateDirectory(string path)
        {
            NoLinkedAncestors(path);
            if (!Directory.Exists(path)) throw StorageError();
            if (Windows)
            {
                var security = new DirectoryInfo(path).GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner);
                if (!security.AreAccessRulesProtected) throw StorageError();
                ValidateWindowsAccess(security);
            }
            else
            {
                using var handle = OpenUnix(path, directory: true, create: false);
                ValidateUnix(handle, true);
            }
        }

        internal static void ValidateFile(string path)
        {
            using var stream = Open(path, create: false);
        }

        internal static byte[] Read(string path, int maximum)
        {
            using var stream = Open(path, create: false);
            if (stream.Length > maximum || stream.Length < 0) throw StorageError();
            var bytes = new byte[(int)stream.Length];
            var offset = 0;
            while (offset < bytes.Length)
            {
                var count = stream.Read(bytes, offset, bytes.Length - offset);
                if (count == 0) throw StorageError();
                offset += count;
            }
            if (stream.ReadByte() != -1) throw StorageError();
            return bytes;
        }

        internal static void WriteNew(string path, byte[] bytes)
        {
            using var stream = Open(path, create: true);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush(flushToDisk: true);
        }

        internal static void Replace(string path, byte[] bytes)
        {
            var directory = Path.GetDirectoryName(path)!;
            ValidateDirectory(directory);
            if (Exists(path)) ValidateFile(path);
            var temporary = Path.Combine(directory, ".write-" + Guid.NewGuid().ToString("N"));
            try
            {
                WriteNew(temporary, bytes);
                // Same-directory rename is atomic; a failed write retains the old file.
                ValidateDirectory(directory);
                if (Exists(path)) ValidateFile(path);
                File.Move(temporary, path, overwrite: true);
                if (!Windows)
                {
                    using var handle = OpenUnix(directory, true, false);
                    if (Fsync(handle.DangerousGetHandle().ToInt32()) != 0) throw StorageError();
                }
            }
            finally
            {
                // Only the exact randomly named file created by this operation.
                if (Exists(temporary)) File.Delete(temporary);
            }
        }

        private static FileStream Open(string path, bool create)
        {
            NoLinkedAncestors(Path.GetDirectoryName(path)!);
            if (!create && !Exists(path)) throw StorageError();
            SafeFileHandle handle;
            if (Windows)
            {
                handle = CreateFile(path, create ? 0x40000000u : 0x80000000u, 1, IntPtr.Zero,
                    create ? 1u : 3u, 0x00200000u | 0x80u, IntPtr.Zero);
                if (handle.IsInvalid) { handle.Dispose(); throw StorageError(); }
                if (!GetFileInformationByHandle(handle, out var info) || info.NumberOfLinks != 1
                    || (info.FileAttributes & (0x400u | 0x10u)) != 0)
                { handle.Dispose(); throw StorageError(); }
                try { ValidateWindowsAccess(new FileInfo(path).GetAccessControl(AccessControlSections.Access | AccessControlSections.Owner)); }
                catch { handle.Dispose(); throw; }
            }
            else handle = OpenUnix(path, directory: false, create);
            try { return new FileStream(handle, create ? FileAccess.Write : FileAccess.Read); }
            catch { handle.Dispose(); throw; }
        }

        private static SafeFileHandle OpenUnix(string path, bool directory, bool create)
        {
            if (!OperatingSystem.IsLinux() && !Mac) throw StorageError();
            var flags = Mac ? 0x100 | 0x1000000 : 0x20000 | 0x80000; // NOFOLLOW | CLOEXEC
            if (directory) flags |= Mac ? 0x100000 : 0x10000;
            if (create) flags |= Mac ? 1 | 0x200 | 0x800 : 1 | 0x40 | 0x80; // WRONLY | CREAT | EXCL
            else if (!directory) flags |= Mac ? 4 : 0x800; // NONBLOCK: reject FIFO without hanging.
            var fd = UnixOpen(path, flags, 384); // 0600
            if (fd < 0) throw StorageError();
            var handle = new SafeFileHandle(new IntPtr(fd), ownsHandle: true);
            try { ValidateUnix(handle, directory); return handle; }
            catch { handle.Dispose(); throw; }
        }

        private static void ValidateUnix(SafeFileHandle handle, bool directory)
        {
            var stat = Marshal.AllocHGlobal(256);
            try
            {
                var fd = handle.DangerousGetHandle().ToInt32();
                // Intel Darwin retains the legacy unsuffixed fstat ABI. Apple
                // silicon uses the 64-bit inode layout without a suffix.
                var status = Mac && RuntimeInformation.ProcessArchitecture == Architecture.X64
                    ? FstatDarwin64(fd, stat) : Fstat(fd, stat);
                if (status != 0) throw StorageError();
                uint mode, owner; ulong links;
                if (Mac)
                {
                    mode = (ushort)Marshal.ReadInt16(stat, 4); links = (ushort)Marshal.ReadInt16(stat, 6);
                    owner = (uint)Marshal.ReadInt32(stat, 16);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
                {
                    mode = (uint)Marshal.ReadInt32(stat, 24); links = (ulong)Marshal.ReadInt64(stat, 16);
                    owner = (uint)Marshal.ReadInt32(stat, 28);
                }
                else if (RuntimeInformation.ProcessArchitecture == Architecture.Arm64)
                {
                    mode = (uint)Marshal.ReadInt32(stat, 16); links = (uint)Marshal.ReadInt32(stat, 20);
                    owner = (uint)Marshal.ReadInt32(stat, 24);
                }
                else throw new PlatformNotSupportedException("Private profile storage requires a supported 64-bit Unix ABI.");
                if ((mode & 0xf000) != (directory ? 0x4000 : 0x8000) || (mode & 0x1ff) != (directory ? 448u : 384u)
                    || owner != GetEuid() || !directory && links != 1) throw StorageError();
            }
            finally { Marshal.FreeHGlobal(stat); }
        }

        private static void CreateWindowsDirectory(string path)
        {
            var sid = WindowsIdentity.GetCurrent().User?.Value ?? throw StorageError();
            if (!ConvertSecurityDescriptor("O:" + sid + "D:P(A;OICI;FA;;;SY)(A;OICI;FA;;;" + sid + ")", 1, out var descriptor, out _))
                throw StorageError();
            try
            {
                var attributes = new SecurityAttributes { Length = Marshal.SizeOf<SecurityAttributes>(), Descriptor = descriptor };
                if (!CreateDirectory(path, ref attributes)) throw StorageError();
            }
            finally { LocalFree(descriptor); }
        }

        private static void ValidateWindowsAccess(FileSystemSecurity security)
        {
            var current = WindowsIdentity.GetCurrent().User;
            var owner = security.GetOwner(typeof(SecurityIdentifier));
            // Elevated Windows processes may use Administrators as default
            // file owner. It has the same trusted OS-administrator boundary as
            // Unix root; access grants remain restricted to this user/SYSTEM.
            if (owner == null || !owner.Equals(current) && owner.Value is not ("S-1-5-18" or "S-1-5-32-544")) throw StorageError();
            foreach (FileSystemAccessRule rule in security.GetAccessRules(true, true, typeof(SecurityIdentifier)))
                if (rule.AccessControlType == AccessControlType.Allow && !rule.IdentityReference.Equals(current)
                    && rule.IdentityReference.Value != "S-1-5-18") throw StorageError();
        }

        [StructLayout(LayoutKind.Sequential)] private struct SecurityAttributes { internal int Length; internal IntPtr Descriptor; internal int Inherit; }
        [StructLayout(LayoutKind.Sequential)] private struct FileInformation
        {
            internal uint FileAttributes; internal System.Runtime.InteropServices.ComTypes.FILETIME Creation, Access, Write;
            internal uint Volume, SizeHigh, SizeLow, NumberOfLinks, IndexHigh, IndexLow;
        }
        [DllImport("libc", EntryPoint = "mkdir", SetLastError = true)] private static extern int Mkdir(string path, uint mode);
        [DllImport("libc", EntryPoint = "open", SetLastError = true)] private static extern int UnixOpen(string path, int flags, uint mode);
        [DllImport("libc", EntryPoint = "fstat", SetLastError = true)] private static extern int Fstat(int fd, IntPtr stat);
        [DllImport("libc", EntryPoint = "fstat$INODE64", SetLastError = true)] private static extern int FstatDarwin64(int fd, IntPtr stat);
        [DllImport("libc", EntryPoint = "geteuid")] private static extern uint GetEuid();
        [DllImport("libc", EntryPoint = "fsync", SetLastError = true)] private static extern int Fsync(int fd);
        [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string path, uint access, uint share, IntPtr attributes, uint creation, uint flags, IntPtr template);
        [DllImport("kernel32.dll", SetLastError = true)] private static extern bool GetFileInformationByHandle(SafeFileHandle handle, out FileInformation information);
        [DllImport("kernel32.dll", EntryPoint = "CreateDirectoryW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CreateDirectory(string path, ref SecurityAttributes attributes);
        [DllImport("advapi32.dll", EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ConvertSecurityDescriptor(string descriptor, uint revision, out IntPtr result, out uint size);
        [DllImport("kernel32.dll")] private static extern IntPtr LocalFree(IntPtr memory);
    }
}
