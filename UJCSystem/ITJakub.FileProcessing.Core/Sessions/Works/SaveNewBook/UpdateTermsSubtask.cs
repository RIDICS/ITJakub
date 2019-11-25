﻿using System.Collections.Generic;
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
                if (IsTermUpdated(dbTerm, data))
                {
                    dbTerm.Position = data.Position;
                    dbTerm.Text = data.Text;
                    dbTerm.TermCategory = GetOrCreateTermCategory(data.TermCategoryName);

                    m_resourceRepository.Update(dbTerm);
                }

                return dbTerm;
            }

            // some books can use different XmlId
            var dbTerm2 = m_resourceRepository.GetTermByNameAndCategoryName(data.Text, data.TermCategoryName);
            if (dbTerm2 != null)
            {
                if (dbTerm2.Position != data.Position || dbTerm2.ExternalId != data.XmlId)
                {
                    dbTerm2.Position = data.Position;
                    dbTerm2.ExternalId = data.XmlId;

                    m_resourceRepository.Update(dbTerm2);
                }

                return dbTerm2;
            }

            // create new Term
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

        private bool IsTermUpdated(Term dbTerm, TermData termData)
        {
            return dbTerm.Text != termData.Text ||
                   dbTerm.Position != termData.Position ||
                   dbTerm.TermCategory.Name != termData.TermCategoryName;
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