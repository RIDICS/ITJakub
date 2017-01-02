using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Container.Installers
{
    public class NHibernateInstaller : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            var connectionString = "Server=localhost;Database=ITJakubDB;User Id=admin;Password=***REMOVED***;";
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