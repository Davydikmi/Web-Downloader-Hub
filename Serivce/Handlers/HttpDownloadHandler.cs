using System.IO;
using System.Net.Http;
using System.Web;

namespace Web_Downloader_Hub.Serivce.Handlers
{
    public class HttpDownloadHandler : IDownloadHandler
    {
        public string Download(string url)
        {
            using HttpClient client = new HttpClient();
            using var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            // Получаем байты файла
            var bytes = response.Content.ReadAsByteArrayAsync().Result;

            // Определяем расширение из Content-Type
            var contentType = response.Content.Headers.ContentType?.MediaType;
            string extension = GetExtensionFromMimeType(contentType);

            // Пытаемся извлечь имя файла из URL
            var uri = new Uri(url);
            string fileName = Path.GetFileName(HttpUtility.UrlDecode(uri.LocalPath));

            if (string.IsNullOrEmpty(Path.GetExtension(fileName)) && !string.IsNullOrEmpty(extension))
            {
                fileName += extension;
            }

            // Папка загрузки
            string downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads");
            Directory.CreateDirectory(downloadDir);

            string filePath = Path.Combine(downloadDir, fileName);
            File.WriteAllBytes(filePath, bytes);
            return filePath;
        }

        private string GetExtensionFromMimeType(string? mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
                return ".bin";

            return mimeType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                "image/bmp" => ".bmp",
                "image/svg+xml" => ".svg",
                "video/mp4" => ".mp4",
                "video/webm" => ".webm",
                "application/pdf" => ".pdf",
                "application/msword" => ".doc",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
                "application/vnd.ms-excel" => ".xls",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => ".xlsx",
                "application/zip" => ".zip",
                "application/x-rar-compressed" => ".rar",
                "text/html" => ".html",
                "text/plain" => ".txt",
                _ => ".bin"
            };
        }
    }
}
