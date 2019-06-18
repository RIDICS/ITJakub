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
using Vokabular.DataEntities;
using Vokabular.Shared;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
{
    public static class NHibernateMainInstaller
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void AddNHibernateDefaultDatabase(this IContainer container)
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
                .AddAssembly(typeof(DataEntitiesContainerRegistration).Assembly);

            try
            {
                var sessionFactory = cfg.BuildSessionFactory();

                container.UseInstance(cfg, serviceKey: IocServiceKeys.Main);

                container.UseInstance(sessionFactory, serviceKey: IocServiceKeys.Main);

                container.Register<UnitOfWork>(Reuse.Scoped, Made.Of(() => new UnitOfWork(Arg.Of<ISessionFactory>(IocServiceKeys.Main))), serviceKey: IocServiceKeys.Main);
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