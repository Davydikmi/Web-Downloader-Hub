namespace Web_Downloader_Hub.Serivce
{
    // Service class for choosing right metod of downloading file by url
    public class HandlerSelectorService
    {
        public static IDownloadHandler GetHandler(string url)
        {
            //if (url.Contains("youtube.com"))
            //    return new YouTubeHandler();

            return new HttpDownloadHandler(); 
        }
    }
}
