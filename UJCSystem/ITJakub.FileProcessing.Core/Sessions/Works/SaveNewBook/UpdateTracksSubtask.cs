using System;
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

        public void UpdateTracks(long projectId, int userId, string message, BookData bookData)
        {
            if (bookData.Tracks == null)
                return;

            var now = DateTime.UtcNow;
            var project = m_resourceRepository.Load<Project>(projectId);
            var user = m_resourceRepository.Load<User>(userId);

            var dbTracks = m_resourceRepository.GetProjectTracks(projectId);
            var dbAudioList = m_resourceRepository.GetProjectAudioResources(projectId);
            var dbAudioGroups = dbAudioList.GroupBy(x => x.Resource.Id);
            foreach (var track in bookData.Tracks)
            {
                var dbTrack = dbTracks.FirstOrDefault(x => x.Position == track.Position);
                if (dbTrack == null)
                {
                    var newResource = new Resource
                    {
                        Project = project,
                        Name = string.Empty,
                        ContentType = ContentTypeEnum.AudioTrack,
                        ResourceType = ResourceTypeEnum.AudioTrack,
                    };
                    CreateTrackResource(1, newResource, track, user, now);
                }
                else if (IsTrackUpdated(dbTrack, track))
                {
                    //TODO
                }

                foreach (var trackRecordingData in track.Recordings)
                {
                    // TODO
                }
            }

            throw new NotImplementedException();
        }
        
        private bool IsTrackUpdated(TrackResource dbTrack, TrackData trackData)
        {
            return dbTrack.Name != trackData.Name || dbTrack.Text != trackData.Text;
        }

        private void CreateTrackResource(int version, Resource resource, TrackData trackData, User user, DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}