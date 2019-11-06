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
        private readonly List<long> m_allImportedResourceVersionIds;
        
        public UpdateTracksSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
            m_allImportedResourceVersionIds = new List<long>();
        }

        public List<long> ImportedResourceVersionIds => m_allImportedResourceVersionIds;

        public void UpdateTracks(long projectId, int userId, BookData bookData)
        {
            if (bookData.Tracks == null)
                return;

            var updatedTrackResourceIds = new List<long>();
            var updatedAudioResourceIds = new List<long>();
            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbTracks = m_resourceRepository.GetProjectLatestTracks(projectId);
            var dbAudioList = m_resourceRepository.GetProjectLatestAudioResources(projectId)
                .Where(x => x.ResourceTrack != null)
                .ToList();
            var dbAudioGroups = dbAudioList
                .GroupBy(x => x.ResourceTrack.Id)
                .ToDictionary(x => x.Key, x => x.ToList());
            foreach (var track in bookData.Tracks)
            {
                var dbTrack = dbTracks.FirstOrDefault(x => x.Name == track.Name);

                var newDbTrack = new TrackResource
                {
                    VersionNumber = 0,
                    Resource = null,
                    Comment = string.Empty,
                    CreatedByUser = user,
                    CreateTime = now,
                    Name = track.Name,
                    Text = track.Text,
                    Position = track.Position,
                };

                if (dbTrack == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = track.Name,
                        ContentType = ContentTypeEnum.AudioTrack,
                        ResourceType = ResourceTypeEnum.AudioTrack,
                    };

                    newDbTrack.Resource = newResource;
                    newDbTrack.VersionNumber = 1;
                }
                else
                {
                    newDbTrack.Resource = dbTrack.Resource;
                    newDbTrack.VersionNumber = dbTrack.VersionNumber + 1;

                    updatedTrackResourceIds.Add(dbTrack.Id);
                }

                newDbTrack.Resource.LatestVersion = newDbTrack;

                m_resourceRepository.Create(newDbTrack);

                
                m_allImportedResourceVersionIds.Add(newDbTrack.Id);
                UpdateAudioResources(track.Recordings, dbAudioGroups, newDbTrack, project, user, now, bookData.FileNameMapping, updatedAudioResourceIds);
            }

            // Remove unused resources
            var unusedDbTracks = dbTracks.Where(x => !updatedTrackResourceIds.Contains(x.Id));
            RemoveResourceVersions(unusedDbTracks);
            
            var unusedDbAudioList = dbAudioList.Where(x => !updatedAudioResourceIds.Contains(x.Id));
            RemoveResourceVersions(unusedDbAudioList);
        }

        private void RemoveResourceVersions(IEnumerable<ResourceVersion> resourceVersions)
        {
            foreach (var unusedResourceVersion in resourceVersions)
            {
                var resourceToRemove = unusedResourceVersion.Resource;
                resourceToRemove.IsRemoved = true;
                m_resourceRepository.Update(resourceToRemove);
            }
        }
        
        private bool IsTrackUpdated(TrackResource dbTrack, TrackData trackData)
        {
            return dbTrack.Name != trackData.Name ||
                   dbTrack.Text != trackData.Text ||
                   dbTrack.Position != trackData.Position;
        }

        private void UpdateAudioResources(IList<TrackRecordingData> trackRecordings, Dictionary<long, List<AudioResource>> dbAudioGroups,
            TrackResource dbTrack, Project project, User user, DateTime now, Dictionary<string, FileResource> fileNameMapping,
            List<long> updatedAudioResourceIds)
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

                        CreateAudioResource(newDbResource, dbTrack.Resource, 1, trackRecordingData, user, now, fileNameMapping);
                    }
                    else
                    {
                        CreateAudioResource(dbAudioResource.Resource, dbTrack.Resource, dbAudioResource.VersionNumber + 1, trackRecordingData, user, now, fileNameMapping);
                        updatedAudioResourceIds.Add(dbAudioResource.Id);
                    }
                }
            }
        }

        private void CreateAudioResource(Resource resource, Resource resourceTrack, int version, TrackRecordingData data, User user, DateTime now, Dictionary<string, FileResource> fileNameMapping)
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
                Comment = string.Empty,
                CreateTime = now,
                CreatedByUser = user,
                ResourceTrack = resourceTrack
            };
            resource.LatestVersion = newDbAudio;
            resource.Name = data.FileName;
            m_resourceRepository.Create(newDbAudio);
            m_allImportedResourceVersionIds.Add(newDbAudio.Id);
        }

        public void UpdateFullBookTracks(long projectId, int userId, BookData bookData)
        {
            if (bookData.FullBookRecordings == null)
                return;

            var updatedAudioResourceIds = new List<long>();
            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbFullBookAudioList = m_resourceRepository.GetProjectLatestFullAudioResources(projectId);

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

                    CreateAudioResource(newDbResource, null, 1, fullBookRecording, user, now, bookData.FileNameMapping);
                }
                else
                {
                    CreateAudioResource(dbFullBookAudio.Resource, null, dbFullBookAudio.VersionNumber + 1, fullBookRecording, user, now, bookData.FileNameMapping);
                    updatedAudioResourceIds.Add(dbFullBookAudio.Id);
                }
            }

            var unusedDbAudioList = dbFullBookAudioList.Where(x => !updatedAudioResourceIds.Contains(x.Id));
            RemoveResourceVersions(unusedDbAudioList);
        }
    }
}