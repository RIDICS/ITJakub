using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AdvLib
{
    /// <summary>
    /// Class that represents ADV (archive od document views) file.
    /// </summary>
    public class AdvFile
    {
        static private readonly string META_FILENAME = "meta.xml";

        Dictionary<string, byte[]> fileMap = new Dictionary<string, byte[]>();

        /// <summary>
        /// Default constructor that creates empty ADV file structure.
        /// </summary>
        public AdvFile()
        {
        }

        /// <summary>
        /// Puts a file in the archive. Rewrites existing file if already exists in the archive.
        /// </summary>
        /// <param name="name">name of the file in the archive</param>
        /// <param name="content">content of the file</param>
        public void PutFile(string name, byte[] content)
        {
            fileMap[name] = content;
        }

        /// <summary>
        /// Puts a metadata file in the archive. Rewrites existing metadata file if already exists in the archive.
        /// </summary>
        /// <param name="content">content of the meta file</param>
        public void PutMetaFile(byte[] content)
        {
            PutFile(META_FILENAME, content);
        }
        /// <summary>
        /// Deletes a file from the ADV archive.
        /// </summary>
        /// <param name="name">Name of file to be deleted</param>
        public void DeleteFile(string name)
        {
            if (!HasFile(name))
                throw new FileNotFoundException("File with name <" + name + "> cannot be found in the ADV archive.");
            fileMap.Remove(name);
        }

        /// <summary>
        /// Deletes a file from the ADV archive.
        /// </summary>
        public void DeleteMetaFile()
        {
            DeleteFile(META_FILENAME);
        }

        /// <summary>
        /// Checks whether specified file is present within the archive.
        /// </summary>
        /// <param name="name">Name of the file to be checked</param>
        /// <returns>True if file with specified name is present in the archive, otherwise false</returns>
        public bool HasFile(string name)
        {
            return fileMap.ContainsKey(name);
        }

        /// <summary>
        /// Returns a file content of the specified file.
        /// </summary>
        /// <param name="name">Name of file which content should be returned</param>
        /// <returns></returns>
        public byte[] GetFile(string name)
        {
            if (!HasFile(name))
                throw new FileNotFoundException("File with name <" + name + "> cannot be found in the ADV archive.");
            return fileMap[name];
        }

        /// <summary>
        /// Returns a meta file content.
        /// </summary>
        /// <returns>meta file content</returns>
        public byte[] GetMetaFile()
        {
            return GetFile(META_FILENAME);
        }

        /// <summary>
        /// Returns present file names in the ADV archive.
        /// </summary>
        /// <returns></returns>
        public string[] GetFileNames()
        {
            string[] result = new string[fileMap.Count];
            fileMap.Keys.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        /// Validates the current ADV archive if satisfies the standard.
        /// </summary>
        /// <returns>True if ADV archive is correct, otherwise false</returns>
        public bool Validate()
        {
            string[] filenames = GetFileNames();
            if (filenames == null || filenames.Length == 0)
            {
                return false;
            }
            return true;
        }

    }
}
