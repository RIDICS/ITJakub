using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateTracksSubtask
    {
        private readonly ResourceRepository m_resourceRepository;
        private List<long> m_allImportedResourceVersionIds;
        
        public UpdateTracksSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_allImportedResourceVersionIds = new List<long>();
        }

        public List<long> ImportedResourceVersionIds => m_allImportedResourceVersionIds;

        public void UpdateTracks(long projectId, int userId, string comment, BookData bookData)
        {
            if (bookData.Tracks == null)
                return;

            var importedTrackResourceIds = new List<long>();
            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbTracks = m_resourceRepository.GetProjectTracks(projectId);
            var dbAudioList = m_resourceRepository.GetProjectAudioResources(projectId);
            var dbAudioGroups = dbAudioList.Where(x => x.ResourceTrack != null)
                .GroupBy(x => x.ResourceTrack.Id)
                .ToDictionary(x => x.Key, x => x.ToList());
            foreach (var track in bookData.Tracks)
            {
                var dbTrack = dbTracks.FirstOrDefault(x => x.Name == track.Name);
                if (dbTrack == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = track.Name,
                        ContentType = ContentTypeEnum.AudioTrack,
                        ResourceType = ResourceTypeEnum.AudioTrack,
                    };
                    var newDbTrack = new TrackResource
                    {
                        VersionNumber = 1,
                        Resource = newResource,
                        Comment = comment,
                        CreatedByUser = user,
                        CreateTime = now,
                        Name = track.Name,
                        Text = track.Text,
                        Position = track.Position
                    };
                    newResource.LatestVersion = newDbTrack;
                    
                    m_resourceRepository.Create(newDbTrack);
                    dbTrack = newDbTrack;
                }
                else if (IsTrackUpdated(dbTrack, track))
                {
                    dbTrack.Name = track.Name;
                    dbTrack.Text = track.Text;
                    dbTrack.Position = track.Position;
                    dbTrack.Comment = comment;
                    dbTrack.CreateTime = now;
                    dbTrack.CreatedByUser = user;
                    // Update resource name is not required (TrackResources are distinguish by name)

                    m_resourceRepository.Update(dbTrack);
                    importedTrackResourceIds.Add(dbTrack.Id);
                }
                else
                {
                    importedTrackResourceIds.Add(dbTrack.Id);
                }

                m_allImportedResourceVersionIds.Add(dbTrack.Id);
                UpdateAudioResources(track.Recordings, dbAudioGroups, dbTrack, project, comment, user, now, bookData.FileNameMapping);
            }

            // Update positions of unused tracks
            var unusedDbTracks = dbTracks.Where(x => !importedTrackResourceIds.Contains(x.Id));
            foreach (var unusedDbTrack in unusedDbTracks)
            {
                unusedDbTrack.Position = 0;
                m_resourceRepository.Update(unusedDbTrack);
            }
        }
        
        private bool IsTrackUpdated(TrackResource dbTrack, TrackData trackData)
        {
            return dbTrack.Name != trackData.Name ||
                   dbTrack.Text != trackData.Text ||
                   dbTrack.Position != trackData.Position;
        }

        private void UpdateAudioResources(IList<TrackRecordingData> trackRecordings, Dictionary<long, List<AudioResource>> dbAudioGroups, TrackResource dbTrack, Project project, string comment, User user, DateTime now, Dictionary<string, FileResource> fileNameMapping)
        {
            List<AudioResource> dbAudioResources;
            if (!dbAudioGroups.TryGetValue(dbTrack.Resource.Id, out dbAudioResources))
            {
                dbAudioResources = new List<AudioResource>();
            }

            if (trackRecordings != null)
            {
                foreach (var trackRecordingData in trackRecordings)
                {
                    var dbAudioResource = dbAudioResources.FirstOrDefault(x => x.FileName == trackRecordingData.FileName);
                    if (dbAudioResource == null)
                    {
                        var newDbResource = new Resource
                        {
                            Name = trackRecordingData.FileName,
                            ContentType = ContentTypeEnum.AudioTrack,
                            ResourceType = ResourceTypeEnum.Audio,
                            Project = project,
                        };

                        CreateAudioResource(newDbResource, dbTrack.Resource, 1, trackRecordingData, comment, user, now, fileNameMapping);
                    }
                    else
                    {
                        CreateAudioResource(dbAudioResource.Resource, dbTrack.Resource, dbAudioResource.VersionNumber + 1, trackRecordingData, comment, user, now, fileNameMapping);
                    }
                }
            }
        }

        private void CreateAudioResource(Resource resource, Resource resourceTrack, int version, TrackRecordingData data, string comment, User user, DateTime now, Dictionary<string, FileResource> fileNameMapping)
        {
            fileNameMapping.TryGetValue(data.FileName, out var fileInfo);

            var newDbAudio = new AudioResource
            {
                Resource = resource,
                AudioType = data.AudioType,
                Duration = data.Length,
                MimeType = data.MimeType,
                FileName = data.FileName,
                FileId = fileInfo?.NewNameInStorage,
                VersionNumber = version,
                Comment = comment,
                CreateTime = now,
                CreatedByUser = user,
                ResourceTrack = resourceTrack
            };
            resource.LatestVersion = newDbAudio;
            resource.Name = data.FileName;
            m_resourceRepository.Create(newDbAudio);
            m_allImportedResourceVersionIds.Add(newDbAudio.Id);
        }

        public void UpdateFullBookTracks(long projectId, int userId, string comment, BookData bookData)
        {
            if (bookData.FullBookRecordings == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbFullBookAudioList = m_resourceRepository.GetProjectFullAudioResources(projectId);

            foreach (var fullBookRecording in bookData.FullBookRecordings)
            {
                var dbFullBookAudio = dbFullBookAudioList.FirstOrDefault(x => x.FileName == fullBookRecording.FileName);
                if (dbFullBookAudio == null)
                {
                    var newDbResource = new Resource
                    {
                        Name = string.Empty,
                        ContentType = ContentTypeEnum.FullLiteraryWork,
                        ResourceType = ResourceTypeEnum.Audio,
                        Project = project,
                    };

                    CreateAudioResource(newDbResource, null, 1, fullBookRecording, comment, user, now, bookData.FileNameMapping);
                }
                else
                {
                    CreateAudioResource(dbFullBookAudio.Resource, null, dbFullBookAudio.VersionNumber + 1, fullBookRecording, comment, user, now, bookData.FileNameMapping);
                }
            }
        }
    }
}