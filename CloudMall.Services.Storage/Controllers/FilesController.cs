using CloudMall.Services.Storage.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using WeihanLi.Common.Models;

namespace CloudMall.Services.Storage.Controllers
{
    [ApiController]
    [Route("api/storage")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IStorageProvider _storageProvider;

        private static readonly Dictionary<string, string> ExtTable = new Dictionary<string, string>
        {
            {"images", "gif,jpg,jpeg,png,bmp"},
            {"files", "doc,docx,xls,xlsx,ppt,pptx,pdf,txt,zip"}
        };

        private const int MaxSize = 100 * 1024 * 1024;

        public FilesController(ILogger<FilesController> logger, IStorageProvider storageProvider)
        {
            _logger = logger;
            _storageProvider = storageProvider;
        }

        [HttpPost("files")]
        public async Task<IActionResult> PostFile(IFormFile formFile, [FromQuery]string dir)
        {
            //file root dir path 文件保存目录路径
            if (formFile?.FileName == null)
            {
                return BadRequest(ResultModel.Fail<string>("上传文件为空"));
            }

            if (string.IsNullOrEmpty(dir))
            {
                dir = "files";
            }
            return await UploadFile(formFile, dir);
        }

        [HttpPost("images")]
        public async Task<IActionResult> PostImages(IFormFile formFile)
        {
            if (formFile?.FileName == null)
            {
                return BadRequest(ResultModel.Fail<string>("上传文件为空"));
            }

            return await UploadFile(formFile, "images");
        }

        private async Task<IActionResult> UploadFile(IFormFile formFile, string dirName)
        {
            if (!ExtTable.ContainsKey(dirName))
            {
                return BadRequest(ResultModel.Fail<string>("目录名不正确"));
            }
            var savePath = "/upload/";

            var fileExt = Path.GetExtension(formFile.FileName)?.ToLower() ?? string.Empty;
            if (formFile.Length > MaxSize)
            {
                return BadRequest(ResultModel.Fail<string>("上传文件大小超过限制"));
            }
            if (string.IsNullOrEmpty(fileExt) ||
                Array.IndexOf((ExtTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                return BadRequest(ResultModel.Fail<string>($"上传文件扩展名是不允许的扩展名。\n只允许{ExtTable[dirName]}格式。"));
            }
            savePath += dirName + "/";
            var ymd = DateTime.UtcNow.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
            savePath += ymd + "/";

            var newFileName = DateTime.UtcNow.ToString("HHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
            var filePath = savePath + newFileName;
            //save file
            await using var stream = new MemoryStream();
            await formFile.CopyToAsync(stream);

            var fileUrl = await _storageProvider.SaveBytes(stream.ToArray(), filePath);
            if (!string.IsNullOrEmpty(fileUrl))
            {
                _logger.LogInformation($"file({formFile.FileName}) saved success, savedUrl:{fileUrl}");
                return Ok(ResultModel.Success(fileUrl));
            }

            return BadRequest(ResultModel.Fail<string>($"文件上传失败"));
        }
    }
}