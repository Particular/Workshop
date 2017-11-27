using Microsoft.Extensions.DependencyModel;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ITOps.ViewModelComposition
{
    internal static class AssemblyLoader
    {
        public static Assembly Load(string path)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(path);

            return IsCompileLibrary(assemblyName) || IsRuntimeLibrary(assemblyName)
                ? Assembly.Load(new AssemblyName(assemblyName))
                : AssemblyLoadContext.Default.LoadFromAssemblyPath(path);

            bool IsCompileLibrary(string name) =>
                DependencyContext.Default.CompileLibraries
                    .Any(library => library.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            bool IsRuntimeLibrary(string name) =>
                DependencyContext.Default.RuntimeLibraries
                    .Any(library => library.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
