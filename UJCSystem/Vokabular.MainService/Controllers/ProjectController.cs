using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : Controller
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

        [HttpGet("{id}")]
        public ProjectContract GetProject(int id)
        {
            return MockDataProject.GetProjectContract(id);
        }

        [HttpPost]
        public long CreateProject([FromBody] ProjectContract project)
        {
            return 5;
        }

        [HttpDelete("{id}")]
        public void DeleteProject(int id)
        {
        }

        [HttpGet("{id}/metadata")]
        public void GetProjectMetadata(long id)
        {

        }
    }

    public class MockDataProject
    {
        public static ProjectContract GetProjectContract(int id)
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
    }
}
