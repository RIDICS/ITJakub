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

            var project = m_projectRepository.Load<Project>(projectId);
            var dbResponsibles = m_projectRepository.GetProjectResponsibleList(projectId);

            var updatedResponsibles = new List<ProjectResponsiblePerson>();
            var comparer = new ProjectResponsibleNameEqualityComparer();

            for (var i = 0; i < bookData.Responsibles.Count; i++)
            {
                var responsiblePerson = bookData.Responsibles[i];
                var dbResponsiblePerson = GetOrCreateResponsiblePerson(responsiblePerson.NameText);
                var dbResponsibleTypes = GetOrCreateResponsibleType(responsiblePerson.TypeText);

                foreach (var dbResponsibleType in dbResponsibleTypes)
                {
                    var newProjectResponsible = new ProjectResponsiblePerson
                    {
                        Project = project,
                        ResponsiblePerson = dbResponsiblePerson,
                        ResponsibleType = dbResponsibleType,
                        Sequence = i + 1,
                    };
                    
                    if (!dbResponsibles.Contains(newProjectResponsible, comparer))
                    {
                        m_projectRepository.Create(newProjectResponsible);
                    }
                    else
                    {
                        var dbResponsible = dbResponsibles.Single(x => comparer.Equals(x, newProjectResponsible));
                        dbResponsible.Sequence = i + 1;
                        m_projectRepository.Update(dbResponsible);

                        updatedResponsibles.Add(dbResponsible);
                    }
                }
            }

            var responsiblesToRemove = dbResponsibles.Except(updatedResponsibles);
            m_projectRepository.DeleteAll(responsiblesToRemove);
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