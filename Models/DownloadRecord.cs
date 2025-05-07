namespace Web_Downloader_Hub.Models
{
    public class DownloadRecord
    {
        public Guid Id { get; set; }
        public string url { get; set; }
        public string filepath { get; set; }
        public string filename { get; set; }
        public long FileSize { get; set; } // size in bytes
        public DateTime DownloadDate { get; set; }




        // Convert file size from bytes into Mb Gb Tb
        public string ToReadableSize()
        {
            string[] sizes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
            long len = FileSize;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        

       

    }


}
