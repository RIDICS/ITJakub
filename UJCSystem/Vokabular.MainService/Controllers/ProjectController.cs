using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.ServiceContracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller, IProjectMainService
    {
        [HttpGet]
        public List<ProjectContract> GetProjectList()
        {
            return new List<ProjectContract>
            {
                MockDataProject.GetProjectContract(45),
                MockDataProject.GetProjectContract(46),
                MockDataProject.GetProjectContract(47)
            };
        }

        [HttpGet("{projectId}")]
        public ProjectContract GetProject(long projectId)
        {
            return MockDataProject.GetProjectContract(projectId);
        }

        [HttpPost]
        public long CreateProject([FromBody] ProjectContract project)
        {
            return 5;
        }

        [HttpDelete("{projectId}")]
        public void DeleteProject(long projectId)
        {
        }

        [HttpGet("{projectId}/metadata")]
        public ProjectMetadataContract GetProjectMetadata(long projectId)
        {
            return MockDataProject.GetProjectMetadata();
        }
    }

    public class MockDataProject
    {
        public static ProjectContract GetProjectContract(long id)
        {
            return new ProjectContract
            {
                Id = id,
                CreateDate = DateTime.Now.AddDays(-1),
                CreateUser = "Jan Novák",
                LastEditDate = DateTime.Now,
                LastEditUser = "Jan Novák",
                LiteraryOriginalText = "Praha, Národní knihovna České republiky, konec 14. století",
                Name = "Andělíku rozkochaný",
                PublisherText = "Praha, 2009–2015, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.",
                PageCount = 1
            };
        }

        public static ProjectMetadataContract GetProjectMetadata()
        {
            return new ProjectMetadataContract
            {
                Editor = "Jan Novák",
                LastModification = DateTime.Now,
                LiteraryGenre = "xxxxxxx",
                LiteraryKind = "xxxxxxx",
                LiteraryOriginal = new ProjectLiteraryOriginalContract
                {
                    City = "Praha",
                    Country = "Česká republika",
                    Institution = "Knihovna národního muzea",
                    Signature = "sign.: III E48",
                    Extent = "148f"
                },
                RelicAbbreviation = "xxxxxxx",
                SourceAbbreviation = "xxxxxxx"
            };
        }
    }
}
