using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook;
using ITJakub.FileProcessing.Service.Test.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Service.Test
{
    [TestClass]
    public class SaveNewBookTest
    {
        [TestMethod]
        public void TestUpdateProject()
        {
            var unitOfWork = new MockUnitOfWork();
            var bookData = new BookData
            {
                BookXmlId = "external-id",
                Title = "title"
            };


            var projectRepository = new MockProjectRepository(unitOfWork);
            var subtask = new UpdateProjectSubtask(projectRepository);

            long? projectId = 12;
            subtask.UpdateProject(projectId, 1, bookData);

            Assert.AreEqual(1, projectRepository.UpdatedObjects.Count);
            Assert.AreEqual(0, projectRepository.CreatedObjects.Count);

            var updatedProject = (Project) projectRepository.UpdatedObjects.First();
            Assert.AreEqual(bookData.BookXmlId, updatedProject.ExternalId);



            projectRepository = new MockProjectRepository(unitOfWork) {CanFindProjectByExternalId = true};
            subtask = new UpdateProjectSubtask(projectRepository);

            var dbProjectId = subtask.UpdateProject(null, 1, bookData);

            Assert.AreEqual(0, projectRepository.CreatedObjects.Count);
            Assert.AreEqual(0, projectRepository.UpdatedObjects.Count);
            Assert.AreEqual(MockProjectRepository.GetProjectIdValue, dbProjectId);



            projectRepository = new MockProjectRepository(unitOfWork) {CanFindProjectByExternalId = false};
            subtask = new UpdateProjectSubtask(projectRepository);

            subtask.UpdateProject(null, 1, bookData);

            Assert.AreEqual(1, projectRepository.CreatedObjects.Count);
            Assert.AreEqual(0, projectRepository.UpdatedObjects.Count);

            var createdProject = (Project)projectRepository.CreatedObjects.First();
            Assert.AreEqual(bookData.BookXmlId, createdProject.ExternalId);
        }

        [TestMethod]
        public void TestAuthorConverter()
        {
            var authorName1 = "Aaa Bbb";
            var authorName2 = "Bbb, Aaa";
            var authorName3 = "Aaa Bbb Ccc";

            var author1 = PersonHelper.ConvertToEntity(authorName1);
            var author2 = PersonHelper.ConvertToEntity(authorName2);
            var author3 = PersonHelper.ConvertToEntity(authorName3);

            Assert.AreEqual("Bbb", author1.LastName);
            Assert.AreEqual("Aaa", author1.FirstName);
            Assert.AreEqual("Bbb", author2.LastName);
            Assert.AreEqual("Aaa", author2.FirstName);
            Assert.AreEqual("Aaa Bbb Ccc", author3.LastName);
            Assert.AreEqual("", author3.FirstName);
        }

        [TestMethod]
        public void TestUpdateAuthors()
        {
            var unitOfWork = new MockUnitOfWork();
            var bookData = new BookData
            {
                Authors = new List<AuthorData>
                {
                    new AuthorData {Name = "Aaa Bbb"},
                    new AuthorData {Name = "Ccc Ddd"}
                }
            };


            var metadataRepository = new MockMetadataRepository(unitOfWork)
            {
                CanFindAuthorByName = true,
                ProjectOriginalAuthors = new List<ProjectOriginalAuthor>
                {
                    new ProjectOriginalAuthor
                    {
                        OriginalAuthor = new OriginalAuthor {FirstName = "Eee", LastName = "Fff", Id = 30},
                        Sequence = 1
                    },
                    new ProjectOriginalAuthor
                    {
                        OriginalAuthor = new OriginalAuthor {FirstName = "Aaa", LastName = "Bbb", Id = 10},
                        Sequence = 2
                    }
                }
            };
            var subtask = new UpdateAuthorsSubtask(metadataRepository);

            subtask.UpdateAuthors(41, bookData);

            Assert.AreEqual(1, metadataRepository.CreatedObjects.Count);
            Assert.AreEqual(1, metadataRepository.UpdatedObjects.Count);
            Assert.AreEqual(1, metadataRepository.DeletedObjects.Count);

            var createdItem = (ProjectOriginalAuthor) metadataRepository.CreatedObjects.Single();
            var updatedItem = (ProjectOriginalAuthor) metadataRepository.UpdatedObjects.Single();
            Assert.AreEqual(1, updatedItem.Sequence);
            Assert.AreEqual(2, createdItem.Sequence);
        }

        [TestMethod]
        public void TestUpdateMetadata()
        {
            var unitOfWork = new MockUnitOfWork();
            var bookData = new BookData
            {
                BookXmlId = "external-id",
                Title = "title",
                SourceAbbreviation = "t",
                PublishPlace = "Praha",
                ManuscriptDescriptions = new List<ManuscriptDescriptionData>(),
                Publisher = new PublisherData()
            };


            var metadataRepository = new MockMetadataRepository(unitOfWork)
            {
                CanGetLatestMetadata = true,
                LatestMetadataVersion = 29
            };
            var subtask = new UpdateMetadataSubtask(metadataRepository);

            subtask.UpdateMetadata(40, 1, "comment", bookData);

            var createdMetadata = (MetadataResource) metadataRepository.CreatedObjects.Single();
            Assert.AreEqual(30, createdMetadata.VersionNumber);
            Assert.AreEqual(ContentTypeEnum.None, createdMetadata.Resource.ContentType);
            Assert.AreEqual(ResourceTypeEnum.ProjectMetadata, createdMetadata.Resource.ResourceType);
            Assert.AreEqual(createdMetadata, createdMetadata.Resource.LatestVersion);
            Assert.IsNull(bookData.SubTitle);
            Assert.AreEqual(bookData.Title, createdMetadata.Title);
            Assert.AreEqual(bookData.SourceAbbreviation, createdMetadata.SourceAbbreviation);
            Assert.AreEqual(bookData.PublishPlace, createdMetadata.PublishPlace);


            metadataRepository = new MockMetadataRepository(unitOfWork)
            {
                CanGetLatestMetadata = false
            };
            subtask = new UpdateMetadataSubtask(metadataRepository);

            subtask.UpdateMetadata(40, 1, "comment", bookData);

            createdMetadata = (MetadataResource)metadataRepository.CreatedObjects.Single();
            Assert.AreEqual(1, createdMetadata.VersionNumber);
            Assert.AreEqual(ContentTypeEnum.None, createdMetadata.Resource.ContentType);
            Assert.AreEqual(ResourceTypeEnum.ProjectMetadata, createdMetadata.Resource.ResourceType);
            Assert.AreEqual(createdMetadata, createdMetadata.Resource.LatestVersion);
            Assert.IsNull(bookData.SubTitle);
            Assert.AreEqual(bookData.Title, createdMetadata.Title);
            Assert.AreEqual(bookData.SourceAbbreviation, createdMetadata.SourceAbbreviation);
            Assert.AreEqual(bookData.PublishPlace, createdMetadata.PublishPlace);
        }

        [TestMethod]
        public void TestUpdatePages()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);
            var bookData = new BookData
            {
                Pages = new List<BookPageData>
                {
                    new BookPageData
                    {
                        Text = "39v"
                    },
                    new BookPageData
                    {
                        Text = "40r"
                    }
                }
            };
            
            var subtask = new UpdatePagesSubtask(resourceRepository);
            subtask.UpdatePages(40, 1, "comment", bookData);

            Assert.AreEqual(1, resourceRepository.CreatedObjects.Count);
            Assert.AreEqual(2, resourceRepository.UpdatedObjects.Count);

            var firstPage = resourceRepository.CreatedObjects.Cast<PageResource>().First();
            var secondPage = resourceRepository.UpdatedObjects.Cast<PageResource>().First(x => x.Name == "40r");
            var removedPage = resourceRepository.UpdatedObjects.Cast<PageResource>().First(x => x.Name == "40v");

            Assert.AreEqual(1, firstPage.Position);
            Assert.AreEqual(2, secondPage.Position);
            Assert.AreEqual(0, removedPage.Position);
        }

        [TestMethod]
        public void TestUpdatePageTexts()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);
            var bookData = new BookData
            {
                Pages = new List<BookPageData>
                {
                    new BookPageData
                    {
                        Text = "39v",
                        XmlId = "new-xml-39-v"
                    },
                    new BookPageData
                    {
                        Text = "40r",
                        XmlId = "new-xml-40-r"
                    }
                }
            };

            var subtask = new UpdatePagesSubtask(resourceRepository);
            subtask.UpdatePages(40, 1, "comment", bookData);

            var createdTexts = resourceRepository.CreatedObjects.OfType<TextResource>().ToList();
            var updatedTexts = resourceRepository.UpdatedObjects.OfType<TextResource>().ToList();

            Assert.AreEqual(2, createdTexts.Count);
            Assert.AreEqual(0, updatedTexts.Count);

            var firstText = createdTexts.First(x => x.ExternalId == "new-xml-39-v");
            var secondText = createdTexts.First(x => x.ExternalId == "new-xml-40-r");

            Assert.AreEqual(1, firstText.VersionNumber);
            Assert.AreEqual(2, secondText.VersionNumber);
            Assert.AreEqual(900, secondText.Resource.Id);
        }

        [TestMethod]
        public void TestUpdatePageImages()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);
            var bookData = new BookData
            {
                Pages = new List<BookPageData>
                {
                    new BookPageData
                    {
                        Text = "39v",
                        Image = "image_39v.jpg"
                    },
                    new BookPageData
                    {
                        Text = "40r",
                        Image = "image_40r.jpg"
                    }
                }
            };

            var subtask = new UpdatePagesSubtask(resourceRepository);
            subtask.UpdatePages(41, 2, "upload comment", bookData);

            var createdImages = resourceRepository.CreatedObjects.OfType<ImageResource>().ToList();
            var updatedImages = resourceRepository.UpdatedObjects.OfType<ImageResource>().ToList();

            Assert.AreEqual(2, createdImages.Count);
            Assert.AreEqual(0, updatedImages.Count);

            var firstImage = createdImages.First(x => x.FileName == "image_39v.jpg");
            var secondImage = createdImages.First(x => x.FileName == "image_40r.jpg");

            Assert.AreEqual(1, firstImage.VersionNumber);
            Assert.AreEqual(2, secondImage.VersionNumber);
            Assert.AreEqual(900, secondImage.Resource.Id);
        }
    }
}
