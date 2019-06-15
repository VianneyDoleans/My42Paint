using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ResourceSolution
{
    public class GiveRessource
    {
        public static string GetFile()
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = Path.GetDirectoryName(dirPath);
            return Path.GetFullPath(Path.Combine(dirPath, "Resouces/saves.txt"));
        }
    }
}
