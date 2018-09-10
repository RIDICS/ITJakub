using System;
using System.Configuration;
using AutoMapper;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;
using Configuration = NHibernate.Cfg.Configuration;

namespace Vokabular.MainService.Test.Containers
{
    public static class IocExtensions
    {
        public static void InitAutoMapper(this IIocContainer container)
        {
            var profiles = container.ResolveAll<Profile>();

            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
        }

        public static void InitNHibernate(this IIocContainer container)
        {
            var connectionString = ConfigurationManager.AppSettings[SettingKeys.TestDbConnectionString] ?? throw new ArgumentException("Connection string not found");

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

            container.AddInstance(cfg);

            container.AddInstance(sessionFactory);
        }
    }
}
