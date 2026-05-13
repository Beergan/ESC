using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using ESC.CONCOST.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ESC.CONCOST.Base.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private IWebHostEnvironment _hostingEnv;

    public UploadController(IWebHostEnvironment env)
    {
        this._hostingEnv = env;
    }

    [HttpPost("SaveDocument")]
    public void SaveDocument(IList<IFormFile> chunkFile, IList<IFormFile> UploadFiles)
    {
        long size = 0;
        try
        {
            foreach (var file in UploadFiles)
            {
                var contentDisposition = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                var originalFileName = contentDisposition.FileName?.Trim('"') ?? file.FileName;

                var fileExt = Path.GetExtension(originalFileName);

                var fileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}{fileExt}";

                var relativePath = $"/upload/document{fileName}";

                var serverPath = Path.Combine(_hostingEnv.WebRootPath, "upload", "document", fileName);

                size += file.Length;

                var folderPath = Path.GetDirectoryName(serverPath);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                if (!System.IO.File.Exists(serverPath))
                {
                    using (var fs = new FileStream(serverPath, FileMode.Create))
                    {
                        file.CopyTo(fs);
                        fs.Flush();
                    }
                }

                Response.Headers.Append(nameof(relativePath), relativePath);
            }
        }
        catch (Exception e)
        {
            Response.Clear();
            Response.StatusCode = 204;
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
        }
    }

    [AllowAnonymous]
    [HttpPost("SaveLogo")]
    public IActionResult SaveLogo(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0) return Ok(ResultOf<string>.Error("No file uploaded"));

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = $"/upload/company/{fileName}";
            var serverPath = Path.Combine(_hostingEnv.WebRootPath, "upload", "company", fileName);

            var folderPath = Path.GetDirectoryName(serverPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var fs = new FileStream(serverPath, FileMode.Create))
            {
                file.CopyTo(fs);
            }

            return Ok(ResultOf<string>.Ok(relativePath));
        }
        catch (Exception e)
        {
            return Ok(ResultOf<string>.Error(e.Message));
        }
    }

    [AllowAnonymous]
    [HttpPost("SaveAvatar/{guid}")]
    public IActionResult SaveAvatar(Guid guid, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0) return Ok(ResultOf<string>.Error("No file uploaded"));

            var fileName = $"avatar_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var relativePath = $"/upload/employee/{guid}/{fileName}";
            var serverPath = Path.Combine(_hostingEnv.WebRootPath, "upload", "employee", guid.ToString(), fileName);

            var folderPath = Path.GetDirectoryName(serverPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (var fs = new FileStream(serverPath, FileMode.Create))
            {
                file.CopyTo(fs);
            }

            return Ok(ResultOf<string>.Ok(relativePath));
        }
        catch (Exception e)
        {
            return Ok(ResultOf<string>.Error(e.Message));
        }
    }

    [HttpPost("Remove")]
    public void Remove(IList<IFormFile> UploadFiles)
    {
        try
        {
            var filename = _hostingEnv.ContentRootPath + $@"{UploadFiles[0].FileName}";
            if (System.IO.File.Exists(filename))
            {
                System.IO.File.Delete(filename);
            }
        }
        catch (Exception e)
        {
            Response.Clear();
            Response.StatusCode = 200;
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
        }
    }

    [HttpPost("UploadChunk")]
    public async Task<IActionResult> UploadChunk([FromForm] IFormFile chunk, [FromForm] string fileGuid, [FromForm] int chunkIndex, [FromForm] int totalChunks, [FromForm] string fileName, [FromForm] string? projectFolder = null)
    {
        try
        {
            if (chunk == null || chunk.Length == 0) return BadRequest("Invalid chunk");

            var tempPath = Path.Combine(_hostingEnv.WebRootPath, "upload", "temp", fileGuid);
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            var chunkPath = Path.Combine(tempPath, chunkIndex.ToString());
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }

            // Check if all chunks are uploaded
            var uploadedChunks = Directory.GetFiles(tempPath).Length;
            if (uploadedChunks == totalChunks)
            {
                // Reassemble
                var folderName = string.IsNullOrEmpty(projectFolder) ? "project_docs" : $"project_docs/{projectFolder}";
                var finalDir = Path.Combine(_hostingEnv.WebRootPath, "upload", folderName);
                if (!Directory.Exists(finalDir)) Directory.CreateDirectory(finalDir);

                var finalFileName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{fileName}";
                var finalPath = Path.Combine(finalDir, finalFileName);

                using (var finalStream = new FileStream(finalPath, FileMode.Create))
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        var partPath = Path.Combine(tempPath, i.ToString());
                        using (var partStream = new FileStream(partPath, FileMode.Open))
                        {
                            await partStream.CopyToAsync(finalStream);
                        }
                    }
                }

                // Cleanup
                Directory.Delete(tempPath, true);

                return Ok(ResultOf<string>.Ok($"/upload/{folderName}/{finalFileName}"));
            }

            return Ok(ResultOf<string>.Ok("Chunk uploaded"));
        }
        catch (Exception ex)
        {
            return Ok(ResultOf<string>.Error(ex.Message));
        }
    }
}