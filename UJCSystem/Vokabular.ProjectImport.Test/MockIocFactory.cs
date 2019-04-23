using System;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Driver;
using Vokabular.DataEntities.Database.Daos;

namespace Vokabular.ProjectImport.Test
{
    public static class MockIocFactory
    {
        public static IServiceProvider CreateMockIocContainer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddProjectImportServices();

            var connectionString = "Data Source=:memory:;Version=3;New=True;";

            var cfg = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionString = connectionString;
                    db.Dialect<CustomSQLiteDialect>();
                    db.Driver<SQLite20Driver>();
                    db.ConnectionProvider<DriverConnectionProvider>();
                    db.BatchSize = 5000;
                    db.Timeout = byte.MaxValue;
                    db.SchemaAction = SchemaAutoAction.Create;
                    //db.LogFormattedSql = true;
                    //db.LogSqlInConsole = true;                     
                })
                .AddAssembly(typeof(NHibernateDao).Assembly);

            var sessionFactory = cfg.BuildSessionFactory();

            serviceCollection.AddSingleton(cfg);
            serviceCollection.AddSingleton(sessionFactory);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}