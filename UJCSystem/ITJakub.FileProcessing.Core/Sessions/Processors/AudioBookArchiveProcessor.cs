using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using ITJakub.FileProcessing.Core.Data;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class AudioBookArchiveProcessor : IResourceProcessor
    {
        private const string MimeType = "application/zip";
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var bookData = resourceSessionDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);

            if (bookData.Tracks == null)
                return;

            //DISCOVER all entites
            var recordings = ExtractAudioRecordingsInfo(bookData);

            //GENERATE audioArchive
            PackAllBookAudioArchives(resourceSessionDirector, recordings, bookData);
        }

        private static Dictionary<AudioTypeEnum, List<TrackRecordingData>> ExtractAudioRecordingsInfo(BookData bookData)
        {
            var recordings = new Dictionary<AudioTypeEnum, List<TrackRecordingData>>();
            foreach (var track in bookData.Tracks)
            {
                foreach (var recording in track.Recordings)
                {
                    List<TrackRecordingData> recordingList;

                    if (!recordings.TryGetValue(recording.AudioType, out recordingList))
                    {
                        recordingList = new List<TrackRecordingData>();
                        recordings.Add(recording.AudioType, recordingList);
                    }

                    recordingList.Add(recording);
                }
            }
            return recordings;
        }

        private void PackAllBookAudioArchives(ResourceSessionDirector resourceSessionDirector, Dictionary<AudioTypeEnum, List<TrackRecordingData>> recordings, BookData bookData)
        {
            bookData.FullBookRecordings = new List<FullBookRecordingData>();

            foreach (var audioType in recordings.Keys)
            {
                var allBookArchive = GenerateFullBookArchive(recordings[audioType], bookData, audioType, resourceSessionDirector);
                resourceSessionDirector.Resources.Add(allBookArchive);
                var entity = new FullBookRecordingData
                {
                    AudioType = audioType,
                    FileName = allBookArchive.FileName,
                    MimeType = MimeType                    
                };

                bookData.FullBookRecordings.Add(entity);
            }
        }

        private FileResource GenerateFullBookArchive(List<TrackRecordingData> recordings, BookData bookData, AudioTypeEnum audioType,
            ResourceSessionDirector resourceSessionDirector)
        {
            var archiveName = GetArchiveName(bookData, audioType);
            var fullPath = Path.Combine(resourceSessionDirector.SessionPath, archiveName);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Create))
                {
                    foreach (var record in recordings)
                    {
                        var sessionResource = resourceSessionDirector.GetResourceFromSession(ResourceType.Audio, record.FileName);
                        if (sessionResource != null)
                        {
                            zipFile.CreateEntryFromFile(sessionResource.FullPath, record.FileName, CompressionLevel.Optimal);
                        }
                        else
                        {
                            if (m_log.IsErrorEnabled)
                                m_log.ErrorFormat($"Found metadata info about audioFileRecord : {record.FileName} but file was not uploaded to session.");
                        }
                        
                    }
                }
            }

            return new FileResource
            {
                FileName = archiveName,
                FullPath = fullPath,
                ResourceType = ResourceType.Audio,
            };
        }

        private string GetArchiveName(BookData bookData, AudioTypeEnum audioType)
        {
            return string.Format("{0}_{1}.zip", bookData.VersionXmlId, audioType);
        }
    }
}