using System;
using System.Reflection;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using log4net;
using Vokabular.Core.Storage;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.ITJakubService.Core
{
    public class AudioBookManager
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly FileSystemManager m_fileSystemManager;
        private readonly AuthorizationManager m_authorizationManager;

        public AudioBookManager(BookVersionRepository bookVersionRepository, FileSystemManager fileSystemManager, AuthorizationManager mAuthorizationManager)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_fileSystemManager = fileSystemManager;
            m_authorizationManager = mAuthorizationManager;
        }

        public FileDataContract DownloadWholeAudioBook(DownloadWholeBookContract requestContract)
        {
            m_authorizationManager.AuthorizeBook(requestContract.BookId);
            var audioType = (AudioType)requestContract.RequestedAudioType;
            var book = m_bookVersionRepository.GetBookWithLastVersion(requestContract.BookId);
            
            FullBookRecording recording = m_bookVersionRepository.GetFullBookRecording(book.LastVersion.Id, audioType);
            if (recording == null)
            {
                var message =
                    $"Cannot locate full book recording archive for book:'{requestContract.BookId}' of requested type:'{requestContract.RequestedAudioType}'";
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(message);
                throw new ArgumentException(message);
            }

            var stream = m_fileSystemManager.GetResource(book.Id, book.LastVersion.VersionId, recording.FileName, ResourceType.Audio);

            return new FileDataContract
            {
                FileName = recording.FileName,
                FileData = stream,                
                MimeType = recording.MimeType,
            };            
        }

        public AudioTrackContract DownloadAudioBookTrack(DownloadAudioBookTrackContract requestContract)
        {
            m_authorizationManager.AuthorizeBook(requestContract.BookId);
            var audioType = (AudioType) requestContract.RequestedAudioType;
            var book = m_bookVersionRepository.GetBookWithLastVersion(requestContract.BookId);

            TrackRecording recording = m_bookVersionRepository.GetRecordingByTrackAndAudioType(requestContract.BookId, requestContract.TrackPosition, audioType);
            if (recording == null)
            {
                var message =
                    $"Cannot locate recording'{requestContract.TrackPosition}' for book:'{requestContract.BookId}' of requested type:'{requestContract.RequestedAudioType}'";
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(message);
                throw new ArgumentException(message);
            }
                
            var stream = m_fileSystemManager.GetResource(book.Id, book.LastVersion.VersionId, recording.FileName, ResourceType.Audio);

            return new AudioTrackContract
            {
                FileName = recording.FileName,
                FileData = stream,
                Lenght = recording.Length,
                MimeType = recording.MimeType,
            };
        }
    }
}