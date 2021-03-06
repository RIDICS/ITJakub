﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Works.Content;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Test.Mock;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class NewResourceVersionTest
    {
        [TestMethod]
        public void TestNewImageVersion()
        {
            var repository = new MockResourceRepository();
            var fileSystemManager = new MockFileSystemManager();
            var resourceId = 1;
            var userId = 2;
            var data = new CreateImageContract
            {
                FileName = "test.jpeg",
                Comment = string.Empty,
                ImageId = resourceId,
                OriginalVersionId = 1,
            };
            fileSystemManager.FileInfo = new SaveResourceResult
            {
                FileNameId = "file-id",
                FileSize = 4096,
            };

            try
            {
                new CreateNewImageResourceVersionWork(repository, fileSystemManager, data, Stream.Null, userId).Execute();
                Assert.Fail("Create new ImageResouce should fail, because version conflict");
            }
            catch (MainServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.Conflict, e.StatusCode);
            }

            var latestImage = repository.GetLatestResourceVersion<ImageResource>(resourceId);
            data.OriginalVersionId = latestImage.Id; // No conflict ID


            var work = new CreateNewImageResourceVersionWork(repository, fileSystemManager, data, Stream.Null, userId);
            work.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(1, repository.UpdatedObjects.Count);
            Assert.AreSame(repository.CreatedObjects.First(), repository.UpdatedObjects.First());

            var newImageResource = repository.CreatedObjects.OfType<ImageResource>().First();

            Assert.AreEqual(latestImage.VersionNumber + 1, newImageResource.VersionNumber);
            Assert.AreEqual("image/jpeg", newImageResource.MimeType);
            Assert.AreEqual(latestImage.Resource.Id, newImageResource.Resource.Id);
            Assert.AreEqual(userId, newImageResource.CreatedByUser.Id);
        }

        [TestMethod]
        public void TestNewAudioVersion()
        {
            var repository = new MockResourceRepository();
            var fileSystemManager = new MockFileSystemManager();
            var resourceId = 2;
            var userId = 3;
            var data = new CreateAudioContract
            {
                FileName = "test-audio.ogg",
                Comment = string.Empty,
                OriginalVersionId = 1,
                ResourceTrackId = 30,
                Duration = new TimeSpan(0, 3, 45)
            };
            fileSystemManager.FileInfo = new SaveResourceResult
            {
                FileNameId = "file-id",
                FileSize = 8192,
            };

            try
            {
                new CreateNewAudioResourceVersionWork(repository, fileSystemManager, resourceId, data, Stream.Null, userId).Execute();
                Assert.Fail("Create new AudioResouce should fail, because version conflict");
            }
            catch (MainServiceException e)
            {
                Assert.AreEqual(HttpStatusCode.Conflict, e.StatusCode);
            }

            var latestAudio = repository.GetLatestResourceVersion<AudioResource>(resourceId);
            data.OriginalVersionId = latestAudio.Id; // No conflict ID


            var work = new CreateNewAudioResourceVersionWork(repository, fileSystemManager, resourceId, data, Stream.Null, userId);
            work.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(1, repository.UpdatedObjects.Count);
            Assert.AreSame(repository.CreatedObjects.First(), repository.UpdatedObjects.First());

            var newAudioResource = repository.CreatedObjects.OfType<AudioResource>().First();

            Assert.AreEqual(latestAudio.VersionNumber + 1, newAudioResource.VersionNumber);
            Assert.AreEqual("audio/ogg", newAudioResource.MimeType);
            Assert.AreEqual(AudioTypeEnum.Ogg, newAudioResource.AudioType);
            Assert.AreEqual(latestAudio.Resource.Id, newAudioResource.Resource.Id);
            Assert.AreEqual(userId, newAudioResource.CreatedByUser.Id);
            Assert.AreEqual(data.ResourceTrackId, newAudioResource.ResourceTrack.Id);
        }
    }
}
