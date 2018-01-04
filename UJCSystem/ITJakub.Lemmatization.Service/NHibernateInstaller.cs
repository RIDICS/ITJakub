using System;
using System.Configuration;
using ITJakub.Lemmatization.DataEntities.Entities;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;
using Configuration = NHibernate.Cfg.Configuration;

namespace ITJakub.Lemmatization.Service
{
    public class NHibernateInstaller : IContainerInstaller
    {
        public void Install(IIocContainer container)
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
                .AddAssembly(typeof(Token).Assembly);

            var sessionFactory = cfg.BuildSessionFactory();

            container.AddInstance(cfg);

            container.AddInstance(sessionFactory);
        }
    }
}