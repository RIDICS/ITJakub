using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.Shared;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Containers.Installers
{
    public class NHibernateInstaller : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            var connectionString = ApplicationConfig.Configuration.GetConnectionString("DefaultConnection");
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