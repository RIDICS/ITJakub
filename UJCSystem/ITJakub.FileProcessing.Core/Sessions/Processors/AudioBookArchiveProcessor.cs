using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class AudioBookArchiveProcessor : IResourceProcessor
    {
        private const string MimeType = "application/zip";
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var bookVersionEntity = resourceSessionDirector.GetSessionInfoValue<BookVersion>(SessionInfo.BookVersionEntity);

            if (bookVersionEntity.Tracks == null)
                return;

            //DISCOVER all entites
            var recordings = ExtractAudioRecordingsInfo(bookVersionEntity);

            //GENERATE audioArchive
            PackAllBookAudioArchives(resourceSessionDirector, recordings, bookVersionEntity);
        }

        private static Dictionary<AudioType, List<TrackRecording>> ExtractAudioRecordingsInfo(BookVersion bookVersionEntity)
        {
            var recordings = new Dictionary<AudioType, List<TrackRecording>>();
            foreach (var track in bookVersionEntity.Tracks)
            {
                foreach (var recording in track.Recordings)
                {
                    List<TrackRecording> recordingList;

                    if (!recordings.TryGetValue(recording.AudioType, out recordingList))
                    {
                        recordingList = new List<TrackRecording>();
                        recordings.Add(recording.AudioType, recordingList);
                    }

                    recordingList.Add(recording);
                }
            }
            return recordings;
        }

        private void PackAllBookAudioArchives(ResourceSessionDirector resourceSessionDirector, Dictionary<AudioType, List<TrackRecording>> recordings, BookVersion bookVersionEntity)
        {
            bookVersionEntity.FullBookRecordings = new List<FullBookRecording>();

            foreach (var audioType in recordings.Keys)
            {
                var allBookArchive = GenerateFullBookArchive(recordings[audioType], bookVersionEntity, audioType, resourceSessionDirector);
                resourceSessionDirector.Resources.Add(allBookArchive);
                var entity = new FullBookRecording
                {
                    BookVersion = bookVersionEntity,
                    AudioType = audioType,
                    FileName = allBookArchive.FileName,
                    MimeType = MimeType                    
                };

                bookVersionEntity.FullBookRecordings.Add(entity);
            }
        }

        private Resource GenerateFullBookArchive(List<TrackRecording> recordings, BookVersion bookVersion, AudioType audioType,
            ResourceSessionDirector resourceSessionDirector)
        {
            var archiveName = GetArchiveName(bookVersion, audioType);
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

            return new Resource
            {
                FileName = archiveName,
                FullPath = fullPath,
                ResourceType = ResourceType.Audio,
            };
        }

        private string GetArchiveName(BookVersion bookVersion, AudioType audioType)
        {
            return string.Format("{0}_{1}.zip", bookVersion.VersionId, audioType);
        }
    }
}