using System;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.Shared.Options;
using Configuration = NHibernate.Cfg.Configuration;

namespace ITJakub.FileProcessing.Service
{
    public class NHibernateInstaller : IContainerInstaller
    {
        public void Install(IServiceCollection services)
        {
            var connectionString = ConfigurationManager.AppSettings[SettingKeys.MainConnectionString] ?? throw new ArgumentException("Connection string not found");

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

            var sessionFactory = cfg.BuildSessionFactory();

            services.AddSingleton(cfg);

            services.AddSingleton(sessionFactory);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}