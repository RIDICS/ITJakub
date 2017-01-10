using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ResponsiblePersonController : Controller
    {
        private readonly PersonManager m_personManager;

        public ResponsiblePersonController(PersonManager personManager)
        {
            m_personManager = personManager;
        }

        [HttpPost("")]
        public int CreateResponsiblePerson([FromBody] NewResponsiblePersonContract responsiblePerson)
        {
            return m_personManager.CreateResponsiblePerson(responsiblePerson);
        }

        [HttpPost("type")]
        public int CreateResponsibleType([FromBody] ResponsibleTypeContract responsibleType)
        {
            return m_personManager.CreateResponsibleType(responsibleType);
        }

        [HttpGet("type")]
        public List<ResponsibleTypeContract> GetResponsibleTypeList()
        {
            return m_personManager.GetResponsibleTypeList();
        }

        [HttpGet("autocomplete")]
        public List<ResponsiblePersonContract> GetAutocomplete([FromQuery] string query)
        {
            return m_personManager.GetResponsiblePersonAutocomplete(query);
        }
    }
}