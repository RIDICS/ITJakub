using System;
using System.Data.SqlClient;
using System.Reflection;
using DryIoc;
using log4net;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
{
    public static class NHibernateForumInstaller
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void AddNHibernateForumDatabase(this IContainer container)
        {
            var connectionString =
                ApplicationConfig.Configuration.GetConnectionString(SettingKeys.ForumConnectionString) ??
                throw new ArgumentException("Connection string not found");

            var cfg = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionString = connectionString;
                    db.Dialect<MsSql2008Dialect>();
                    db.Driver<SqlClientDriver>();
                    db.ConnectionProvider<DriverConnectionProvider>();
                    db.BatchSize = 5000;
                    db.Timeout = byte.MaxValue;
                    //db.LogFormattedSql = true;
                    //db.LogSqlInConsole = true;                     
                })
                .AddAssembly(typeof(ForumRepository).Assembly);

            try
            {
                var sessionFactory = cfg.BuildSessionFactory();

                container.UseInstance(cfg, serviceKey: "forum");

                container.UseInstance(sessionFactory, serviceKey: "forum");

                container.Register<UnitOfWork>(Reuse.InWebRequest, Made.Of(() => new UnitOfWork(Arg.Of<ISessionFactory>("forum"))), serviceKey: "forum");
            }
            catch (SqlException e)
            {
                if (m_log.IsFatalEnabled)
                    m_log.Fatal("Error init relational database connection", e);

                throw;
            }
        }
    }
}