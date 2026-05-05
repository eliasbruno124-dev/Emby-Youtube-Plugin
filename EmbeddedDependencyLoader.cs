using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;

namespace Emby.YouTubePlugin
{
    // Emby deploys plugins most comfortably as one DLL. Harmony is embedded
    // into this assembly and resolved on demand so there is no extra file to
    // copy next to the plugin.
    internal static class EmbeddedDependencyLoader
    {
        private const string HarmonyAssemblyName = "0Harmony";
        private const string HarmonyResourceName = "Emby.YouTubePlugin.Embedded.0Harmony.dll";

        private static int _registered;
        private static Assembly? _harmonyAssembly;

        internal static void Register()
        {
            if (Interlocked.Exchange(ref _registered, 1) == 1)
                return;

            // Depending on where the load starts, .NET can ask either the
            // classic AppDomain resolver or the AssemblyLoadContext resolver.
            AppDomain.CurrentDomain.AssemblyResolve += ResolveFromAppDomain;
            AssemblyLoadContext.Default.Resolving += ResolveFromLoadContext;
        }

        internal static Assembly? LoadHarmonyAssembly()
        {
            Register();

            // The interceptor talks to Harmony by reflection, so force the
            // embedded dependency to load before we ask for its types.
            return Resolve(HarmonyAssemblyName);
        }

        private static Assembly? ResolveFromAppDomain(object? sender, ResolveEventArgs args)
        {
            return Resolve(args.Name);
        }

        private static Assembly? ResolveFromLoadContext(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            return Resolve(assemblyName.FullName);
        }

        private static Assembly? Resolve(string? assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName))
                return null;

            var requested = new AssemblyName(assemblyName);
            if (!string.Equals(requested.Name, HarmonyAssemblyName, StringComparison.OrdinalIgnoreCase))
                return null;

            // Reuse an existing Harmony assembly if Emby or another plugin has
            // already loaded one into the current process.
            var alreadyLoaded = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => string.Equals(a.GetName().Name, HarmonyAssemblyName, StringComparison.OrdinalIgnoreCase));
            if (alreadyLoaded != null)
                return alreadyLoaded;

            if (_harmonyAssembly != null)
                return _harmonyAssembly;

            var executingAssembly = typeof(EmbeddedDependencyLoader).Assembly;
            using var resource = executingAssembly.GetManifestResourceStream(HarmonyResourceName);
            if (resource == null)
            {
                System.Diagnostics.Debug.WriteLine("[YT] Embedded Harmony dependency not found.");
                return null;
            }

            using var buffer = new MemoryStream();
            resource.CopyTo(buffer);
            buffer.Position = 0;

            // Load Harmony from the resource baked into this DLL at build time.
            _harmonyAssembly = AssemblyLoadContext.Default.LoadFromStream(buffer);
            System.Diagnostics.Debug.WriteLine("[YT] Loaded embedded Harmony dependency.");
            return _harmonyAssembly;
        }
    }
}
