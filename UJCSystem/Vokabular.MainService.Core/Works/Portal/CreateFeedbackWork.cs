using System;
using AutoMapper;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.Shared.DataEntities.UnitOfWork;

namespace Vokabular.MainService.Core.Works.Portal
{
    public class CreateFeedbackWork : UnitOfWorkBase<long>
    {
        private readonly IMapper m_mapper;
        private readonly PortalRepository m_portalRepository;
        private readonly CreateFeedbackContract m_data;
        private readonly FeedbackType m_feedbackType;
        private readonly int? m_userId;
        private readonly long? m_resourceVersionId;

        public CreateFeedbackWork(IMapper mapper, PortalRepository portalRepository, CreateFeedbackContract data, FeedbackType feedbackType, int? userId = null, long? resourceVersionId = null) : base(portalRepository)
        {
            m_mapper = mapper;
            m_portalRepository = portalRepository;
            m_data = data;
            m_feedbackType = feedbackType;
            m_userId = userId;
            m_resourceVersionId = resourceVersionId;
        }

        protected override long ExecuteWorkImplementation()
        {
            var feedbackEntity = CreateEntity();

            if (m_feedbackType == FeedbackType.Headword)
            {
                feedbackEntity.FeedbackCategory = FeedbackCategoryEnum.Dictionaries; // Feedback category correction for headwords
            }

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
            feedback.FeedbackCategory = m_mapper.Map<FeedbackCategoryEnum>(m_data.FeedbackCategory);

            if (m_userId != null)
            {
                feedback.AuthorUser = m_portalRepository.Load<User>(m_userId.Value);
            }
            else
            {
                var createAnonymousData = m_data as CreateAnonymousFeedbackContract;
                if (createAnonymousData == null)
                {
                    throw new MainServiceException(MainServiceErrorCode.CreateAnonymousFeedback, "If no userId is specified then CreateAnonymousFeedbackContract is required as data argument");
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
                throw new MainServiceException(MainServiceErrorCode.ResourceVersionIdNull, "resourceVersionId can't be null for HeadwordFeedback");
            }

            var headwordResourceVersion = m_portalRepository.FindById<HeadwordResource>(m_resourceVersionId.Value);
            if (headwordResourceVersion == null)
            {
                throw new MainServiceException(MainServiceErrorCode.HeadwordNotFound, "The headword was not found");
            }

            return new HeadwordFeedback
            {
                HeadwordResource = headwordResourceVersion,
            };
        }
    }
}

