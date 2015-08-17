using System.Collections.Generic;
using System.Reflection;
using ITJakub.Lemmatization.DataEntities;
using ITJakub.Lemmatization.DataEntities.Repositories;
using ITJakub.Lemmatization.Shared.Contracts;
using log4net;

namespace ITJakub.Lemmatization.Service
{
    public class LemmatizationService:ILemmatizationService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly LemmatizationRepository m_lemmaRepository;

        public LemmatizationService()
        {
            m_lemmaRepository = Container.Current.Resolve<LemmatizationRepository>();
        }

        public string GetLemma(string word)
        {
            var tokenCharacteristics = new TokenCharacteristic
            {
                Description = "popisek charakteristiky",
                MorphologicalCharakteristic = ""
            };

            var token = new Token
            {
                Text = "TestovaciToken",
                Description = "Testovaci popisek",
                TokenCharacteristics = new List<TokenCharacteristic>
                {
                    tokenCharacteristics
                }
            };

            var canonicalForm = new CanonicalForm
            {
                Text = "TestLemma",
                Description = "Testovaci popisek",
                Type = CanonicalFormType.Lemma,
                HyperCanonicalForm = new HyperCanonicalForm
                {
                    Text = "Testovaci HyperLemma",
                    Type = HyperCanonicalFormType.HyperLemma,
                    Description = "Testovaci popisek hyperlemmatu"
                }
            };

            var canonicalForm2 = new CanonicalForm
            {
                Text = "TestStemma",
                Description = "Testovaci popisek steamma",
                Type = CanonicalFormType.Stemma
            };

            tokenCharacteristics.CanonicalForms = new List<CanonicalForm>
            {
                canonicalForm,
                canonicalForm2
            };

            m_lemmaRepository.Save(token);


            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("test");

            return null;
        }

        public string GetStemma(string word)
        {

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("test");

            return null;
        }
    }
}