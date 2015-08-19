using System.IO.Compression;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts;

namespace ITJakub.ITJakubService.Core
{
    public class AudioBookManager
    {
        private readonly BookVersionRepository m_bookVersionRepository;

        public AudioBookManager(BookVersionRepository bookVersionRepository)
        {
            m_bookVersionRepository = bookVersionRepository;
        }

        public FileDataContract DownloadWholeAudioBook(long bookId, AudioTypeContract audioType)
        {
           return new FileDataContract();                
        }
    }
}