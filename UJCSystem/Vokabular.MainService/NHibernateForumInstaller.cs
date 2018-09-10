using System;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.ForumSite.DataEntities.Database.Repositories;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
{
    public class NHibernateForumInstaller : IContainerInstaller
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Install(IIocContainer container)
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

                container.AddInstance(cfg, "forum");

                container.AddInstance(sessionFactory, "forum");

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