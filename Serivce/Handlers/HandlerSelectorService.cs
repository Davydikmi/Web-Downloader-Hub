namespace Web_Downloader_Hub.Serivce.Handlers
{
    // Service class for choosing right metod of downloading file by url
    public class HandlerSelectorService
    {
        public static IDownloadHandler GetHandler(string url)
        {
            if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return new DataUriDownloadHandler();

            else if (url.Contains("youtube.com") || url.Contains("youtu.be")) return new YouTubeDownloadHandler();

            else if (url.Contains("x.com")) return new TwitterDownloadHandler();

            return new HttpDownloadHandler();

        }
    }
}
