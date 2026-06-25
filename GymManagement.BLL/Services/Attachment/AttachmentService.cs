using GymManagement.BLL.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024;
        private readonly ILogger<AttachmentService> logger;
        private readonly IWebHostEnvironment env;
        private readonly string[] _allowedExtensions = { ".png", ".jpeg", ".jpg" };
        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }

        public bool Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(env.ContentRootPath, folderName, fileName);
            try
            {

                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Failed to Delete Attachment {fileName}");
                return false;
            }
        }

        public Result<(Stream stream, string contentType)> GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return Result<(Stream stream, string contentType)>.Fail("File Name Cannot be null");
            if (string.IsNullOrWhiteSpace(folderName)) return Result<(Stream stream, string contentType)>.Fail("Folder Name Cannot be null");

            var fullPath = Path.Combine(env.ContentRootPath, folderName, fileName);
            if (!File.Exists(fullPath)) return Result<(Stream stream, string contentType)>.NotFound("file not found");

            var stream = new FileStream(fullPath , FileMode.Open , FileAccess.Read);
            var extension = Path.GetExtension(fullPath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpg",
                 _ => "application/octet-stream" // Binary Data
            };
            return Result<(Stream stream, string contentType)>.OK((stream, contentType));
        }

        public async Task<Result<string>> UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            if (fileStream == null || fileStream.Length == 0) return Result<string>.NotFound("file is empty");
            if (!fileStream.CanRead) return Result<string>.Fail("Failed to read file");

            if (fileStream.Length > _maxFileSize) {
                logger.LogError($"File Rejected : File Too Large {fileStream.Length} Bytes");
                return Result<string>.Fail("File Size Is More than 5 MB");
            }
            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtensions.Contains(extension))
            {
                logger.LogError($"File Rejected : Extension {extension} Not Allowed");
                return Result<string>.Fail("File Extension is Rejected");
            }

            var uploadFolder = Path.Combine(env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadFolder);

            var storedFileName = $"{Guid.NewGuid()}{fileName}";
            var filePath = Path.Combine(uploadFolder, storedFileName);
            try
            {
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                await fileStream.CopyToAsync(fs, ct);
                return Result<string>.OK(storedFileName);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Failed To Upload File {fileName}");
                return Result<string>.Fail($"Failed To Upload File {fileName}");
            }

        }
    }
}
