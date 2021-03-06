﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Vokabular.DataEntities;
using Vokabular.ProjectImport.Shared.Options;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.DataEntities.UnitOfWork;
using ILoggerFactory = Microsoft.Extensions.Logging.ILoggerFactory;


namespace Vokabular.ProjectImport.Test.Mock
{
    public class MockIocContainer
    {
        public IServiceCollection ServiceCollection { get; }

        public MockIocContainer(bool initDatabase = false)
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddProjectImportServices();
            new DataEntitiesContainerRegistration().Install(ServiceCollection);
            ServiceCollection.AddScoped(serviceProvider => new UnitOfWorkProvider(serviceProvider.GetServices<IUnitOfWork>()
                .Select(x => new KeyValuePair<object, IUnitOfWork>(null, x))
                .ToList()));

            ServiceCollection.AddOptions();
            ServiceCollection.Configure(new Action<OaiPmhClientOption>(option =>
            {
                option.Delay = 5;
                option.DisableSslValidation = true;
            }));
            ServiceCollection.AddAutoMapper();
            MockLogging();

            if (initDatabase)
            {
                InitDatabase();                
            }
        }

        private void MockLogging()
        {
            ServiceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(NullLogger.Instance);
            ServiceCollection.AddSingleton(typeof(ILoggerFactory), loggerFactoryMock.Object);
        }

        private void InitDatabase()
        {  
            var connectionString = "Data Source=:memory:;Version=3;New=True;";

            var cfg = new Configuration()
                .DataBaseIntegration(db =>
                {
                    db.ConnectionString = connectionString;
                    db.Dialect<CustomSQLiteDialect>();
                    db.Driver<SQLite20Driver>();
                    db.ConnectionProvider<DriverConnectionProvider>();
                    db.BatchSize = 200;
                    db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;        
                    //db.LogFormattedSql = true;
                    //db.LogSqlInConsole = true;   
                })
                .AddAssembly(typeof(DataEntitiesContainerRegistration).Assembly);
           
            var sessionFactory = cfg.BuildSessionFactory();
            var session = sessionFactory.OpenSession();
            new SchemaExport(cfg).Execute(false, true, false,  session.Connection, null);
            session.Flush();

            var sessionFactoryMock = new Mock<ISessionFactory>();
            sessionFactoryMock.Setup(x => x.OpenSession()).Returns(session);
         
            ServiceCollection.AddSingleton(sessionFactoryMock.Object);

            ServiceCollection.RemoveAll<IUnitOfWork>();
            ServiceCollection.AddScoped<IUnitOfWork, MockUnitOfWork>();
            ServiceCollection.AddScoped<MockDataConstant>();
            ServiceCollection.AddScoped<MockDataManager>();
        }

        public IServiceProvider CreateServiceProvider()
        {
            var serviceProvider = ServiceCollection.BuildServiceProvider();
            return serviceProvider;
        }
    }
}