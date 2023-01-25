using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YannikG.TSBE.Pagedownloader.Console.Access
{
    public class FileAccessHandler
    {
        private readonly string baseFolderPath;

        public FileAccessHandler(string baseFolderPath)
        {
            this.baseFolderPath = baseFolderPath;
            Directory.CreateDirectory(baseFolderPath);
        }

        public bool DoesFileAlreadyExists(string fileNameWithPath)
        {
            string path = calculateFullPath(fileNameWithPath);

            return File.Exists(path);
        }

        public void SaveFile(byte[] fileData, string fileWithPathAndFormat)
        {
            string path = calculateFullPath(fileWithPathAndFormat);

            var file = new FileInfo(path);

            file.Directory!.Create();

            File.WriteAllBytes(path, fileData);
        }

        public string GetAbsoluteBasePath()
        {
            return calculateFullPath("");
        }

        private string calculateFullPath(string lastPath)
        {
            lastPath = lastPath.StartsWith("/") ? lastPath.TrimStart('/') : lastPath;

            string basePath = Path.Combine(Environment.CurrentDirectory, baseFolderPath);

            return Path.Combine(basePath, lastPath);
        }
    }
}