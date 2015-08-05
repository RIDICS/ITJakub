using System;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class FeedbackManager
    {
        private readonly FeedbackRepository m_feedbackRepository;
        private readonly UserRepository m_userRepository;

        public FeedbackManager(UserRepository userRepository, FeedbackRepository feedbackRepository)
        {
            m_userRepository = userRepository;
            m_feedbackRepository = feedbackRepository;
        }        

        public void CreateFeedback(string note, int? userId)
        {
            User user = null;
            if(userId.HasValue)
                user = m_userRepository.Load<User>(userId);

            Feedback entity = new Feedback {CreateDate = DateTime.UtcNow, Text = note, User = user};
            m_feedbackRepository.Save(entity);
        }

        public void CreateAnonymousFeedback(string feedback, string name, string email)
        {
            Feedback entity = new Feedback { CreateDate = DateTime.UtcNow, Text = feedback, Name = name, Email = email};
            m_feedbackRepository.Save(entity);
        }
    }
}