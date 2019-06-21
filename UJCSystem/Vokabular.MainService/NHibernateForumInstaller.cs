using System;
using System.Data.SqlClient;
using System.Reflection;
using DryIoc;
using log4net;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
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
                    db.Dialect<MsSql2012Dialect>();
                    db.Driver<SqlClientDriver>();
                    db.ConnectionProvider<DriverConnectionProvider>();
                    db.BatchSize = 5000;
                    db.Timeout = byte.MaxValue;
                    //db.LogFormattedSql = true;
                    //db.LogSqlInConsole = true;                     
                });

            cfg.AddMapping(GetMapping());

            try
            {
                var sessionFactory = cfg.BuildSessionFactory();

                container.UseInstance(cfg, serviceKey: IocServiceKeys.Forum);

                container.UseInstance(sessionFactory, serviceKey: IocServiceKeys.Forum);

                container.Register<IUnitOfWork>(Reuse.Scoped, Made.Of(() => new UnitOfWork(Arg.Of<ISessionFactory>(IocServiceKeys.Forum))), serviceKey: IocServiceKeys.Forum);
            }
            catch (SqlException e)
            {
                if (m_log.IsFatalEnabled)
                    m_log.Fatal("Error init relational database connection", e);

                throw;
            }
        }

        private static HbmMapping GetMapping()
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetAssembly(typeof(ForumRepository)).GetExportedTypes());
            return mapper.CompileMappingForAllExplicitlyAddedEntities();
        }
    }
}