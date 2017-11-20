using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectAudio : BaseController
    {
        private readonly ProjectContentManager m_projectContentManager;

        public ProjectAudio(ProjectContentManager projectContentManager)
        {
            m_projectContentManager = projectContentManager;
        }

        [HttpGet("{projectId}/track")]
        public List<TrackContract> GetTrackList(long projectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("track/{trackId}")]
        public TrackContract GetTrackResource(long trackId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{projectId}/track")]
        public IActionResult CreateTrackResource(long projectId, [FromBody] CreateTrackContract trackData)
        {
            throw new NotImplementedException();
        }

        [HttpPut("track/{trackId}")]
        public IActionResult UpdateTrackResource(long trackId, [FromBody] CreateTrackContract trackData)
        {
            throw new NotImplementedException();
        }

        [HttpPost("audio/{audioId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateAudioResourceVersion(long audioId, [FromQuery] string fileName, [FromQuery] long? originalVersionId, [FromQuery] long? trackId, [FromQuery] TimeSpan? duration, [FromQuery] string comment)
        {
            if (fileName == null || originalVersionId == null || trackId == null || duration == null)
            {
                return BadRequest("Missing required parameters");
            }

            var stream = Request.Body;
            var contract = new CreateAudioContract
            {
                Comment = comment,
                FileName = fileName,
                OriginalVersionId = originalVersionId.Value,
                ResourceTrackId = trackId.Value,
                Duration = duration,
            };

            var resultVersionId = m_projectContentManager.CreateNewAudioVersion(audioId, contract, stream);
            return Ok(resultVersionId);
        }
    }
}
