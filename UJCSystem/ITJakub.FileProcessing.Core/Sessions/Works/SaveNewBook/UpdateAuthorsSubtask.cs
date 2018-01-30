using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.Sessions.Works.Helpers;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateAuthorsSubtask
    {
        private readonly ProjectRepository m_projectRepository;
        private readonly PersonRepository m_personRepository;

        public UpdateAuthorsSubtask(ProjectRepository projectRepository, PersonRepository personRepository)
        {
            m_projectRepository = projectRepository;
            m_personRepository = personRepository;
        }

        public void UpdateAuthors(long projectId, BookData bookData)
        {
            var dbProjectAuthors = m_projectRepository.GetProjectOriginalAuthorList(projectId, true);
            var dbAuthors = dbProjectAuthors.Select(x => x.OriginalAuthor).ToList();
            var newAuthors = bookData.Authors?.Select(x => PersonHelper.ConvertToOriginalAuthor(x.Name)).ToList() ?? new List<OriginalAuthor>();

            var comparer = new AuthorNameEqualityComparer();
            var authorsToAdd = newAuthors.Except(dbAuthors, comparer).ToList();
            var authorsToRemove = dbAuthors.Except(newAuthors, comparer).ToList();

            if (authorsToAdd.Count == 0 && authorsToRemove.Count == 0)
            {
                return;
            }


            foreach (var author in authorsToRemove)
            {
                var projectAuthor = dbProjectAuthors.Single(x => x.OriginalAuthor.Id == author.Id);
                m_projectRepository.Delete(projectAuthor);
            }


            for (int i = 0; i < newAuthors.Count; i++)
            {
                var newAuthor = newAuthors[i];
                if (authorsToAdd.Contains(newAuthor, comparer))
                {
                    var dbAuthor = GetOrCreateAuthor(newAuthor.FirstName, newAuthor.LastName);
                    var newProjectAuthor = new ProjectOriginalAuthor
                    {
                        OriginalAuthor = dbAuthor,
                        Project = m_projectRepository.Load<Project>(projectId),
                        Sequence = i + 1
                    };
                    m_projectRepository.Create(newProjectAuthor);
                }
                else
                {
                    var projectAuthor = dbProjectAuthors.Single(x => x.OriginalAuthor.FirstName == newAuthor.FirstName && x.OriginalAuthor.LastName == newAuthor.LastName);
                    projectAuthor.Sequence = i + 1;
                    m_projectRepository.Update(projectAuthor);
                }
            }
        }

        private OriginalAuthor GetOrCreateAuthor(string firstName, string lastName)
        {
            var dbAuthor = m_personRepository.GetAuthorByName(firstName, lastName);
            if (dbAuthor != null)
            {
                return dbAuthor;
            }

            var newDbAuthor = new OriginalAuthor
            {
                FirstName = firstName,
                LastName = lastName
            };
            m_projectRepository.Create(newDbAuthor);
            newDbAuthor = m_projectRepository.Load<OriginalAuthor>(newDbAuthor.Id);

            return newDbAuthor;
        }
    }
}