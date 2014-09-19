using System.IO;

namespace ITJakub.ITJakubService.Core
{
    public class LocalFilesystemManager
    {
        public string SaveToTempFolder(Stream dataStream)
        {
            string pathString = Path.Combine("D:\\", "UploadedFiles");


            if (!Directory.Exists(pathString))
                Directory.CreateDirectory(pathString);

            //string path = string.Format("{0}\\{1}", pathString, file.FileName);
            return "";
        }
    }
}