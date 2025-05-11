using Newtonsoft.Json;
using System.Text.Json;
using Web_Downloader_Hub.Models;

namespace Web_Downloader_Hub.Serivce
{
    public class HistoryStatService
    {
        private readonly string _jsonpath;
        public List<DownloadRecord> DownloadRecords { get; set; } = new List<DownloadRecord>();

        public HistoryStatService()
        {
            _jsonpath = Path.Combine(AppContext.BaseDirectory, "Records.json");
            LoadRecords();
        }

        // Десереализация данных и их загрузка в DownloadRecords
        public void LoadRecords()
        {
            if (!File.Exists(_jsonpath))
            {
                string json = JsonConvert.SerializeObject(DownloadRecords, Formatting.Indented);
                File.WriteAllText(_jsonpath, json);
            }
            else
            {
                string json = File.ReadAllText(_jsonpath);
                DownloadRecords = JsonConvert.DeserializeObject<List<DownloadRecord>>(json) ?? new();
            }
        }

        // Сохранение записей в файл
        private void SaveRecords()
        {
            string json = JsonConvert.SerializeObject(DownloadRecords, Formatting.Indented);
            File.WriteAllText(_jsonpath, json);
        }


        // Добавление записи
        public void AddRecord(DownloadRecord record)
        {
            DownloadRecords.Add(record);
            SaveRecords();
        }

        // Удаление одной записи
        public void DeleteRecord(DownloadRecord record)
        {
            DownloadRecords.RemoveAll(r => r.Id == record.Id);
            SaveRecords();
        }

        // Очистка всех записей
        public void ClearRecords()
        {
            DownloadRecords.Clear();
            SaveRecords();
        }




    }
}
