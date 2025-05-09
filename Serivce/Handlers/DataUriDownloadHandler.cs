using System.Text.RegularExpressions;

namespace Web_Downloader_Hub.Serivce.Handlers
{
    public class DataUriDownloadHandler:IDownloadHandler
    {
        public string Download(string url)
        {
            var match = Regex.Match(url, @"data:(?<mime>[\w/+.-]+);base64,(?<data>.+)");
            if (!match.Success)
                throw new ArgumentException("Invalid data URI format");

            string mimeType = match.Groups["mime"].Value;
            string base64Data = match.Groups["data"].Value;
            byte[] bytes = Convert.FromBase64String(base64Data);

            // Генерация расширения из MIME-типа
            string extension = mimeType switch
            {
                "image/png" => ".png",
                "image/jpeg" => ".jpg",
                "image/gif" => ".gif",

                "text/plain" => ".txt",

                "video/mp4" => ".mp4",
                "video/webm" => ".webm",
                "video/ogg" => ".ogv",
                "video/quicktime" => ".mov",
                "video/x-msvideo" => ".avi",
                "video/x-flv" => ".flv",
                "video/mpeg" => ".mpeg",
                "application/octet-stream" => ".mp4",

                _ => ".bin"
            };


            string fileName = $"data_uri_{Guid.NewGuid()}{extension}";

            string currentDir = AppContext.BaseDirectory;
            string downloadDir = Path.Combine(currentDir, "Downloads");

            if (!Directory.Exists(downloadDir))
                Directory.CreateDirectory(downloadDir);

            string filePath = Path.Combine(downloadDir, fileName);
            File.WriteAllBytes(filePath, bytes);

            return filePath;
        }

    }
}
