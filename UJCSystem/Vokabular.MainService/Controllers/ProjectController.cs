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
        public IEnumerable<ProjectContract> Get()
        {
            return new List<ProjectContract>
            {
                new ProjectContract
                {
                    Id = 45,
                    CreateDate = DateTime.Now.AddDays(-1),
                    CreateUser = "Jan Novák",
                    LastEditDate = DateTime.Now,
                    LastEditUser = "Jan Novák",
                    LiteraryOriginalText = "Praha, Národní knihovna České republiky, konec 14. století",
                    Name = "Andělíku rozkochaný",
                    PublisherText = "Praha, 2009–2015, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.",
                    PageCount = 1
                },
                new ProjectContract
                {
                    Id = 45,
                    CreateDate = DateTime.Now.AddDays(-1),
                    CreateUser = "Jan Novák",
                    LastEditDate = DateTime.Now,
                    LastEditUser = "Jan Novák",
                    LiteraryOriginalText = "Praha, Národní knihovna České republiky, konec 14. století",
                    Name = "Andělíku rozkochaný",
                    PublisherText = "Praha, 2009–2015, oddělení vývoje jazyka Ústavu pro jazyk český AV ČR, v. v. i.",
                    PageCount = 1
                }
            };
        }

        [HttpGet("{id}")]
        public ProjectContract Get(int id)
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

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]ProjectContract value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
