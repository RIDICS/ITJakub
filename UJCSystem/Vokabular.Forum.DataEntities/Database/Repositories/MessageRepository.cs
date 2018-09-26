using Vokabular.ForumSite.DataEntities.Database.Entities;
using Vokabular.Shared.DataEntities.Daos;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.ForumSite.DataEntities.Database.Repositories
{
    public class MessageRepository : NHibernateDao
    {
        public MessageRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override object Create(object instance)
        {
            base.Create(instance);

            Message message = (Message) instance;
            Topic topic = message.Topic;
            topic.LastMessage = message;
            topic.LastUser = message.User;
            topic.LastPosted = message.Posted;
            topic.LastMessageFlags = message.Flags;
            topic.LastUserDisplayName = message.UserDisplayName;
            topic.NumPosts++;
            Update(topic);

            Forum forum = message.Topic.Forum;
            forum.LastPosted = topic.Posted;
            forum.LastTopic = topic;
            forum.LastUser = topic.User;
            forum.LastUserDisplayName = topic.UserDisplayName;
            forum.LastMessage = topic.LastMessage;
            forum.NumPosts++;
            Update(forum);

            return instance;
        }
    }
}

