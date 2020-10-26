using Microsoft.EntityFrameworkCore;
using FQCS.Admin.Business.Models;
using FQCS.Admin.Business.Queries;
using FQCS.Admin.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TNT.Core.Helpers.DI;
using FQCS.Admin.Business.Helpers;
using static FQCS.Admin.Business.Constants;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FQCS.Admin.Business.Services
{
    public class FileService : Service
    {
        public FileService(ServiceInjection inj) : base(inj)
        {
        }

        public async Task SaveFile(IFormFile file, string filePath)
        {
            var folderPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }
        }

        public (string, string) GetPath(string folderPath, string fileName = null, string ext = null)
        {
            if (fileName == null)
            {
                if (ext != null && !ext.Contains("."))
                    ext = "." + ext;
                fileName = Path.GetRandomFileName() + ext ?? "";
            }
            var filePath = Path.Combine(folderPath, fileName);
            var fullFolderPath = Path.GetFullPath(folderPath);
            var relativePath = filePath.Replace(fullFolderPath, "");
            var absPath = Path.GetFullPath(filePath);
            return (relativePath, absPath);
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public void DeleteDirectory(string folderPath, bool recursive = true)
        {
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, recursive);
        }
    }
}
