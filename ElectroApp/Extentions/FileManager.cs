using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ElectroApp.Extentions
{
    public static class FileManager
    {
        public static bool IsImage(this IFormFile file)
        {
            return file.ContentType.Contains("image/");
        }
        public static bool CheckSize(this IFormFile file, int size)
        {
            return file.Length / Math.Pow(2, 20) <= size;
        }
        public static string SaveImg(this IFormFile file, string root, string folder)
        {
            string rootPath = Path.Combine(root, folder);
            string filename = Guid.NewGuid().ToString() + file.FileName;
            string fullpath = Path.Combine(rootPath, filename);
            using (FileStream fileStream = new FileStream(fullpath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            return filename;
        }
    }
}
