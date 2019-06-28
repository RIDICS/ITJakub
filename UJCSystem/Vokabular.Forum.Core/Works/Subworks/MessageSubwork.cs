using System;
using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.ForumSite.DataEntities.Database.Repositories;

namespace Vokabular.ForumSite.Core.Works.Subworks
{
    public class MessageSubwork
    {
        private readonly TopicRepository m_topicRepository;

        public MessageSubwork(TopicRepository topicRepository)
        {
            m_topicRepository = topicRepository;
        }

        public void PostMessageInTopic(Topic topic, User user, string messageText)
        {
            var message = new Message(topic, user, DateTime.UtcNow, messageText);

            m_topicRepository.Create(message);

            topic.LastMessage = message;
            topic.LastUser = message.User;
            topic.LastPosted = message.Posted;
            topic.LastMessageFlags = message.Flags;
            topic.LastUserDisplayName = message.UserDisplayName;
            topic.NumPosts++;
            m_topicRepository.Update(topic);

            Forum forum = message.Topic.Forum;
            forum.LastPosted = topic.Posted;
            forum.LastTopic = topic;
            forum.LastUser = topic.User;
            forum.LastUserDisplayName = topic.UserDisplayName;
            forum.LastMessage = topic.LastMessage;
            forum.NumPosts++;
            m_topicRepository.Update(forum);
        }
    }
}
