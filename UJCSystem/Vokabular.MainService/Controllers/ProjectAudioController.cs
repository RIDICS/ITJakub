﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectAudioController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;
        private readonly ProjectContentManager m_projectContentManager;

        public ProjectAudioController(ProjectItemManager projectItemManager, ProjectContentManager projectContentManager)
        {
            m_projectItemManager = projectItemManager;
            m_projectContentManager = projectContentManager;
        }

        [HttpGet("{projectId}/track")]
        public List<TrackContract> GetTrackList(long projectId)
        {
            var result = m_projectItemManager.GetTrackList(projectId);
            return result;
        }

        [HttpGet("track/{trackId}")]
        public TrackContract GetTrackResource(long trackId)
        {
            var result = m_projectItemManager.GetTrackResource(trackId);
            return result;
        }

        [HttpGet("track/{trackId}/recordings")]
        public IList<AudioContract> GetTrackRecordings(long trackId)
        {
            var result = m_projectItemManager.GetTrackRecordings(trackId);
            return result;
        }

        [HttpGet("audio/{audioId}")]
        public IActionResult GetAudio(long audioId)
        {
            var result = m_projectItemManager.GetAudio(audioId);
            if (result == null)
                return NotFound();

            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }

        [HttpPost("{projectId}/track")]
        public IActionResult CreateTrackResource(long projectId, [FromBody] CreateTrackContract trackData)
        {
            var resourceId = m_projectItemManager.CreateTrackResource(projectId, trackData);
            return Ok(resourceId);
        }

        [HttpPut("track/{trackId}")]
        public IActionResult UpdateTrackResource(long trackId, [FromBody] CreateTrackContract trackData)
        {
            m_projectItemManager.UpdateTrackResource(trackId, trackData);
            return Ok();
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