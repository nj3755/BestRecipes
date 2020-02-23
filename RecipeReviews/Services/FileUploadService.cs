using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeReviews.Services
{
    public class FileUploadService
    {
        private readonly Regex _validFileNames;
        private readonly int _invalidFilenameLength;
        private readonly int _invalidSize;

        public FileUploadService(string validFilenamesRegex, int invalidFilenameLength, int invalidSize)
        {
            _validFileNames = new Regex(validFilenamesRegex);
            _invalidFilenameLength = invalidFilenameLength;
            _invalidSize = invalidSize;
        }


        public bool InvalidExtension(IFormFile file) => !_validFileNames.Match(file.FileName).Success;

        public bool InvalidNameLength(IFormFile file) => file.FileName.Length < _invalidFilenameLength;

        public bool InvalidSize(IFormFile file) => file.Length > _invalidSize;

        public async Task<bool> TryUpload(IFormFile file, string fileName, IHostingEnvironment appEnvironment)
        {
            try
            {
                var filePath = Path.Combine(appEnvironment.ContentRootPath, Program.UploadsDirectory, Program.ImagesDirectory, fileName);
                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fs);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Delete(IFormFile file, IHostingEnvironment hostingEnv)
        {
            Delete(file.FileName, hostingEnv);
        }

        public void Delete(string fileName, IHostingEnvironment hostingEnv)
        {
            try
            {
                var filePath = Path.Combine(hostingEnv.ContentRootPath, Program.UploadsDirectory, Program.ImagesDirectory, fileName);
                File.Delete(filePath);
            }
            catch (Exception)
            {
            }
        }

        public void DeleteIfNotNull(IFormFile file, IHostingEnvironment hostingEnv)
        {
            if (file != null && file.FileName != null)
            {
                Delete(file.FileName, hostingEnv);
            }
        }

        public void DeleteIfNotNull(string fileName, IHostingEnvironment hostingEnv)
        {
            if (fileName != null)
            {
                Delete(fileName, hostingEnv);
            }
        }
    }
}
