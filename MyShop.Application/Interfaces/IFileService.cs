using Microsoft.AspNetCore.Http;
using MyShop.Application.Common.ResultPattern;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyShop.Application.Interfaces
{
    public interface IFileService
    {
        
        Task<Result<string>> SaveFileAsync(IFormFile file, string folderName);

       
        Task<Result<List<string>>> SaveFilesAsync(List<IFormFile> files, string folderName);

        
        Result<bool> DeleteFile(string fileName, string folderName);

        
        Result<string> GetFilePath(string fileName, string folderName);
    }
}
