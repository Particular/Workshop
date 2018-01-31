using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Divergent.ITOps
{
    public static class ReflectionHelper
    {
        public static IEnumerable<Assembly> GetAssemblies(string path, string nameEndsWith)
        {
            var executionPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var searchPath = Path.Combine(executionPath, path);
            var files = Directory.EnumerateFiles(searchPath, "*.*", SearchOption.AllDirectories)
                .Where(s => s.EndsWith(nameEndsWith));

            var assemblies = new List<Assembly>();
            foreach (var file in files)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFile(file));
                }
                catch (FileLoadException) { } // Already loaded.
                catch (BadImageFormatException) { } // Not a valid assembly.
            }

            return assemblies;
        }
    }
}
