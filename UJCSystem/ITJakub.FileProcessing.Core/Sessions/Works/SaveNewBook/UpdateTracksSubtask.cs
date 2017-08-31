using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateTracksSubtask
    {
        private readonly ResourceRepository m_resourceRepository;

        public UpdateTracksSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public void UpdateTracks(long projectId, int userId, string comment, BookData bookData)
        {
            if (bookData.Tracks == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbTracks = m_resourceRepository.GetProjectTracks(projectId);
            var dbAudioList = m_resourceRepository.GetProjectAudioResources(projectId);
            var dbAudioGroups = dbAudioList.GroupBy(x => x.Resource.Id).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var track in bookData.Tracks)
            {
                var dbTrack = dbTracks.FirstOrDefault(x => x.Position == track.Position);
                if (dbTrack == null)
                {
                    // TODO only create
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = string.Empty,
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

                    m_resourceRepository.Update(dbTrack);
                }

                foreach (var trackRecordingData in track.Recordings)
                {
                    List<AudioResource> dbAudioResources;
                    if (!dbAudioGroups.TryGetValue(dbTrack.Resource.Id, out dbAudioResources))
                    {
                        dbAudioResources = new List<AudioResource>();
                    }
                    
                    var dbAudioResource = dbAudioResources.FirstOrDefault(x => x.FileName == trackRecordingData.FileName);
                    if (dbAudioResource == null)
                    {
                        // TODO create new resource
                        var newDbResource = new Resource
                        {
                            Name = string.Empty,
                            ContentType = ContentTypeEnum.AudioTrack,
                            ResourceType = ResourceTypeEnum.Audio,
                            Project = project,
                        };
                        var newDbAudio = new AudioResource
                        {
                            Resource = newDbResource,
                            //AudioType =  
                            //TODO ...
                        };
                    }
                    else
                    {
                        // TODO always create new version (new file exists)
                    }

                    // TODO check correct LatestVersion updating
                    // TODO remove references in unused recordings
                }
            }

            throw new NotImplementedException();
        }
        
        private bool IsTrackUpdated(TrackResource dbTrack, TrackData trackData)
        {
            return dbTrack.Name != trackData.Name || dbTrack.Text != trackData.Text;
        }
    }
}