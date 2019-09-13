using System;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core;
using Vokabular.MainService.Core.Works.Portal;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.Test.Containers;
using Vokabular.MainService.Test.Mock;
using Vokabular.Shared.AspNetCore.Container.Extensions;

namespace Vokabular.MainService.Test
{
    [TestClass]
    public class NewFeedbackTest : IDisposable
    {
        private readonly DryIocContainer m_container;
        private readonly IMapper m_mapper;

        public NewFeedbackTest()
        {
            var services = new ServiceCollection();
            services.AddAutoMapper();

            var container = new DryIocContainer();
            container.Install<MainServiceCoreContainerRegistration>();
            container.Populate(services);

            m_container = container;
            m_mapper = container.Resolve<IMapper>();
        }

        [TestMethod]
        public void TestCreateFeedbackFromUser()
        {
            const int userId = 67;
            const long resourceVersionId = 48915;

            var repository = new MockPortalRepository();

            var data = new CreateFeedbackContract
            {
                FeedbackCategory = FeedbackCategoryEnumContract.OldGrammar,
                Text = "Feedback text"
            };

            var work1 = new CreateFeedbackWork(m_mapper, repository, data, FeedbackType.Generic, userId);
            work1.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(0, repository.UpdatedObjects.Count);
            Assert.AreEqual(typeof(Feedback), repository.CreatedObjects.First().GetType());

            
            repository = new MockPortalRepository();
            var work2 = new CreateFeedbackWork(m_mapper, repository, data, FeedbackType.Headword, userId);

            Assert.ThrowsException<MainServiceException>(() => work2.Execute());


            repository = new MockPortalRepository();
            var work3 = new CreateFeedbackWork(m_mapper, repository, data, FeedbackType.Headword, userId, resourceVersionId);
            work3.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(0, repository.UpdatedObjects.Count);
            Assert.AreEqual(typeof(HeadwordFeedback), repository.CreatedObjects.First().GetType());


            var feedbackEntity = (Feedback)repository.CreatedObjects.First();
            Assert.IsNull(feedbackEntity.AuthorEmail);
            Assert.IsNull(feedbackEntity.AuthorName);
            Assert.IsNotNull(feedbackEntity.AuthorUser);
            Assert.AreEqual(data.Text, feedbackEntity.Text);
        }

        [TestMethod]
        public void TestCreateAnonymousFeedback()
        {
            const long resourceVersionId = 2149;

            var repository = new MockPortalRepository();

            var data = new CreateAnonymousFeedbackContract
            {
                FeedbackCategory = FeedbackCategoryEnumContract.OldGrammar,
                Text = "Feedback text",
                AuthorEmail = "author@example.com",
                AuthorName = "Author Name"
            };

            var work1 = new CreateFeedbackWork(m_mapper, repository, data, FeedbackType.Generic, null, resourceVersionId);
            work1.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(0, repository.UpdatedObjects.Count);
            Assert.AreEqual(typeof(Feedback), repository.CreatedObjects.First().GetType());


            repository = new MockPortalRepository();
            var work2 = new CreateFeedbackWork(m_mapper, repository, new CreateFeedbackContract(), FeedbackType.Headword);

            Assert.ThrowsException<MainServiceException>(() => work2.Execute());


            repository = new MockPortalRepository();
            var work3 = new CreateFeedbackWork(m_mapper, repository, data, FeedbackType.Headword, null, resourceVersionId);
            work3.Execute();

            Assert.AreEqual(1, repository.CreatedObjects.Count);
            Assert.AreEqual(0, repository.UpdatedObjects.Count);
            Assert.AreEqual(typeof(HeadwordFeedback), repository.CreatedObjects.First().GetType());


            var feedbackEntity = (Feedback) repository.CreatedObjects.First();
            Assert.IsNull(feedbackEntity.AuthorUser);
            Assert.AreEqual(data.AuthorEmail, feedbackEntity.AuthorEmail);
            Assert.AreEqual(data.AuthorName, feedbackEntity.AuthorName);
            Assert.AreEqual(data.Text, feedbackEntity.Text);
        }

        public void Dispose()
        {
            m_container.Dispose();
        }
    }
}