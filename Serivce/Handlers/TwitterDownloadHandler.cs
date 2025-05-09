using ImageMagick;
using System.Diagnostics;

namespace Web_Downloader_Hub.Serivce.Handlers
{
    public class TwitterDownloadHandler : IDownloadHandler
    {
        public string Download(string url)
        {
            string ytDlpPath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");
            if (!File.Exists(ytDlpPath))
                throw new FileNotFoundException("yt-dlp.exe не найден. Поместите его в папку tools.");

            string downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads");
            Directory.CreateDirectory(downloadDir);

            string fileNameTemplate = Path.Combine(downloadDir, "twitter_%(id)s.%(ext)s");

            var processInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"\"{url}\" -o \"{fileNameTemplate}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"Ошибка при загрузке с Twitter: {error}");
            }

            var latestFile = Directory.GetFiles(downloadDir)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .FirstOrDefault();

            if (latestFile == null || !latestFile.Exists)
                throw new Exception("Медиафайл из Twitter не найден после загрузки.");

            return latestFile.FullName;
        }
    }
}
