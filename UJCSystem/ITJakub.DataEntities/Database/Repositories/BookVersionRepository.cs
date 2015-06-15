using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using ITJakub.DataEntities.Database.Daos;
using ITJakub.DataEntities.Database.Entities;
using log4net;

namespace ITJakub.DataEntities.Database.Repositories
{
    [Transactional]
    public class BookVersionRepository : NHibernateTransactionalDao
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BookVersionRepository(ISessionManager sessManager)
            : base(sessManager)
        {
        }

        [Transaction(TransactionMode.Requires)]
        public virtual long Create(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                return (long) session.Save(bookVersion);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void Delete(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                session.Delete(bookVersion);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookVersion FindBookVersionByGuid(string bookVersionGuid)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<BookVersion>()
                        .Where(bookVersion => bookVersion.VersionId == bookVersionGuid)
                        .SingleOrDefault<BookVersion>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Publisher FindPublisherByText(string text)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<Publisher>()
                        .Where(publisher => publisher.Text == text)
                        .SingleOrDefault<Publisher>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual Responsible FindResponsible(string text, ResponsibleType type)
        {
            using (var session = GetSession())
            {
                var responsibleType = FindResponsibleType(type) ?? type;
                return
                    session.QueryOver<Responsible>()
                        .Where(
                            responsible =>
                                responsible.Text == text && responsible.ResponsibleType.Id == responsibleType.Id)
                        .Take(1)
                        .SingleOrDefault<Responsible>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual ResponsibleType FindResponsibleType(ResponsibleType responsibleType)
        {
            using (var session = GetSession())
            {
                return
                    session.QueryOver<ResponsibleType>()
                        .Where(
                            respType =>
                                respType.Text == responsibleType.Text ||
                                respType.Type == responsibleType.Type)
                        .Take(1)
                        .SingleOrDefault<ResponsibleType>();
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage GetPageByVersionAndXmlId(BookVersion bookVersion, string pageXmlId)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id && x.XmlId == pageXmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage FindBookPageByVersionAndPosition(BookVersion bookVersion, int position)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id && x.Position == position)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage FindBookPageByVersionAndXmlId(long versionId, string xmlId)
        {
            using (var session = GetSession())
            {
                var bookPage =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == versionId && x.XmlId == xmlId)
                        .SingleOrDefault<BookPage>();
                return bookPage;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookPage> GetPageList(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                var bookPages =
                    session.QueryOver<BookPage>()
                        .Where(x => x.BookVersion.Id == bookVersion.Id)
                        .List<BookPage>();
                return bookPages;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual IList<BookContentItem> GetRootBookContentItemsWithPagesAndAncestors(BookVersion bookVersion)
        {
            using (var session = GetSession())
            {
                var bookContentItems =
                    session.QueryOver<BookContentItem>()
                        //.Fetch(x => x.Page).Eager
                        .Where(item => item.BookVersion.Id == bookVersion.Id && item.ParentBookContentItem == null)
                        .List<BookContentItem>();
                return bookContentItems;
            }
        }

        [Transaction(TransactionMode.Requires)]
        public virtual BookPage GetPageByXmlId(string bookId, string pageXmlId)
        {
            using (var session = GetSession())
            {
                BookPage page = null;
                BookVersion version = null;

                var resultPage =session.QueryOver<BookPage>(() => page)
                    .JoinQueryOver(x => x.BookVersion, () => version)
                    .JoinQueryOver(x => x.Book)
                    .Where(book => book.Guid == bookId && version.Id == book.LastVersion.Id && page.XmlId == pageXmlId)
                    .SingleOrDefault<BookPage>();

               

                return resultPage;
            }
        }
    }
}