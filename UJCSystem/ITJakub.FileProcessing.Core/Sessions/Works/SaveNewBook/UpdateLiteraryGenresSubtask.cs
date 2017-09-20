using System.Linq;
using ITJakub.FileProcessing.Core.Data;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Repositories;

namespace ITJakub.FileProcessing.Core.Sessions.Works.SaveNewBook
{
    public class UpdateLiteraryGenresSubtask
    {
        private readonly MetadataRepository m_metadataRepository;

        public UpdateLiteraryGenresSubtask(MetadataRepository metadataRepository)
        {
            m_metadataRepository = metadataRepository;
        }

        public void UpdateLiteraryGenres(long projectId, BookData bookData)
        {
            if (bookData.LiteraryGenres == null)
                return;

            var dbGenreList = m_metadataRepository.GetLiteraryGenreList();
            var project = m_metadataRepository.GetAdditionalProjectMetadata(projectId, false, false, false, true, false);

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
                    m_metadataRepository.Create(dbGenre);
                    dbGenreList.Add(dbGenre);
                }

                // Assign Literary Genre to project
                if (project.LiteraryGenres.All(x => x.Id != dbGenre.Id))
                {
                    project.LiteraryGenres.Add(dbGenre);
                }
            }
            m_metadataRepository.Update(project);
        }
    }
}