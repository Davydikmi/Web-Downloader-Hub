using System.Diagnostics;

namespace Web_Downloader_Hub.Serivce.Handlers
{
    public class YouTubeDownloadHandler : IDownloadHandler
    {
        public string Download(string url)
        {
            string ytDlpPath = Path.Combine(AppContext.BaseDirectory, "yt-dlp.exe");
            if (!File.Exists(ytDlpPath))
                throw new FileNotFoundException("yt-dlp.exe не найден. Поместите его в папку tools.");

            string downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads");
            Directory.CreateDirectory(downloadDir);

            string outputPath = Path.Combine(downloadDir, "%(title)s.%(ext)s");

            var processInfo = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"\"{url}\" -o \"{outputPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception("yt-dlp завершился с ошибкой:\n" + process.StandardError.ReadToEnd());

            // Можно найти скачанный файл по маске
            var downloadedFile = Directory.GetFiles(downloadDir)
                .OrderByDescending(File.GetCreationTime)
                .FirstOrDefault();

            return downloadedFile ?? throw new Exception("Файл не найден после загрузки.");
        }
    }

}
