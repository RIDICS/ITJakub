using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateLiteraryGenresSubtask
    {
        private readonly CatalogValueRepository m_catalogValueRepository;
        private readonly MetadataRepository m_metadataRepository;

        public UpdateLiteraryGenresSubtask(CatalogValueRepository catalogValueRepository, MetadataRepository metadataRepository)
        {
            m_catalogValueRepository = catalogValueRepository;
            m_metadataRepository = metadataRepository;
        }

        public void UpdateLiteraryGenres(long projectId, BookData bookData)
        {
            if (bookData.LiteraryGenres == null)
                return;

            var dbGenreList = m_catalogValueRepository.GetLiteraryGenreList();
            var project = m_metadataRepository.GetAdditionalProjectMetadata(projectId, false, false, false, true, false, false, false);

            foreach (var newGenreName in bookData.LiteraryGenres)
            {
                var dbGenre = dbGenreList.FirstOrDefault(x => x.Name == newGenreName);

                // Create new Literary Genre
                if (dbGenre == null)
                {
                    dbGenre = new LiteraryGenre
                    {
                        Name = newGenreName
                    };
                    m_catalogValueRepository.Create(dbGenre);
                    dbGenreList.Add(dbGenre);
                }

                // Assign Literary Genre to project
                if (project.LiteraryGenres.All(x => x.Id != dbGenre.Id))
                {
                    project.LiteraryGenres.Add(dbGenre);
                }
            }
            m_catalogValueRepository.Update(project);
        }
    }
}