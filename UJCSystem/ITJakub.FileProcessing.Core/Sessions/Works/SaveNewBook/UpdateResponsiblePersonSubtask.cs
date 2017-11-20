using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateResponsiblePersonSubtask
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly PersonRepository m_personRepository;

        public UpdateResponsiblePersonSubtask(ProjectRepository projectRepository, PersonRepository personRepository)
        {
            m_projectRepository = projectRepository;
            m_personRepository = personRepository;
        }

        public void UpdateResponsiblePersonList(long projectId, BookData bookData)
        {
            if (bookData.Responsibles == null)
                return;

            var itemsToCreate = new List<ProjectResponsiblePerson>();
            var project = m_projectRepository.Load<Project>(projectId);
            var dbResponsibles = m_projectRepository.GetProjectResponsibleList(projectId);

            foreach (var responsiblePerson in bookData.Responsibles)
            {
                var dbResponsiblePerson = GetOrCreateResponsiblePerson(responsiblePerson.NameText);
                var dbResponsibleTypes = GetOrCreateResponsibleType(responsiblePerson.TypeText);

                foreach (var dbResponsibleType in dbResponsibleTypes)
                {
                    var newProjectResponsible = new ProjectResponsiblePerson
                    {
                        Project = project,
                        ResponsiblePerson = dbResponsiblePerson,
                        ResponsibleType = dbResponsibleType
                    };
                    var comparer = new ProjectResponsibleNameEqualityComparer();

                    if (!dbResponsibles.Contains(newProjectResponsible, comparer))
                    {
                        itemsToCreate.Add(newProjectResponsible);
                    }
                }
            }

            m_projectRepository.CreateAll(itemsToCreate);
        }

        private ResponsiblePerson GetOrCreateResponsiblePerson(string responsiblePersonName)
        {
            var personData = PersonHelper.ConvertToEntity(responsiblePersonName);
            var responsible = m_personRepository.GetResponsiblePersonByName(personData.FirstName, personData.LastName);
            if (responsible != null)
            {
                return responsible;
            }

            responsible = new ResponsiblePerson
            {
                FirstName = personData.FirstName,
                LastName = personData.LastName
            };
            m_projectRepository.Create(responsible);
            return responsible;
        }

        private List<ResponsibleType> GetOrCreateResponsibleType(string text)
        {
            var resultList = new List<ResponsibleType>();
            var typeTexts = text.Split(',');

            foreach (var typeText in typeTexts)
            {
                var trimmedTypeText = typeText.Trim();
                var responsibleType = m_personRepository.GetResponsibleTypeByName(trimmedTypeText);

                if (responsibleType != null)
                {
                    resultList.Add(responsibleType);
                    continue;
                }

                ResponsibleTypeEnum typeEnum;
                Enum.TryParse(trimmedTypeText, true, out typeEnum);

                responsibleType = new ResponsibleType
                {
                    Text = trimmedTypeText,
                    Type = typeEnum
                };
                m_projectRepository.Create(responsibleType);
                resultList.Add(responsibleType);
            }

            return resultList;
        }
    }
}