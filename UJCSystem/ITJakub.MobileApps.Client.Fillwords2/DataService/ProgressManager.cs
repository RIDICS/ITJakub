using System;
using System.Collections.Generic;
using System.Linq;
using ITJakub.MobileApps.Client.Fillwords2.DataContract;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel.Data;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using Newtonsoft.Json;

namespace ITJakub.MobileApps.Client.Fillwords2.DataService
{
    public class ProgressManager
    {
        private readonly ISynchronizeCommunication m_applicationCommunication;
        private const string UserAnswerObjectType = "UserAnswer";

        public ProgressManager(ISynchronizeCommunication applicationCommunication)
        {
            m_applicationCommunication = applicationCommunication;
        }

        public async void SendAnswer(int wordPosition, IList<LetterOptionViewModel> selectedAnswers, Action<Exception> callback)
        {
            var progressContract = new FillwordsAnswerContract
            {
                WordPosition = wordPosition,
                Answers = selectedAnswers.Select(x => new FillwordsAnswerContract.OptionContract
                {
                    StartPosition = x.StartPosition,
                    Answer = x.SelectedAnswer
                }).ToList()
            };
            var serializedProgressContract = JsonConvert.SerializeObject(progressContract);

            try
            {
                await m_applicationCommunication.SendObjectAsync(ApplicationType.Fillwords2, UserAnswerObjectType, serializedProgressContract);
                callback(null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(exception);
            }
        }

        public async void GetAnswers(Action<IList<AnswerViewModel>, Exception> callback)
        {
            try
            {
                var answers = await m_applicationCommunication.GetObjectsAsync(ApplicationType.Fillwords2, new DateTime(1975, 1, 1), UserAnswerObjectType);
                var resultList = new List<AnswerViewModel>();
                foreach (var objectDetails in answers.Where(x => x.Author.IsMe))
                {
                    var answerContract = JsonConvert.DeserializeObject<FillwordsAnswerContract>(objectDetails.Data);
                    resultList.Add(new AnswerViewModel
                    {
                        WordPosition = answerContract.WordPosition,
                        Answers = answerContract.Answers.Select(x => new ConcreteAnswerViewModel
                        {
                            Answer = x.Answer,
                            StartPosition = x.StartPosition
                        }).ToList()
                    });
                }

                callback(resultList, null);
            }
            catch (ClientCommunicationException exception)
            {
                callback(null, exception);
            }
        }
    }
}
