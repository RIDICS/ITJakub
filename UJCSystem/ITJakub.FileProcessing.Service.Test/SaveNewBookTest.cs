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

        private Dictionary<string, Term> GetTestTermCache()
        {
            return new List<Term>
            {
                new Term
                {
                    ExternalId = "id-1",
                    Text = "term-1"
                },
                new Term
                {
                    ExternalId = "id-2",
                    Text = "term-2"
                }
            }.ToDictionary(x => x.ExternalId);
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
                        Text = "40r",
                        TermXmlIds = new List<string>
                        {
                            "id-2",
                            "id-1"
                        }
                    }
                }
            };
            
            var subtask = new UpdatePagesSubtask(resourceRepository);
            subtask.UpdatePages(40, 1, "comment", bookData, GetTestTermCache());

            Assert.AreEqual(1, resourceRepository.CreatedObjects.Count);
            Assert.AreEqual(2, resourceRepository.UpdatedObjects.Count);

            var firstPage = resourceRepository.CreatedObjects.Cast<PageResource>().First();
            var secondPage = resourceRepository.UpdatedObjects.Cast<PageResource>().First(x => x.Name == "40r");
            var removedPage = resourceRepository.UpdatedObjects.Cast<PageResource>().First(x => x.Name == "40v");

            Assert.AreEqual(1, firstPage.Position);
            Assert.AreEqual(2, secondPage.Position);
            Assert.AreEqual(0, removedPage.Position);

            // Test term assignment
            Assert.IsNull(firstPage.Terms);
            Assert.AreEqual(2, secondPage.Terms.Count);
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
            subtask.UpdatePages(40, 1, "comment", bookData, GetTestTermCache());

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
            subtask.UpdatePages(41, 2, "upload comment", bookData, GetTestTermCache());

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

        [TestMethod]
        public void TestUpdateChapters()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);

            var contentItem1 = new BookContentItemData
            {
                Text = "Chapter 40",
                ItemOrder = 2,
                Page = new BookPageData
                {
                    Text = "40r",
                    XmlId = "new-xml-40-r"
                },
                SubContentItems = new List<BookContentItemData>()
            };
            var contentItem2 = new BookContentItemData
            {
                Text = "Chapter 41",
                ItemOrder = 1,
                Page = new BookPageData
                {
                    Text = "40v",
                    XmlId = "new-xml-40-v"
                },
                SubContentItems = new List<BookContentItemData>()
            };

            contentItem2.SubContentItems.Add(contentItem1);
            var bookData = new BookData
            {
                BookContentItems = new List<BookContentItemData>
                {
                    contentItem2
                }
            };

            var subtask = new UpdateChaptersSubtask(resourceRepository);
            var pageResources = resourceRepository.GetProjectPages(0).ToList();
            subtask.UpdateChapters(41, 2, "upload", bookData, pageResources);

            var createdChapters = resourceRepository.CreatedObjects.OfType<ChapterResource>().ToList();
            var updatedChapters = resourceRepository.UpdatedObjects.OfType<ChapterResource>().ToList();

            Assert.AreEqual(1, createdChapters.Count);
            Assert.AreEqual(2, updatedChapters.Count);

            var firstChapter = updatedChapters.First(x => x.Name == "Chapter 40");
            var secondChapter = createdChapters.First();
            var deletedChapter = updatedChapters.First(x => x.Name != "Chapter 40");

            Assert.AreEqual(2, firstChapter.Position);
            Assert.AreEqual(1, secondChapter.Position);
            Assert.AreEqual(0, deletedChapter.Position);

            Assert.IsNotNull(firstChapter.ParentResource);
            Assert.IsNull(secondChapter.ParentResource);

            Assert.AreEqual(90, firstChapter.ResourceBeginningPage.Id);
            Assert.AreEqual(80, secondChapter.ResourceBeginningPage.Id);
        }

        [TestMethod]
        public void TestUpdateHeadwords()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);

            var headwordDataList = new List<BookHeadwordData>
            {
                new BookHeadwordData // not exists in DB
                {
                    XmlEntryId = "null",
                    DefaultHeadword = "aaa",
                    SortOrder = "aaa-s",
                    Headword = "aaa"
                },
                new BookHeadwordData // same data in DB
                {
                    XmlEntryId = "id-1",
                    DefaultHeadword = "aaa",
                    SortOrder = "aaa-s",
                    Headword = "aaa"
                },
                new BookHeadwordData
                {
                    XmlEntryId = "id-1",
                    DefaultHeadword = "aaa",
                    SortOrder = "aaa-s",
                    Headword = "bbb"
                },
                new BookHeadwordData // HeadwordItem is different
                {
                    XmlEntryId = "id-2",
                    DefaultHeadword = "ccc",
                    SortOrder = "ccc-s",
                    Headword = "aaa"
                },
                new BookHeadwordData
                {
                    XmlEntryId = "id-2",
                    DefaultHeadword = "ccc",
                    SortOrder = "ccc-s",
                    Headword = "bbb-different"
                },
            };
            
            var bookData = new BookData
            {
                BookHeadwords = headwordDataList
            };

            var subtask = new UpdateHeadwordsSubtask(resourceRepository);
            subtask.UpdateHeadwords(41, 2, "upload", bookData);
            
            var createdHeadwordResources = resourceRepository.CreatedObjects.OfType<HeadwordResource>().ToList();
            var createdHeadwordItems = resourceRepository.CreatedObjects.OfType<HeadwordItem>().ToList();
            
            Assert.AreEqual(0, resourceRepository.UpdatedObjects.Count); // No updates
            Assert.AreEqual(2, createdHeadwordResources.Count);
            Assert.AreEqual(3, createdHeadwordItems.Count);

            var newHeadword = createdHeadwordResources.First(x => x.ExternalId == "null");
            var updatedHeadword = createdHeadwordResources.First(x => x.ExternalId == "id-2");

            Assert.AreEqual(1, newHeadword.VersionNumber);
            Assert.AreEqual(2, updatedHeadword.VersionNumber);
        }

        [TestMethod]
        public void TestUpdateTerms()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);

            var termList = new List<TermData>
            {
                new TermData // create term and assign category
                {
                    XmlId = "null",
                    Position = 0,
                    Text = "new-term",
                    TermCategoryName = "category"
                },
                new TermData // update term and create category
                {
                    XmlId = "id-1",
                    Position = 0,
                    Text = "updated-term",
                    TermCategoryName = "null"
                },
                new TermData // term and category already exists
                {
                    XmlId = "id-2",
                    Position = 0,
                    Text = "term",
                    TermCategoryName = "category"
                }
            };

            var bookData = new BookData
            {
                Terms = termList
            };

            var subtask = new UpdateTermsSubtask(resourceRepository);
            subtask.UpdateTerms(41, bookData);

            var createdTerms = resourceRepository.CreatedObjects.OfType<Term>().ToList();
            var createdCategories = resourceRepository.CreatedObjects.OfType<TermCategory>().ToList();
            var updatedTerms = resourceRepository.UpdatedObjects.OfType<Term>().ToList();
            var updatedCategories = resourceRepository.UpdatedObjects.OfType<TermCategory>().ToList();

            Assert.AreEqual(0, updatedCategories.Count);
            Assert.AreEqual(1, createdCategories.Count);
            Assert.AreEqual(1, updatedTerms.Count);
            Assert.AreEqual(1, createdTerms.Count);

            Assert.AreEqual("null", createdCategories.First().Name);
            Assert.AreEqual("new-term", createdTerms.First().Text);
            Assert.AreEqual("updated-term", updatedTerms.First().Text);
        }

        [TestMethod]
        public void TestUpdateTracks()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);
            var bookData = new BookData
            {
                Tracks = new List<TrackData>
                {
                    new TrackData // update on position 1
                    {
                        Name = "track-8",
                        Position = 1,
                        Recordings = new List<TrackRecordingData>
                        {
                            new TrackRecordingData // update resource to new version
                            {
                                FileName = "file-1.mp3"
                            }
                        }
                    },
                    new TrackData // no update on position 2
                    {
                        Name = "track-2",
                        Position = 2,
                        Recordings = null // update 2 recordings to have no parent
                    },
                    new TrackData // create on position 3
                    {
                        Name = "track-3",
                        Position = 3,
                        Recordings = new List<TrackRecordingData>
                        {
                            new TrackRecordingData // create new recording
                            {
                                FileName = "file-3.mp3"
                            }
                        }
                    }
                }
            };

            var subtask = new UpdateTracksSubtask(resourceRepository);
            subtask.UpdateTracks(40, 1, "comment", bookData);

            var createdTracks = resourceRepository.CreatedObjects.OfType<TrackResource>().ToList();
            var updatedTracks = resourceRepository.UpdatedObjects.OfType<TrackResource>().ToList();
            var createdRecordings = resourceRepository.CreatedObjects.OfType<AudioResource>().ToList();
            var updatedRecordings = resourceRepository.UpdatedObjects.OfType<AudioResource>().ToList();

            Assert.AreEqual(1, createdTracks.Count);
            Assert.AreEqual(1, updatedTracks.Count);

            Assert.AreEqual(0, updatedRecordings.Count);
            Assert.AreEqual(2, createdRecordings.Count);

            var recording1 = createdRecordings.FirstOrDefault(x => x.FileName == "file-1.mp3");
            var recording3 = createdRecordings.FirstOrDefault(x => x.FileName == "file-3.mp3");

            Assert.AreEqual(2, recording1?.VersionNumber);
            Assert.AreEqual(1, recording3?.VersionNumber);
        }

        [TestMethod]
        public void TestUpdateFullBookAudio()
        {
            var unitOfWork = new MockUnitOfWork();
            var resourceRepository = new MockResourceRepository(unitOfWork);
            var bookData = new BookData
            {
                FullBookRecordings = new List<FullBookRecordingData>
                {
                    new FullBookRecordingData // update
                    {
                        FileName = "file-2.mp3"
                    },
                    new FullBookRecordingData // new
                    {
                        FileName = "file-8.mp3"
                    }
                }
            };

            var subtask = new UpdateTracksSubtask(resourceRepository);
            subtask.UpdateFullBookTracks(40, 1, "comment", bookData);

            var createdRecordings = resourceRepository.CreatedObjects.OfType<AudioResource>().ToList();
            var updatedRecordings = resourceRepository.UpdatedObjects.OfType<AudioResource>().ToList();

            Assert.AreEqual(0, updatedRecordings.Count);
            Assert.AreEqual(2, createdRecordings.Count);

            var recording2 = createdRecordings.FirstOrDefault(x => x.FileName == "file-2.mp3");
            var recording8 = createdRecordings.FirstOrDefault(x => x.FileName == "file-8.mp3");

            Assert.AreEqual(2, recording2?.VersionNumber);
            Assert.AreEqual(1, recording8?.VersionNumber);
        }
    }
}
