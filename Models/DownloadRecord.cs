using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Web_Downloader_Hub.Models
{
    public class DownloadRecord
    {
        [JsonPropertyName("id")]           public Guid Id { get; set; }
        [JsonPropertyName("url")]          public string url { get; set; }
        [JsonPropertyName("filepath")]     public string filepath { get; set; }
        [JsonPropertyName("filename")]     public string filename { get; set; }
        [JsonPropertyName("fileSize")]     public long FileSize { get; set; } // size in bytes
        [JsonPropertyName("downloadDate")] public DateTime DownloadDate { get; set; }
    }
}
