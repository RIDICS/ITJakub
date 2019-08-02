using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateKeywordWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_keywordId;
        private readonly string m_name;

        public CreateOrUpdateKeywordWork(CatalogValueRepository catalogValueRepository, int? keywordId, string name) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_keywordId = keywordId;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var keywordEntity = m_keywordId != null
                ? m_catalogValueRepository.FindById<Keyword>(m_keywordId.Value)
                : new Keyword();

            if (keywordEntity == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            keywordEntity.Text = m_name;

            m_catalogValueRepository.Save(keywordEntity);

            return keywordEntity.Id;
        }
    }
}