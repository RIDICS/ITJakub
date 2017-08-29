using System.Collections.Generic;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateTermsSubtask
    {
        private readonly ResourceRepository m_resourceRepository;

        public UpdateTermsSubtask(ResourceRepository resourceRepository)
        {
            m_resourceRepository = resourceRepository;
        }

        public Dictionary<string, Term> ResultTermCache { get; private set; }

        public void UpdateTerms(long projectId, BookData bookData)
        {
            var termCache = new Dictionary<string, Term>();

            if (bookData.Terms != null)
            {
                foreach (var termData in bookData.Terms)
                {
                    var dbTerm = GetOrCreateTerm(termData);
                    termCache.Add(termData.XmlId, dbTerm);
                }
            }
            
            ResultTermCache = termCache;
        }

        private Term GetOrCreateTerm(TermData data)
        {
            var dbTerm = m_resourceRepository.GetTermByExternalId(data.XmlId);
            if (dbTerm != null)
            {
                return dbTerm;
            }

            var newDbTerm = new Term
            {
                ExternalId = data.XmlId,
                Position = data.Position,
                Text = data.Text,
                TermCategory = GetOrCreateTermCategory(data.TermCategoryName),
            };
            m_resourceRepository.Create(newDbTerm);
            newDbTerm = m_resourceRepository.Load<Term>(newDbTerm.Id);

            return newDbTerm;
        }

        private TermCategory GetOrCreateTermCategory(string termCategoryName)
        {
            var dbTermCategory = m_resourceRepository.GetTermCategoryByName(termCategoryName);
            if (dbTermCategory != null)
            {
                return dbTermCategory;
            }

            var newDbTermCategory = new TermCategory
            {
                Name = termCategoryName,
            };
            m_resourceRepository.Create(newDbTermCategory);
            newDbTermCategory = m_resourceRepository.Load<TermCategory>(newDbTermCategory.Id);

            return newDbTermCategory;
        }
    }
}