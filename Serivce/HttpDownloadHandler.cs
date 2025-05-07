using System.IO;
using System.Net.Http;

namespace Web_Downloader_Hub.Serivce
{
    public class HttpDownloadHandler:IDownloadHandler
    {
        public string Download(string url)
        {
            using HttpClient client = new HttpClient();
            var bytes = client.GetByteArrayAsync(url).Result;

            string fileName = Path.GetFileName(new Uri(url).LocalPath);
            string path = Path.Combine("Downloads", fileName);

            File.WriteAllBytes(path, bytes);
            return path;
        }


    }
}
