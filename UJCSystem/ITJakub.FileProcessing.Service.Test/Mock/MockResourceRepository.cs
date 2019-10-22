﻿using System;
using System.Collections;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace ITJakub.FileProcessing.Service.Test.Mock
{
    public class MockResourceRepository : ResourceRepository
    {
        public const long HeadwordBookVersionId = 3;

        public MockResourceRepository(UnitOfWorkProvider unitOfWorkProvider) : base(unitOfWorkProvider)
        {
            CreatedObjects = new List<object>();
            UpdatedObjects = new List<object>();
        }

        public List<object> CreatedObjects { get; }
        public List<object> UpdatedObjects { get; }

        public override object FindById(Type type, object id)
        {
            throw new NotSupportedException();
        }

        public override T FindById<T>(object id)
        {
            throw new NotSupportedException();
        }

        public override object Load(Type type, object id)
        {
            switch (type.Name)
            {
                case "Project":
                    return new Project
                    {
                        Id = (long)id
                    };
                case "User":
                    return new User
                    {
                        Id = (int)id
                    };
                case "BookVersionResource":
                    return new BookVersionResource
                    {
                        Id = (long) id
                    };
                case "Term":
                    return new Term
                    {
                        Id = (int) id
                    };
                case "TermCategory":
                    return new TermCategory
                    {
                        Id = (int) id
                    };
                default:
                    throw new NotSupportedException($"Missing mock for type: {type.Name}");
            }
        }

        public override T Load<T>(object id)
        {
            return (T) Load(typeof(T), id);
        }

        public override object Create(object instance)
        {
            CreatedObjects.Add(instance);
            return 446L;
        }

        public override IList<object> CreateAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override void Delete(object instance)
        {
            throw new NotSupportedException();
        }

        public override void Update(object instance)
        {
            UpdatedObjects.Add(instance);
        }

        public override void DeleteAll(Type type)
        {
            throw new NotSupportedException();
        }

        public override void DeleteAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override void Save(object instance)
        {
            throw new NotSupportedException();
        }

        public override void SaveAll(IEnumerable data)
        {
            throw new NotSupportedException();
        }

        public override BookVersionResource GetLatestBookVersion(long projectId)
        {
            if (projectId == 0)
                return null;

            return new BookVersionResource
            {
                ExternalId = "id-1",
                VersionNumber = 1,
                Resource = new Resource
                {
                    Id = 1,
                    Project = new Project
                    {
                        Id = projectId
                    }
                }
            };
        }

        public override IList<PageResource> GetProjectLatestPages(long projectId)
        {
            var project = new Project
            {
                Id = projectId,
            };
            return new List<PageResource>
            {
                new PageResource
                {
                    Id = 901,
                    Resource = new Resource
                    {
                        Project = project,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page,
                        Id = 90,
                    },
                    Name = "40r",
                    Position = 1,
                    VersionNumber = 1
                },
                new PageResource
                {
                    Id = 902,
                    Resource = new Resource
                    {
                        Project = project,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Page,
                        Id = 80,
                    },
                    Name = "40v",
                    Position = 2,
                    VersionNumber = 1
                },
            };
        }

        public IList<TextResource> GetProjectLatestTexts(long projectId, long? namedResourceGroupId, bool fetchParentPage)
        {
            return new List<TextResource>
            {
                new TextResource
                {
                    Resource = new Resource
                    {
                        Id = 900,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Text
                    },
                    ResourcePage = new Resource {Id = 90},
                    ExternalId = "xml-40-r",
                    VersionNumber = 1
                },
                new TextResource
                {
                    Resource = new Resource
                    {
                        Id = 800,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Text
                    },
                    ResourcePage = new Resource {Id = 80},
                    ExternalId = "xml-40-v",
                    VersionNumber = 1
                },
                new TextResource
                {
                    Resource = new Resource
                    {
                        Id = 700,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Text
                    },
                    ResourcePage = new Resource {Id = 90},
                    ExternalId = "xml-copy",
                    VersionNumber = 1
                }
            };
        }

        public override IList<ImageResource> GetProjectLatestImages(long projectId, long? namedResourceGroupId, bool fetchParentPage)
        {
            return new List<ImageResource>
            {
                new ImageResource
                {
                    Resource = new Resource
                    {
                        Id = 900,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Image
                    },
                    ResourcePage = new Resource {Id = 90},
                    FileName = "image_40r.jpg",
                    VersionNumber = 1
                },
                new ImageResource
                {
                    Resource = new Resource
                    {
                        Id = 800,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Image
                    },
                    ResourcePage = new Resource {Id = 80},
                    FileName = "image_40v.jpg",
                    VersionNumber = 1
                },
                new ImageResource
                {
                    Resource = new Resource
                    {
                        Id = 700,
                        ContentType = ContentTypeEnum.Page,
                        ResourceType = ResourceTypeEnum.Image
                    },
                    ResourcePage = new Resource {Id = 90},
                    FileName = "image_copy.jpg",
                    VersionNumber = 1
                }
            };
        }

        public override NamedResourceGroup GetNamedResourceGroup(long projectId, string name, TextTypeEnum textType)
        {
            return new NamedResourceGroup
            {
                Name = name,
                Project = new Project {Id = projectId},
                TextType = textType
            };
        }

        public override IList<ChapterResource> GetProjectLatestChapters(long projectId)
        {
            return new List<ChapterResource>
            {
                new ChapterResource
                {
                    Id = 1051,
                    Resource = new Resource
                    {
                        Id = 900,
                        ContentType = ContentTypeEnum.Chapter,
                        ResourceType = ResourceTypeEnum.Chapter
                    },
                    ResourceBeginningPage = new Resource {Id = 90},
                    Name = "Chapter 40",
                    Position = 1,
                    VersionNumber = 1
                },
                new ChapterResource
                {
                    Id = 1052,
                    Resource = new Resource
                    {
                        Id = 800,
                        ContentType = ContentTypeEnum.Chapter,
                        ResourceType = ResourceTypeEnum.Chapter
                    },
                    ResourceBeginningPage = new Resource {Id = 80},
                    Name = "Chapter 45",
                    Position = 2,
                    VersionNumber = 1
                }
            };
        }

        public override Term GetTermByExternalId(string externalId)
        {
            if (externalId == "null")
                return null;

            return new Term
            {
                ExternalId = externalId,
                Text = "term",
                Position = 0,
                TermCategory = new TermCategory
                {
                    Name = "category"
                }
            };
        }

        public override TermCategory GetTermCategoryByName(string termCategoryName)
        {
            if (termCategoryName == "null")
                return null;

            return new TermCategory
            {
                Name = termCategoryName
            };
        }

        public override HeadwordResource GetLatestHeadword(long projectId, string externalId)
        {
            if (externalId == "null")
                return null;

            return new HeadwordResource
            {
                HeadwordItems = new List<HeadwordItem>
                {
                    new HeadwordItem
                    {
                        Headword = "aaa",
                        HeadwordOriginal = "aaa-o"
                    },
                    new HeadwordItem
                    {
                        Headword = "bbb",
                        HeadwordOriginal = "bbb-o"
                    }
                },
                DefaultHeadword = externalId == "id-1" ? "aaa" : "ccc",
                ExternalId = externalId,
                Resource = new Resource(),
                Sorting = externalId == "id-1" ? "aaa-s" : "ccc-s",
                VersionNumber = 1,
                Id = 100,
                BookVersion = new BookVersionResource
                {
                    Id = HeadwordBookVersionId
                }
            };
        }

        public override IList<HeadwordResource> GetProjectLatestHeadwordPage(long projectId, int start, int count)
        {
            if (start > 0)
                return new List<HeadwordResource>();

            return new List<HeadwordResource>
            {
                new HeadwordResource
                {
                    HeadwordItems = new List<HeadwordItem>
                    {
                        new HeadwordItem
                        {
                            Headword = "aaa",
                            HeadwordOriginal = "aaa-o"
                        },
                        new HeadwordItem
                        {
                            Headword = "bbb",
                            HeadwordOriginal = "bbb-o"
                        }
                    },
                    DefaultHeadword = "aaa",
                    ExternalId = "id-1",
                    Resource = new Resource(),
                    Sorting = "aaa-s",
                    VersionNumber = 1,
                    Id = 100,
                    BookVersion = new BookVersionResource
                    {
                        Id = HeadwordBookVersionId
                    }
                },
                new HeadwordResource
                {
                    HeadwordItems = new List<HeadwordItem>
                    {
                        new HeadwordItem
                        {
                            Headword = "aaa",
                            HeadwordOriginal = "aaa-o"
                        },
                        new HeadwordItem
                        {
                            Headword = "bbb",
                            HeadwordOriginal = "bbb-o"
                        }
                    },
                    DefaultHeadword = "ccc",
                    ExternalId = "id-2",
                    Resource = new Resource(),
                    Sorting = "ccc-s",
                    VersionNumber = 1,
                    Id = 102,
                    BookVersion = new BookVersionResource
                    {
                        Id = HeadwordBookVersionId
                    }
                },
                new HeadwordResource // for removing
                {
                    HeadwordItems = new List<HeadwordItem>(),
                    DefaultHeadword = "eee",
                    ExternalId = "id-REMOVE",
                    Resource = new Resource(),
                    Sorting = "eee-s",
                    VersionNumber = 1,
                    Id = 103,
                    BookVersion = new BookVersionResource
                    {
                        Id = HeadwordBookVersionId
                    }
                }
            };
        }

        public override IList<TrackResource> GetProjectLatestTracks(long projectId)
        {
            if (projectId == 0)
                return null;

            return new List<TrackResource>
            {
                new TrackResource
                {
                    Id = 1005,
                    Resource = new Resource
                    {
                        Id = 1
                    },
                    Position = 88,
                    VersionNumber = 1,
                    Name = "track-8"
                },
                new TrackResource
                {
                    Id = 1006,
                    Resource = new Resource
                    {
                        Id = 2
                    },
                    Position = 2,
                    VersionNumber = 1,
                    Name = "track-2"
                },
                new TrackResource
                {
                    Id = 11001,
                    Resource = new Resource
                    {
                        Id = 81
                    },
                    Position = 3,
                    VersionNumber = 1,
                    Name = "track-to-remove"
                }
            };
        }

        public override IList<AudioResource> GetProjectLatestAudioResources(long projectId)
        {
            if (projectId == 0)
                return null;

            return new List<AudioResource>
            {
                new AudioResource
                {
                    Id = 2001,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-1.mp3",
                    VersionNumber = 1,
                    ResourceTrack = new Resource
                    {
                        Id = 1
                    }
                },
                new AudioResource
                {
                    Id = 2002,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-2.mp3",
                    VersionNumber = 1,
                    ResourceTrack = new Resource
                    {
                        Id = 2
                    }
                },
                new AudioResource
                {
                    Id = 2003,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-2.wav",
                    VersionNumber = 1,
                    ResourceTrack = new Resource
                    {
                        Id = 2
                    }
                },
                new AudioResource
                {
                    Id = 2004,
                    Resource = new Resource
                    {
                        Id = 4,
                    },
                    FileName = "file-full.mp3",
                    VersionNumber = 1,
                },
            };
        }

        public override IList<AudioResource> GetProjectLatestFullAudioResources(long projectId)
        {
            if (projectId == 0)
                return null;

            return new List<AudioResource>
            {
                new AudioResource
                {
                    Id = 2101,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-1.mp3",
                    VersionNumber = 1
                },
                new AudioResource
                {
                    Id = 2102,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-2.mp3",
                    VersionNumber = 1
                },
                new AudioResource
                {
                    Id = 2103,
                    Resource = new Resource
                    {
                        Id = 3,
                    },
                    FileName = "file-2.wav",
                    VersionNumber = 1
                },
            };
        }

        public override EditionNoteResource GetLatestEditionNote(long projectId)
        {
            if (projectId == 0)
                return null;

            return new EditionNoteResource
            {
                Resource = new Resource
                {
                    Id = 11,
                    Project = new Project
                    {
                        Id = projectId
                    }
                },
                VersionNumber = 1
            };
        }
    }
}