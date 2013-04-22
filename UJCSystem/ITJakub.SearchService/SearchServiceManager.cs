using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;
using ITJakub.Core;
using ITJakub.Core.Database.Exist;
using ITJakub.Core.Database.Exist.DAOs;
using ITJakub.Core.Searching;
using Ujc.Naki.DataLayer;
using IFilenamesResolver = ITJakub.Core.Database.Exist.IFilenamesResolver;
using System.Linq;

namespace ITJakub.SearchService
{
    public class SearchServiceManager:ISearchService
    {

        private readonly KeyWordsDao m_keyWordsDao;
        private readonly IFilenamesResolver m_fileNameResolver;
        private readonly TeiP5Descriptor m_xmlFormatDescriptor;


        public SearchServiceManager(IFilenamesResolver fileNameResolver, TeiP5Descriptor xmlFormatDescriptor)
        {
            m_fileNameResolver = fileNameResolver;
            m_xmlFormatDescriptor = xmlFormatDescriptor;
            m_keyWordsDao = Container.Current.Resolve<KeyWordsDao>();
        }

        public List<string> AllExtendedTermsForKey(string key)
        {
            var dbResult = m_keyWordsDao.GetAllPossibleKeyWords(key);
            
            return dbResult;
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            //TODO search implementation
            throw new NotImplementedException();
        }

        public KwicResult[] GetContextForKeyWord(string keyWord)
        {
            var dbResult = m_keyWordsDao.GetKeyWordInContextByWord(keyWord);
            return dbResult;
        }
    }
}