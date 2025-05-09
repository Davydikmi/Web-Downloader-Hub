using System.IO;
using System.Net.Http;

namespace Web_Downloader_Hub.Serivce.Handlers
{
    public class HttpDownloadHandler : IDownloadHandler
    {
        public string Download(string url)
        {
            using HttpClient client = new HttpClient();
            var bytes = client.GetByteArrayAsync(url).Result;

            string fileName = Path.GetFileName(new Uri(url).LocalPath);

            // Путь к текущей директории (bin/Debug/net8.0)
            string currentDir = AppContext.BaseDirectory;
            string downloadDir = Path.Combine(currentDir, "Downloads");

            // Создание папки, если её нет
            if (!Directory.Exists(downloadDir))
                Directory.CreateDirectory(downloadDir);

            string path = Path.Combine(downloadDir, fileName);

            File.WriteAllBytes(path, bytes);
            return path;
        }
    }
}
