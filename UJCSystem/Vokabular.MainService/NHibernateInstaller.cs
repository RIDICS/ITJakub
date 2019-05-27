using System;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
{
    public class NHibernateInstaller : IContainerInstaller
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Install(IServiceCollection services)
        {
            var connectionString =
                ApplicationConfig.Configuration.GetConnectionString(SettingKeys.MainConnectionString) ??
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
                .AddAssembly(typeof(NHibernateDao).Assembly);

            try
            {
                var sessionFactory = cfg.BuildSessionFactory();

                services.AddSingleton(cfg);

                services.AddSingleton(sessionFactory);
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