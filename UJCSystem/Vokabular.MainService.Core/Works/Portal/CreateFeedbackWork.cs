using System;
using System.Net;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.RestClient.Errors;

namespace Vokabular.MainService.Core.Works.Portal
{
    public class CreateFeedbackWork : UnitOfWorkBase<long>
    {
        private readonly PortalRepository m_portalRepository;
        private readonly CreateFeedbackContract m_data;
        private readonly FeedbackType m_feedbackType;
        private readonly int? m_userId;
        private readonly long? m_resourceVersionId;

        public CreateFeedbackWork(PortalRepository portalRepository, CreateFeedbackContract data, FeedbackType feedbackType, int? userId = null, long? resourceVersionId = null) : base(portalRepository)
        {
            m_portalRepository = portalRepository;
            m_data = data;
            m_feedbackType = feedbackType;
            m_userId = userId;
            m_resourceVersionId = resourceVersionId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var feedbackEntity = CreateEntity();

            return (long) m_portalRepository.Create(feedbackEntity);
        }

        private Feedback CreateEntity()
        {
            var now = DateTime.UtcNow;

            Feedback feedback;
            switch (m_feedbackType)
            {
                case FeedbackType.Generic:
                    feedback = CreateGenericFeedbackEntity();
                    break;
                case FeedbackType.Headword:
                    feedback = CreateHeadwordFeedbackEntity();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            feedback.Text = m_data.Text;
            feedback.CreateTime = now;
            feedback.FeedbackCategory = Mapper.Map<FeedbackCategoryEnum>(m_data.FeedbackCategory);

            if (m_userId != null)
            {
                feedback.AuthorUser = m_portalRepository.Load<User>(m_userId.Value);
            }
            else
            {
                var createAnonymousData = m_data as CreateAnonymousFeedbackContract;
                if (createAnonymousData == null)
                {
                    throw new ArgumentException("If no userId is specified then CreateAnonymousFeedbackContract is required as data argument");
                }
                feedback.AuthorEmail = createAnonymousData.AuthorEmail;
                feedback.AuthorName = createAnonymousData.AuthorName;
            }

            return feedback;
        }

        private Feedback CreateGenericFeedbackEntity()
        {
            return new Feedback();
        }

        private HeadwordFeedback CreateHeadwordFeedbackEntity()
        {
            if (m_resourceVersionId == null)
            {
                throw new ArgumentException("resourceVerionId can't be null for HeadwordFeedback");
            }

            var headwordResourceVersion = m_portalRepository.FindById<HeadwordResource>(m_resourceVersionId.Value);
            if (headwordResourceVersion == null)
            {
                throw new HttpErrorCodeException("Headword not found", HttpStatusCode.BadRequest);
            }

            return new HeadwordFeedback
            {
                HeadwordResource = headwordResourceVersion,
            };
        }
    }
}

