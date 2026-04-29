using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyShop.CORE.Helpers.ResultPattern;
using MyShop.CORE.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MyShop.CORE.Implmentations
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Result<string>> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return Result<string>.Failure("The file is empty", "400");

            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var extension = Path.GetExtension(file.FileName);
            var trustedFileName = Guid.NewGuid().ToString() + extension;
            var finalPath = Path.Combine(folderPath, trustedFileName);

            using (var fileStream = new FileStream(finalPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Result<string>.Success(trustedFileName);
        }

        public async Task<Result<List<string>>> SaveFilesAsync(List<IFormFile> files, string folderName)
        {
            var savedFileNames = new List<string>();

            foreach (var file in files)
            {
                var result = await SaveFileAsync(file, folderName);
                if (!result.IsSuccess)
                    continue;

                savedFileNames.Add(result.Data);
            }

            return Result<List<string>>.Success(savedFileNames);
        }

        public Result<bool> DeleteFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Result<bool>.Failure("The file name is empty", "400");

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (!File.Exists(filePath))
                return Result<bool>.Failure("The file does not exist", "404");

            File.Delete(filePath);
            return Result<bool>.Success(true);
        }

        public Result<string> GetFilePath(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Result<string>.Failure("The file name is empty", "400");

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folderName, fileName);

            if (!File.Exists(filePath))
                return Result<string>.Failure("The file does not exist", "404");

            return Result<string>.Success(filePath);
        }
    }
}
