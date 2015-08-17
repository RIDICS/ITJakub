using System;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Notes;

namespace ITJakub.ITJakubService.Core
{
    public class FeedbackManager
    {
        private readonly FeedbackRepository m_feedbackRepository;
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly UserRepository m_userRepository;

        public FeedbackManager(UserRepository userRepository, FeedbackRepository feedbackRepository, BookVersionRepository bookVersionRepository)
        {
            m_userRepository = userRepository;
            m_feedbackRepository = feedbackRepository;
            m_bookVersionRepository = bookVersionRepository;
        }        

        public void CreateFeedback(string note, string username, FeedbackCategoryEnumContract feedbackCategory)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is empty, cannot add bookmark");

            User user = m_userRepository.FindByUserName(username);
            if (user == null)
                throw new ArgumentException(string.Format("Cannot locate user by username: '{0}'", username));

            Feedback entity = new Feedback {CreateDate = DateTime.UtcNow, Text = note, User = user, Category = (FeedbackCategoryEnum)feedbackCategory };
            m_feedbackRepository.Save(entity);
        }

        public void CreateAnonymousFeedback(string feedback, string name, string email, FeedbackCategoryEnumContract feedbackCategory)
        {
            Feedback entity = new Feedback { CreateDate = DateTime.UtcNow, Text = feedback, Name = name, Email = email, Category = (FeedbackCategoryEnum)feedbackCategory };
            m_feedbackRepository.Save(entity);
        }

        public void CreateAnonymousFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string name, string email)
        {
            BookHeadword headwordEntity = m_bookVersionRepository.GetFirstHeadwordInfo(bookXmlId, entryXmlId, versionXmlId);

            HeadwordFeedback entity = new HeadwordFeedback
            {
                CreateDate = DateTime.UtcNow,
                Text = feedback,
                Name = name,
                Email = email,
                BookHeadword = headwordEntity,
                Category = FeedbackCategoryEnum.Dictionaries
            };
            m_feedbackRepository.Save(entity);
        }

        public void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username is empty, cannot add bookmark");

            User user = m_userRepository.FindByUserName(username);
            if (user == null)
                throw new ArgumentException(string.Format("Cannot locate user by username: '{0}'", username));

            BookHeadword headwordEntity = m_bookVersionRepository.GetFirstHeadwordInfo(bookXmlId, entryXmlId, versionXmlId);
            if (headwordEntity == null)
                throw new ArgumentException(string.Format("Cannot find headword with bookId: {0}, versionId: {1}, entryXmlId: {2}", bookXmlId, versionXmlId, entryXmlId));
            
            HeadwordFeedback entity = new HeadwordFeedback
            {
                CreateDate = DateTime.UtcNow,
                Text = feedback,
                BookHeadword = headwordEntity,
                User = user,
                Category = FeedbackCategoryEnum.Dictionaries
            };
            m_feedbackRepository.Save(entity);
        }
    }
}