using System;
using ITJakub.Web.DataEntities.Database.Daos;
using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub.AppStart
{
    public class NHibernateInstaller : IContainerInstaller
    {
        public void Install(IIocContainer container)
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
                })
                .AddAssembly(typeof(NHibernateDao).Assembly);

            var sessionFactory = cfg.BuildSessionFactory();
            
            container.AddInstance(cfg);

            container.AddInstance(sessionFactory);
        }
    }
}