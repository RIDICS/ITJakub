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
        private readonly MetadataRepository m_metadataRepository;

        public UpdateAuthorsSubtask(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public void UpdateAuthors(long projectId, BookData bookData)
        {
            var dbProjectAuthors = m_metadataRepository.GetProjectOriginalAuthorList(projectId, true);
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
                m_metadataRepository.Delete(projectAuthor);
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
                        Project = m_metadataRepository.Load<Project>(projectId),
                        Sequence = i + 1
                    };
                    m_metadataRepository.Create(newProjectAuthor);
                }
                else
                {
                    var projectAuthor = dbProjectAuthors.Single(x => x.OriginalAuthor.FirstName == newAuthor.FirstName && x.OriginalAuthor.LastName == newAuthor.LastName);
                    projectAuthor.Sequence = i + 1;
                    m_metadataRepository.Update(projectAuthor);
                }
            }
        }

        private OriginalAuthor GetOrCreateAuthor(string firstName, string lastName)
        {
            var dbAuthor = m_metadataRepository.GetAuthorByName(firstName, lastName);
            if (dbAuthor != null)
            {
                return dbAuthor;
            }

            var newDbAuthor = new OriginalAuthor
            {
                FirstName = firstName,
                LastName = lastName
            };
            m_metadataRepository.Create(newDbAuthor);
            newDbAuthor = m_metadataRepository.Load<OriginalAuthor>(newDbAuthor.Id);

            return newDbAuthor;
        }
    }
}