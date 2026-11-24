using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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

        internal static bool IsPatchable(MethodBase method) => Cache.GetOrAdd(method, Inspect);

        internal static void RequirePatchable(IEnumerable<MethodBase?> methods)
        {
            foreach (var method in methods)
                if (method != null && !IsPatchable(method))
                    throw new InvalidOperationException("protected target");
        }

        private static bool Inspect(MethodBase method)
        {
            if (HasInvisibleName(method.Name) || HasInvisibleName(method.DeclaringType?.Name)) return false;
            byte[]? il;
            try { il = method.GetMethodBody()?.GetILAsByteArray(); }
            catch { return false; }
            if (il == null) return true;
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
}
