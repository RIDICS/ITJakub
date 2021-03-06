﻿using System.Collections.Generic;
using System.Reflection;
using Castle.Windsor;
using ITJakub.Lemmatization.Core;
using ITJakub.Lemmatization.Shared.Contracts;
using log4net;

namespace ITJakub.Lemmatization.Service
{
    public class LemmatizationService : ILemmatizationService
    {
        private readonly LemmatizationManager m_lemmatizationManager;
        private readonly WindsorContainer m_container = Container.Current;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public LemmatizationService()
        {
            m_lemmatizationManager = m_container.Resolve<LemmatizationManager>();
        }

        public IList<TokenContract> GetTypeaheadToken(string query)
        {
            return m_lemmatizationManager.GetTypeaheadToken(query);
        }

        public long CreateToken(string token, string description)
        {
            return m_lemmatizationManager.CreateToken(token, description);
        }

        public IList<TokenCharacteristicDetailContract> GetTokenCharacteristic(long tokenId)
        {
            return m_lemmatizationManager.GetTokenCharacteristic(tokenId);
        }

        public long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description)
        {
            return m_lemmatizationManager.AddTokenCharacteristic(tokenId, morphologicalCharacteristic, description);
        }

        public void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            m_lemmatizationManager.AddCanonicalForm(tokenCharacteristicId, canonicalFormId);
        }

        public void SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId)
        {
            m_lemmatizationManager.SetHyperCanonicalForm(canonicalFormId, hyperCanonicalFormId);
        }

        public long CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description)
        {
            return m_lemmatizationManager.CreateCanonicalForm(tokenCharacteristicId, type, text, description);
        }

        public long CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description)
        {
            return m_lemmatizationManager.CreateHyperCanonicalForm(canonicalFormId, type, text, description);
        }

        public IList<CanonicalFormTypeaheadContract> GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query)
        {
            return m_lemmatizationManager.GetTypeaheadCannonicalForm(type, query);
        }

        public IList<HyperCanonicalFormContract> GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query)
        {
            return m_lemmatizationManager.GetTypeaheadHyperCannonicalForm(type, query);
        }

        public void EditToken(long tokenId, string description)
        {
            m_lemmatizationManager.EditToken(tokenId, description);
        }

        public void EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description)
        {
            m_lemmatizationManager.EditTokenCharacteristic(tokenCharacteristicId, morphologicalCharacteristic, description);
        }

        public void EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description)
        {
            m_lemmatizationManager.EditCanonicalForm(canonicalFormId, text, type, description);
        }

        public void EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type,
            string description)
        {
            m_lemmatizationManager.EditHyperCanonicalForm(hyperCanonicalFormId, text, type, description);
        }

        public int GetTokenCount()
        {
            return m_lemmatizationManager.GetTokenCount();
        }

        public IList<TokenContract> GetTokenList(int start, int count)
        {
            return m_lemmatizationManager.GetTokenList(start, count);
        }

        public TokenContract GetToken(long tokenId)
        {
            return m_lemmatizationManager.GetToken(tokenId);
        }

        public IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId)
        {
            return m_lemmatizationManager.GetCanonicalFormIdList(hyperCanonicalFormId);
        }

        public InverseCanonicalFormContract GetCanonicalFormDetail(long canonicalFormId)
        {
            return m_lemmatizationManager.GetCanonicalFormDetail(canonicalFormId);
        }

        public void DeleteTokenCharacteristic(long tokenCharacteristicId)
        {
            m_lemmatizationManager.DeleteTokenCharacteristic(tokenCharacteristicId);
        }

        public void RemoveCanonicalForm(long tokenCharacteristicId, long canonicalFormId)
        {
            m_lemmatizationManager.RemoveCanonicalForm(tokenCharacteristicId, canonicalFormId);
        }

        public void RemoveHyperCanonicalForm(long canonicalFormId)
        {
            m_lemmatizationManager.RemoveHyperCanonicalForm(canonicalFormId);
        }
    }
}