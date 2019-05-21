using System;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

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

        private static IConfigurationRoot GetConfiguration()
        {
            var globalConfiguration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("globalsettings.json").Build();

            var secretSettingsPath = globalConfiguration["SecretSettingsPath"];

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(secretSettingsPath, "ITJakub.Secrets.json"), optional: true)
                .AddEnvironmentVariables()
                .Build();

            return config;
        }

        public static void InitNHibernate(this IIocContainer container)
        {
            var config = GetConfiguration();
            
            var connectionString = config.GetConnectionString(SettingKeys.TestDbConnectionString) ?? throw new ArgumentException("Connection string not found");

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

            var sessionFactory = cfg.BuildSessionFactory();

            container.AddInstance(cfg);

            container.AddInstance(sessionFactory);
        }
    }
}
