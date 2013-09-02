using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TwoPS.Processes
{
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class ProcessUtils
    {
        private static string FindExecutableInternal(string exeName, IEnumerable<string> paths)
        {
            foreach (var path in paths)
            {
                var file = Path.Combine(path, exeName);
                if (File.Exists(file))
                {
                    return file;
                }
            }
            return exeName;
        }

        /// <summary>
        /// Searches for an executable file in a list of directories and in the system path.
        /// </summary>
        /// <param name="exeName">The name of the executable file to find</param>
        /// <param name="paths">A list of paths in wich to search for the executable file</param>
        /// <returns>The path of the first file found whose name is <paramref name="exeName"/></returns>
        public static string FindExecutable(string exeName, IEnumerable<string> paths = null)
        {
            paths = paths ?? new List<string>();
            return FindExecutableInternal(exeName, paths.Union(Environment.GetEnvironmentVariable("PATH").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)));
        }
    }
}
