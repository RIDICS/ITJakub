using System;
using System.IO;

namespace ITJakub.FileStorage
{




    public class FileSystemManager
    {
        private const string FrontPageDirName = "FrontPage";
        private const string ImagesFolderName = "Images";
        private const string TempFolderName = "Temp";
        private readonly string m_path;
        private readonly string m_tempFolderPath;

        public FileSystemManager(string path)
        {
            m_path = path;
            m_tempFolderPath = Path.Combine(m_path, TempFolderName);
            MakeDirectoryIfNotExist(m_tempFolderPath);
        }

        //makes all directories in specified path
        private void MakeDirectoryIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void SaveFile(string guid, Stream data)
        {
            string path = GetFilePath(guid);
            string parentDirName = Path.GetDirectoryName(path);
            if (parentDirName != null)
                MakeDirectoryIfNotExist(parentDirName);
            using (FileStream fileStream = File.Create(path))
            {
                data.CopyTo(fileStream);
            }
        }

        public string SaveTempFile(Stream dataStream)
        {
            string guid = Guid.NewGuid().ToString();
            SaveFile(GetTempFilePath(guid), dataStream);
            return guid;
        }

        public void RenameTempFile(string oldName, string newName)
        {
            var file = new FileInfo(GetTempFilePath(oldName));
            file.MoveTo(GetTempFilePath(newName));
        }

        public FileStream OpenTempFile(string fileName)
        {
            return File.Open(GetTempFilePath(fileName), FileMode.Open);
        }

        public void SaveImage(string fileGuid, string imageName, Stream dataStream)
        {
            string imagePath = GetImagePath(fileGuid, imageName);
            SaveFile(imagePath, dataStream);
        }

        public void SaveFrontImage(string fileGuid, string imageName, Stream dataStream)
        {
            string imagePath = GetFrontImagePath(fileGuid, imageName);
            SaveFile(imagePath, dataStream);
        }

        private string GetTempFilePath(string fileGuid)
        {
            return Path.Combine(GetTempFileFolder(fileGuid), fileGuid);
        }

        private string GetFilePath(string fileGuid)
        {
            return Path.Combine(GetFileFolder(fileGuid), fileGuid);
        }

        private string GetImagePath(string fileGuid, string imageName)
        {
            return Path.Combine(GetImageFolder(fileGuid), imageName);
        }

        private string GetFrontImagePath(string fileGuid, string imageName)
        {
            return Path.Combine(GetFrontImageFolder(fileGuid), imageName);
        }

        private string GetTempFileFolder(string fileGuid)
        {
            return m_tempFolderPath;
        }

        private string GetFileFolder(string fileGuid)
        {
            return Path.Combine(m_path, fileGuid);
        }

        private string GetImageFolder(string fileGuid)
        {
            return Path.Combine(GetFileFolder(fileGuid), ImagesFolderName);
        }

        private string GetFrontImageFolder(string fileGuid)
        {
            return Path.Combine(GetImageFolder(fileGuid), FrontPageDirName);
        }
    }
}