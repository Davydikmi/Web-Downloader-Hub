namespace Web_Downloader_Hub.Serivce.Handlers
{
    // Service class for choosing right metod of downloading file by url
    public class HandlerSelectorService
    {
        public static IDownloadHandler GetHandler(string url)
        {
            if (url.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                return new DataUriHandler();

            //if (url.Contains("youtube.com"))
            //    return new YouTubeHandler();

            return new HttpHandler();
        }
    }
}
