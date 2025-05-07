namespace Web_Downloader_Hub.Serivce
{
    public class FileService
    {
        public static FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }


    }
}
