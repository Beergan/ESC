using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public static class UploadHelper
{
    public async static Task<Tuple<string, string>> ImageBase64(IBrowserFile file, int size = 5, string accept = ".png, .jpg, .jpeg", ITextTranslator? text = null)
    {
        if (file != null)
        {
            if (file.Size > size * 1024 * 1024)
            {
                var msg = text != null ? text[$"Kích thước file vượt quá {size}MB|파일 크기가 {size}MB를 초과했습니다"] : $"Kích thước file vượt quá {size}MB";
                return new Tuple<string, string>("error", msg);
            }

            var ext = Path.GetExtension(file.Name).ToLowerInvariant();

            string[] extensions = accept.Split(',').Select(x => x.Trim().ToLower()).ToArray();

            if (!extensions.Contains(ext))
            {
                var msg = text != null ? text[$"File không đúng định dạng ({accept})|파일 형식이 올바르지 않습니다 ({accept})"] : $"File không đúng định dạng ({accept})";
                return new Tuple<string, string>("error", msg);
            }

            using var stream = new MemoryStream();
            using var readStream = file.OpenReadStream(maxAllowedSize: size * 1024 * 1024);
            await readStream.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            byte[] imageBytes = stream.ToArray();
            string base64String = Convert.ToBase64String(imageBytes);

            string imageBase64 = $"data:{file.ContentType};base64,{base64String}";

            return new Tuple<string, string>("success", imageBase64);
        }
        else
        {
            var msg = text != null ? text["File không tồn tại ..|파일이 존재하지 않습니다 .."] : "File không tồn tại ..";
            return new Tuple<string, string>("error", msg);
        }
    }

    public async static Task<Tuple<string, string>> UploadFile(HttpClient http, IBrowserFile file, string url = "api/Upload/SaveLogo", int size = 5, string accept = ".png, .jpg, .jpeg", ITextTranslator? text = null)
    {
        if (file != null)
        {
            if (file.Size > size * 1024 * 1024)
            {
                var msg = text != null ? text[$"Kích thước file vượt quá {size}MB|파일 크기가 {size}MB를 초과했습니다"] : $"Kích thước file vượt quá {size}MB";
                return new Tuple<string, string>("error", msg);
            }

            var ext = Path.GetExtension(file.Name).ToLowerInvariant();
            string[] extensions = accept.Split(',').Select(x => x.Trim().ToLower()).ToArray();
            if (!extensions.Contains(ext))
            {
                var msg = text != null ? text[$"File không đúng định dạng ({accept})|파일 형식이 올바르지 않습니다 ({accept})"] : $"File không đúng định dạng ({accept})";
                return new Tuple<string, string>("error", msg);
            }

            try
            {
                using var content = new MultipartFormDataContent();
                using var stream = file.OpenReadStream(maxAllowedSize: size * 1024 * 1024);
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                var response = await http.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResultOf<string>>();
                    if (result != null && result.Success)
                    {
                        return new Tuple<string, string>("success", result.Item);
                    }
                    return new Tuple<string, string>("error", result?.Message ?? "Upload failed");
                }
                return new Tuple<string, string>("error", $"Server error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                return new Tuple<string, string>("error", ex.Message);
            }
        }
        var finalMsg = text != null ? text["File không tồn tại ..|파일이 존재하지 않습니다 .."] : "File không tồn tại ..";
        return new Tuple<string, string>("error", finalMsg);
    }
}