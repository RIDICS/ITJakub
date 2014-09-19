using System;
using System.IO;

namespace ITJakub.ITJakubService.Core
{
    public class LocalFilesystemManager
    {
        private const string FrontPageDirName = "FrontPage";
        private readonly string m_imagesFolderPath;
        private readonly string m_tempFolderPath;

        public LocalFilesystemManager(string tempFolderPath, string imagesFolderPath)
        {
            m_tempFolderPath = tempFolderPath;
            MakeDirectoriesIfNotExist(m_tempFolderPath);
            m_imagesFolderPath = imagesFolderPath;
            MakeDirectoriesIfNotExist(m_imagesFolderPath);
        }

        private void MakeDirectoriesIfNotExist(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        private void SaveFile(string path, Stream data)
        {
            string parentDirName = Path.GetDirectoryName(path);
            if (parentDirName != null)
                MakeDirectoriesIfNotExist(parentDirName);
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

        private string GetTempFilePath(string fileName)
        {
            return Path.Combine(m_tempFolderPath, fileName);
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

        private string GetImagePath(string fileGuid, string imageName)
        {
            return Path.Combine(m_imagesFolderPath, fileGuid, imageName);
        }

        private string GetFrontImagePath(string fileGuid, string imageName)
        {
            return Path.Combine(m_imagesFolderPath, fileGuid, FrontPageDirName, imageName);
        }
    }
}