using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using ITJakub.Core.Resources;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExtractableArchiveProcessor : IResourceProcessor
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);      
          
        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var archives = resourceSessionDirector.Resources.Where(x => x.ResourceType == ResourceType.ExtractableArchive);
            foreach (var archive in archives.ToList())
            {                
               var filesFromArchive = ExtractFilesFromArchive(archive, resourceSessionDirector);
                foreach (var extractedFile in filesFromArchive)
                {
                    resourceSessionDirector.AddResource(extractedFile);
                }
            }            
        }

        private List<Resource> ExtractFilesFromArchive(Resource extractableArchive, ResourceSessionDirector resourceSessionDirector)
        {
            var result = new List<Resource>();
            using (ZipArchive archive = ZipFile.OpenRead(extractableArchive.FullPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    var fileName = entry.Name;
                    var fullPath = resourceSessionDirector.GetFullPathForNewSessionResource(fileName);

                    try
                    {
                        if (m_log.IsDebugEnabled)
                            m_log.Debug($"Extracting file '{fileName}' from archive '{extractableArchive.FullPath}'");

                        entry.ExtractToFile(fullPath, false);
                        result.Add(new Resource
                        {
                            FileName = fileName,
                            FullPath = fullPath
                        });

                    }
                    catch (InvalidDataException ex)
                    {
                        if (m_log.IsErrorEnabled)
                            m_log.ErrorFormat("Cannot extract resource: '{0}' from archive '{1}'. Exception: '{2}'", fileName, extractableArchive.FullPath, ex.Message);
                    }

                }
            }

            return result;
        }
    }

    public class InvalidXsltTransformationException : Exception
    {
        public InvalidXsltTransformationException(string message) : base(message)
        {
        }
    }

}