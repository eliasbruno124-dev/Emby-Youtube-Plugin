using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MediaBrowser.Controller.Channels;
using MediaBrowser.Controller.Entities;

namespace Emby.YouTubePlugin
{
    // Emby 4.11.0.18 obfuscates and protects parts of the server: protected
    // methods call module-level helpers (string decryption) that check
    // Assembly.GetCallingAssembly(). A Harmony patch replaces the method with
    // a copy in a dynamic assembly, the check fails and the helper spins
    // forever - the whole server stops answering (measured with a dump: eight
    // threads in GetBaseItemDtos). Ordinary C# never calls module-level
    // functions, so a call to one marks a method that must not be patched,
    // whatever the Emby version. Every Harmony site checks its targets here
    // first and stays off as a whole when one of them is protected.
    internal static class EmbyPatchSafety
    {
        private static readonly ConcurrentDictionary<MethodBase, bool> Cache = new();
        private static readonly Dictionary<short, OperandType> Operands = typeof(OpCodes)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(field => field.GetValue(null)).OfType<OpCode>()
            .GroupBy(code => code.Value).ToDictionary(group => group.Key, group => group.First().OperandType);

        private static readonly ConcurrentDictionary<MethodBase, bool> Reported = new();

        internal static bool IsPatchable(MethodBase method) => Cache.GetOrAdd(method, Inspect);

        // Obfuscated names are invisible characters; the metadata token and
        // assembly identify the method in a decompiler instead.
        internal static string Describe(MethodBase method)
        {
            string type = method.DeclaringType?.FullName ?? "<module>";
            string name = method.Name;
            if (HasInvisibleName(type)) type = "<obfuscated>";
            if (HasInvisibleName(name)) name = "<obfuscated>";
            return type + "." + name + " (0x" + method.MetadataToken.ToString("X8") + " in "
                + method.Module.Assembly.GetName().Name + " " + method.Module.Assembly.GetName().Version + ")";
        }

        internal static void RequirePatchable(IEnumerable<MethodBase?> methods)
        {
            foreach (var method in methods)
                if (method != null && !IsPatchable(method))
                {
                    if (Reported.TryAdd(method, true))
                        YouTubeChannel.LogPublic("[YT] Protected Emby method skipped: " + Describe(method) + ".");
                    throw new InvalidOperationException("protected target");
                }
        }

        // A patched method runs as a copy in a dynamic assembly, and the JIT
        // may inline small callees into that copy. A callee that calls a
        // module-level helper would then see the dynamic assembly as its
        // caller as well. So the target and every callee the JIT could inline
        // (same module, small or AggressiveInlining, a few levels deep) must
        // be free of such calls. Obfuscated names alone are no longer a
        // reason to stay off: Emby 4.11.0.19 keeps them on unprotected
        // methods (IDtoService, IUserViewManager, IChannelManager).
        private const int InlineDepth = 3;
        private const int InlineCandidateBytes = 128;

        private static bool Inspect(MethodBase method) => Clean(method, 0, new HashSet<MethodBase>());

        private static bool Clean(MethodBase method, int depth, HashSet<MethodBase> seen)
        {
            if (!seen.Add(method)) return true;
            byte[]? il;
            try { il = method.GetMethodBody()?.GetILAsByteArray(); }
            catch { return false; }
            if (il == null) return true;
            if (depth > 0 && il.Length > InlineCandidateBytes
                && (method.MethodImplementationFlags & MethodImplAttributes.AggressiveInlining) == 0)
                return true; // too large for the JIT to inline into the patched copy
            Type[]? typeArguments = method.DeclaringType?.IsGenericType == true ? method.DeclaringType.GetGenericArguments() : null;
            Type[]? methodArguments = method.IsGenericMethod ? method.GetGenericArguments() : null;
            int i = 0;
            while (i < il.Length)
            {
                short value = il[i];
                int size = 1;
                if (value == 0xFE && i + 1 < il.Length) { value = (short)(0xFE00 | il[i + 1]); size = 2; }
                if (!Operands.TryGetValue(value, out var operand)) return false; // unknown opcode: do not guess
                i += size;
                switch (operand)
                {
                    case OperandType.InlineMethod:
                        if (i + 4 > il.Length) return false;
                        int token = BitConverter.ToInt32(il, i);
                        try
                        {
                            var target = method.Module.ResolveMethod(token, typeArguments, methodArguments);
                            if (target != null && target.DeclaringType == null) return false; // module-level helper
                            if (target != null && depth < InlineDepth && target.Module == method.Module
                                && !target.IsAbstract && !Clean(target, depth + 1, seen))
                                return false;
                        }
                        catch { return false; }
                        i += 4;
                        break;
                    case OperandType.InlineNone: break;
                    case OperandType.ShortInlineBrTarget:
                    case OperandType.ShortInlineI:
                    case OperandType.ShortInlineVar: i += 1; break;
                    case OperandType.InlineVar: i += 2; break;
                    case OperandType.InlineI8:
                    case OperandType.InlineR: i += 8; break;
                    case OperandType.InlineSwitch:
                        if (i + 4 > il.Length) return false;
                        int count = BitConverter.ToInt32(il, i);
                        if (count < 0 || count > (il.Length - i) / 4) return false;
                        i += 4 + 4 * count;
                        break;
                    default: i += 4; break;
                }
            }
            return true;
        }

        // Obfuscated names consist of zero-width and direction-control characters.
        private static bool HasInvisibleName(string? name) =>
            name != null && name.Any(c => c >= '​' && c <= '‏' || c >= '‪' && c <= '‮' || c >= '⁠' && c <= '⁯');
    }

    // ChannelManager.GetChannelProvider(Channel) is private. Emby 4.11.0.18+
    // obfuscates private names, so the method is the one of exactly that
    // shape (IChannel from a Channel item) on the server's IChannelManager
    // implementation. The readable name wins where it still exists; several
    // anonymous candidates mean "unknown", never a guess.
    internal static class EmbyChannelBinding
    {
        private static readonly object Sync = new();
        private static Type? _type;
        private static MethodInfo? _method;

        internal static MethodInfo? ProviderMethod(object? manager)
        {
            if (manager == null) return null;
            var type = manager.GetType();
            lock (Sync)
            {
                if (_type == type) return _method;
                MethodInfo? method = null;
                try
                {
                    const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    method = type.GetMethod("GetChannelProvider", flags, null, new[] { typeof(Channel) }, null);
                    if (method == null || method.ReturnType != typeof(IChannel))
                    {
                        var shaped = type.GetMethods(flags)
                            .Where(candidate => candidate.ReturnType == typeof(IChannel) && !candidate.IsAbstract
                                && candidate.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual(new[] { typeof(Channel) }))
                            .ToArray();
                        method = shaped.Length == 1 ? shaped[0] : null;
                    }
                }
                catch (Exception error) when (error is not OutOfMemoryException) { method = null; }
                _type = type;
                _method = method;
                if (method == null)
                    YouTubeChannel.LogPublic("[YT] Channel provider lookup unavailable on this Emby version.");
                return method;
            }
        }

        internal static bool IsYouTube(object manager, MethodInfo provider, Channel channel)
            => provider.Invoke(manager, new object[] { channel }) is YouTubeChannel;
    }
}
