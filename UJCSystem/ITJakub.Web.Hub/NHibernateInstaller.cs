using System;
using System.Data.SqlClient;
using System.Reflection;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;
using Scalesoft.Localization.Database.NHibernate;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub
{
    public class NHibernateInstaller : IContainerInstaller
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Install(IServiceCollection services)
        {
            var connectionString =
                ApplicationConfig.Configuration.GetConnectionString(SettingKeys.WebConnectionString) ??
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
                });

            cfg.AddMapping(GetMappings());

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

        private HbmMapping GetMappings()
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(NHibernateDatabaseConfiguration.GetMappings()); // Localization library mappings

            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            return mapping;
        }
    }
}