using System.Security.Claims;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using Vokabular.MainService.DataContracts.Contracts.Feedback;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.AspNetCore.Extensions;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class FeedbacksManager
    {
        private readonly StaticTextManager m_staticTextManager;
        private readonly CommunicationProvider m_communicationProvider;

        public FeedbacksManager(StaticTextManager staticTextManager, CommunicationProvider communicationProvider)
        {
            m_staticTextManager = staticTextManager;
            m_communicationProvider = communicationProvider;
        }

        public FeedbackViewModel GetBasicViewModel(FeedbackFormIdentification formIdentification, string staticTextName,
            bool isAuthenticated, string scope, ClaimsPrincipal user)
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(staticTextName, scope);

            if (isAuthenticated)
            {
                var viewModel = new FeedbackViewModel
                {
                    Name = $"{user.GetFirstName()} {user.GetLastName()}",
                    Email = user.GetEmail(),
                    PageStaticText = pageStaticText,
                    FormIdentification = formIdentification
                };

                return viewModel;
            }
            else
            {
                var viewModel = new FeedbackViewModel
                {
                    PageStaticText = pageStaticText,
                    FormIdentification = formIdentification
                };

                return viewModel;
            }
        }

        public void FillViewModel(FeedbackViewModel viewModel, string staticTextName, string scope,
            FeedbackFormIdentification formIdentification)
        {
            viewModel.PageStaticText = m_staticTextManager.GetRenderedHtmlText(staticTextName, scope);
            viewModel.FormIdentification = formIdentification;
        }

        public void CreateFeedback(FeedbackViewModel model, FeedbackCategoryEnumContract category, PortalTypeContract portalType, bool isAuthenticated)
        {
            var client = m_communicationProvider.GetMainServiceFeedbackClient();

            if (isAuthenticated)
            {
                client.CreateFeedback(new CreateFeedbackContract
                {
                    FeedbackCategory = category,
                    Text = model.Text,
                    PortalType = portalType,
                });
            }
            else
            {
                client.CreateAnonymousFeedback(new CreateAnonymousFeedbackContract
                {
                    FeedbackCategory = category,
                    Text = model.Text,
                    PortalType = portalType,
                    AuthorEmail = model.Email,
                    AuthorName = model.Name,
                });
            }
        }
    }
}