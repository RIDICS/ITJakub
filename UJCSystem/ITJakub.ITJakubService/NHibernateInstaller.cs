using System.Configuration;
using Castle.Facilities.NHibernate;
using Castle.Transactions;
using ITJakub.DataEntities.Database.Daos;
using Jewelery;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Configuration = NHibernate.Cfg.Configuration;

namespace ITJakub.ITJakubService
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
                var cfg = new Configuration()
                    .DataBaseIntegration(db =>
                    {
                        db.ConnectionString = ConfigurationManager.AppSettings[SettingKeys.DefaultConnectionString]
                            .GetStringOrThrowArgumentException("Connection string not found");
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