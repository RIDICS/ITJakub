using System;
using System.Collections.Generic;
using ITJakub.Contracts;
using ITJakub.Contracts.Searching;
using ITJakub.Core.Database.Exist;
using ITJakub.Core.Database.Exist.DAOs;

namespace ITJakub.SearchService
{
    public class SearchServiceManager:ISearchService
    {

        private readonly ExistWordsDao m_existWordsDao;
        private readonly IFilenamesResolver m_fileNameResolver;
        private readonly TeiP5Descriptor m_xmlFormatDescriptor;
        private readonly BookDao m_bookDao;
        //private readonly XSLTransformDirector m_xsltDirector;


        public SearchServiceManager(IFilenamesResolver fileNameResolver, TeiP5Descriptor xmlFormatDescriptor)
        {
            m_fileNameResolver = fileNameResolver;
            m_xmlFormatDescriptor = xmlFormatDescriptor;
            m_existWordsDao = Container.Current.Resolve<ExistWordsDao>();
            m_bookDao = Container.Current.Resolve<BookDao>();
            //m_xsltDirector = Container.Current.Resolve<XSLTransformDirector>();
        }

        public List<string> AllExtendedTermsForKey(string key)
        {
            var dbResult = m_existWordsDao.GetAllPossibleKeyWords(key);
            return dbResult;
        }

        public List<string> AllExtendedTermsForKeyWithBooksRestriction(string key, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetAllPossibleKeyWords(key, booksIds);//todo delete toLowercase
            return dbResult;
        }

        public void Search(List<SearchCriteriumBase> criteria)
        {
            //TODO search implementation
            throw new NotImplementedException();
        }

        public List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord)
        {
            var dbResult = m_existWordsDao.GetKeyWordInContextByWord(keyWord.ToLowerInvariant());//todo delete toLowercase
            return dbResult;
        }


        public List<SearchResultWithKwicContext> GetKwicContextForKeyWord(string keyWord, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetKeyWordInContextByWord(keyWord.ToLowerInvariant(), booksIds);//todo delete toLowercase
            return dbResult;
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetHtmlContextByWord(keyWord.ToLowerInvariant(), booksIds);//todo delete toLowercase
            return dbResult;
        }


        public List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord)
        {
            var dbResult = m_existWordsDao.GetXmlContextByWord(keyWord.ToLowerInvariant());//todo delete toLowercase
            return dbResult;
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWord(string keyWord)
        {
            var dbResult = m_existWordsDao.GetHtmlContextByWord(keyWord.ToLowerInvariant());//todo delete toLowercase
            return dbResult;
        }

        public List<SearchResultWithHtmlContext> GetHtmlContextForKeyWordWithBooksRestriction(string keyWord, List<string> bookIds)
        {
            var dbResult = m_existWordsDao.GetHtmlContextByWord(keyWord.ToLowerInvariant(), bookIds);//todo delete toLowercase
            return dbResult;
        }


        public List<SearchResultWithXmlContext> GetXmlContextForKeyWord(string keyWord, List<string> booksIds)
        {
            var dbResult = m_existWordsDao.GetXmlContextByWord(keyWord.ToLowerInvariant(), booksIds);//todo delete toLowercase
            return dbResult;
        }


        public string GetTitleById(string id)
        {
            var dbResult = m_bookDao.GetTitleByBookId(id);
            return dbResult;
        }

       
    }
}