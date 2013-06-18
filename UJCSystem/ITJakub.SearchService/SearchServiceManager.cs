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

        private readonly ExistWordsDao m_existWordsDao;
        private readonly IFilenamesResolver m_fileNameResolver;
        private readonly TeiP5Descriptor m_xmlFormatDescriptor;
        private readonly BookDao m_bookDao;


        public SearchServiceManager(IFilenamesResolver fileNameResolver, TeiP5Descriptor xmlFormatDescriptor)
        {
            m_fileNameResolver = fileNameResolver;
            m_xmlFormatDescriptor = xmlFormatDescriptor;
            m_existWordsDao = Container.Current.Resolve<ExistWordsDao>();
            m_bookDao = Container.Current.Resolve<BookDao>();
        }

        public List<string> AllExtendedTermsForKey(string key)
        {
            var dbResult = m_existWordsDao.GetAllPossibleKeyWords(key);
            return dbResult;
        }

        public List<string> AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetAllPossibleKeyWords(key, booksIds);
            return dbResult;
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            //TODO search implementation
            throw new NotImplementedException();
        }

        public List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord)
        {
            var dbResult = m_existWordsDao.GetKeyWordInContextByWord(keyWord);
            return dbResult;
        }


        public List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetKeyWordInContextByWord(keyWord, booksIds);
            return dbResult;
        }


        public List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord)
        {
            var dbResult = m_existWordsDao.GetXmlContextByWord(keyWord);
            return dbResult;
        }


        public List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetXmlContextByWord(keyWord, booksIds);
            return dbResult;
        }


        public string GetTitleById(string id)
        {
            var dbResult = m_bookDao.GetTitleByBookId(id);
            return dbResult;
        }

       
    }
}