using Web_Downloader_Hub.Models;

namespace Web_Downloader_Hub.Serivce
{
    public class DownloadService
    {
        public DownloadRecord AddToQueue(string url)
        {
            // Проверка валидности
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                throw new Exception("Неверный формат URL");

            // Выбор правильного загрузчика
            var handler = HandlerSelectorService.GetHandler(url);

            // Скачивание файла
            var filePath = handler.Download(url);

            // Получение информации о файле
            var fileInfo = FileService.GetFileInfo(filePath);

            // Сохранение записи
            var record = new DownloadRecord
            {
                Id = Guid.NewGuid(),
                url = url,
                filename = fileInfo.Name,
                FileSize = fileInfo.Length,
                filepath = filePath,
                DownloadDate = DateTime.Now
            };
            return record;
        }






    }
}
