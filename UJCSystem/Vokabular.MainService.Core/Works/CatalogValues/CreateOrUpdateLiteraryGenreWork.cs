using System.Net;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.CatalogValues
{
    public class CreateOrUpdateLiteraryGenreWork : UnitOfWorkBase<int>
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly int? m_genreId;
        private readonly string m_name;

        public CreateOrUpdateLiteraryGenreWork(CatalogValueRepository catalogValueRepository, int? genreId, string name) : base(catalogValueRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_genreId = genreId;
            m_name = name;
        }

        protected override int ExecuteWorkImplementation()
        {
            var literaryGenre = m_genreId != null
                ? m_catalogValueRepository.FindById<LiteraryGenre>(m_genreId.Value)
                : new LiteraryGenre();

            if (literaryGenre == null)
                throw new HttpErrorCodeException(ErrorMessages.NotFound, HttpStatusCode.NotFound);

            literaryGenre.Name = m_name;

            m_catalogValueRepository.Save(literaryGenre);

            return literaryGenre.Id;
        }
    }
}