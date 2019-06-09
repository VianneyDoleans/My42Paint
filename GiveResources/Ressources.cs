using System;
using System.IO;
using System.Reflection;

namespace GiveResources
{
    public class Ressources
    {
        public static string GetFile()
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = Path.GetDirectoryName(dirPath);
            return Path.GetFullPath(Path.Combine(dirPath, "Resources/saves.txt"));
        }
    }
}
