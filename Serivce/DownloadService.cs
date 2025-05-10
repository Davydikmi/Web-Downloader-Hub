using Web_Downloader_Hub.Models;
using Web_Downloader_Hub.Serivce.Handlers;
using System.IO.Compression;
namespace Web_Downloader_Hub.Serivce
{
    public class DownloadService
    {
        public DownloadRecord AddToQueue(string url)
        {
            // Проверка валидности
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
             {
                Console.WriteLine("Неверный формат URL");
                throw new Exception("Неверный формат URL");
            }

            // Выбор правильного загрузчика
            var handler = HandlerSelectorService.GetHandler(url);

            // Скачивание файла
            string filePath = handler.Download(url);

            // Получение информации о файле
            FileInfo fileInfo = FileService.GetFileInfo(filePath);

            // Сохранение записи
            var record = new DownloadRecord
            {
                Id = Guid.NewGuid(),
                url = url,
                filename = fileInfo.Name,
                FileSize = fileInfo.Length,
                filepath = filePath,
                DownloadDate = DateTime.Now,
            };

            // добавление записи в json в классе DatabaseService
            return record;
        }

        public void DeleteFromQueue(string fileName)
        {
            string downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads");
            string filePath = Path.Combine(downloadDir, fileName);
            if (File.Exists(filePath)) File.Delete(filePath);
            // здесь также можно добавить удаление файла из бд
        }

        public void DeleteFiles(List<string> filenames)
        {
            string downloadDir = Path.Combine(AppContext.BaseDirectory, "Downloads");

            foreach (var name in filenames)
            {
                string filePath = Path.Combine(downloadDir, name);
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            // можно также очистить очередь из памяти или БД
        }

        public (byte[] Data, string FileName, string ContentType) PrepareDownload(List<string> filePaths)
        {
            if (filePaths == null || filePaths.Count == 0)
                throw new ArgumentException("Список файлов пуст");

            if (filePaths.Count == 1)
            {
                string path = filePaths[0];
                return (File.ReadAllBytes(path), Path.GetFileName(path), "application/octet-stream");
            }
            else
            {
                using var memoryStream = new MemoryStream();
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in filePaths)
                    {
                        if (File.Exists(filePath))
                        {
                            var entry = archive.CreateEntry(Path.GetFileName(filePath));
                            using var entryStream = entry.Open();
                            using var fileStream = File.OpenRead(filePath);
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                return (memoryStream.ToArray(), "downloaded_files.zip", "application/zip");
            }
        }



    }
}
