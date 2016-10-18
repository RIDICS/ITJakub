using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.Web.DataEntities.Database.Daos;
using Microsoft.Extensions.Configuration;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;

namespace ITJakub.Web.Hub.Installers
{
    public class NHibernateInstaller : INHibernateInstaller
    {
        private readonly IConfigurationPersister m_persister;

        public NHibernateInstaller(IConfigurationPersister persister)
        {
            m_persister = persister;
        }

        public bool IsDefault
        {
            get { return true; }
        }

        public string SessionFactoryKey
        {
            get { return "nhibernate.default"; }
        }

        public Maybe<IInterceptor> Interceptor
        {
            get { return Maybe.None<IInterceptor>(); }
        }

        public Configuration Config
        {
            get
            {
                var connectionString = Startup.Configuration.GetConnectionString("DefaultConnection");
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
                    .AddAssembly(typeof(NHibernateTransactionalDao).Assembly);
                return cfg;
            }
        }

        public void Registered(ISessionFactory factory)
        {
        }

        public Configuration Deserialize()
        {
            //if (File.Exists("serialized.dat"))
            //{
            //    return m_persister.ReadConfiguration("serialized.dat");
            //}
            return null;
        }

        public void Serialize(Configuration configuration)
        {
            //m_persister.WriteConfiguration("serialized.dat", configuration);
        }

        public void AfterDeserialize(Configuration configuration)
        {
        }
    }
}