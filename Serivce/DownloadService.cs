using Web_Downloader_Hub.Models;
using System.IO;
using System;
using Web_Downloader_Hub.Serivce.Handlers;
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




    }
}
