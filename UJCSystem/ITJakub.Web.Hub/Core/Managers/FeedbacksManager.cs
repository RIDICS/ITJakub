using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Clients;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Web.Hub.Models;

namespace ITJakub.Web.Hub.Core.Managers
{
    public class FeedbacksManager
    {
        private readonly StaticTextManager m_staticTextManager;

        public FeedbacksManager(StaticTextManager staticTextManager)
        {
            m_staticTextManager = staticTextManager;
        }

        public FeedbackViewModel GetBasicViewModel(FeedbackFormIdentification formIdentification, string staticTextName, string scope,ItJakubServiceEncryptedClient client, string username = null)
        {
            var pageStaticText = m_staticTextManager.GetRenderedHtmlText(staticTextName, scope);

            if (string.IsNullOrWhiteSpace(username))
            {
                var viewModel = new FeedbackViewModel
                {
                    PageStaticText = pageStaticText,
                    FormIdentification = formIdentification
                };

                return viewModel;
            }

            using (client)
            {
                var user = client.FindUserByUserName(username);
                var viewModel = new FeedbackViewModel
                {
                    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                    Email = user.Email,
                    PageStaticText = pageStaticText,
                    FormIdentification = formIdentification
                };

                return viewModel;
            }
        }

        public void FillViewModel(FeedbackViewModel viewModel, string staticTextName, string scope, FeedbackFormIdentification formIdentification)
        {
            viewModel.PageStaticText = m_staticTextManager.GetRenderedHtmlText(staticTextName, scope);
            viewModel.FormIdentification = formIdentification;
        }

        public void CreateFeedback(FeedbackViewModel model, FeedbackCategoryEnumContract category, IItJakubService client, bool isAuthenticated, string userName)
        {
            using (client)
            {
                if (isAuthenticated)
                {
                    client.CreateFeedback(model.Text, userName, category);
                }
                else
                {
                    client.CreateAnonymousFeedback(model.Text, model.Name, model.Email, category);
                }
            }
        }
    }
}