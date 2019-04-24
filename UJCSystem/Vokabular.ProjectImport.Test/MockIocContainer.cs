﻿using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Moq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Vokabular.DataEntities;
using Vokabular.DataEntities.Database.Daos;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Options;
using ILoggerFactory = NHibernate.ILoggerFactory;

namespace Vokabular.ProjectImport.Test
{
    public class MockIocContainer
    {
        public readonly IServiceCollection ServiceCollection;

        public MockIocContainer(bool initDatabase = false)
        {
            ServiceCollection = new ServiceCollection();
            ServiceCollection.AddProjectImportServices();
            ServiceCollection.AddDataEntitiesServices();
            ServiceCollection.AddOptions();
            ServiceCollection.Configure(new Action<OaiPmhClientOption>(option => option.Delay = 5));
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
            ServiceCollection.AddSingleton(typeof(ILoggerFactory), loggerFactoryMock.Object);

            var logger = new Mock<ILogger<ProjectImportBackgroundService>>();
            ServiceCollection.AddSingleton(typeof(ILogger<ProjectImportBackgroundService>), logger.Object);
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
                })
                .AddAssembly(typeof(NHibernateDao).Assembly);
           
            var sessionFactory = cfg.BuildSessionFactory();
            var session = sessionFactory.OpenSession();
            new SchemaExport(cfg).Execute(false, true, false,  session.Connection, null);
            session.Flush();

            var sessionFactoryMock = new Mock<ISessionFactory>();
            sessionFactoryMock.Setup(x => x.OpenSession()).Returns(session);
         
            ServiceCollection.AddSingleton(sessionFactoryMock.Object);

            ServiceCollection.RemoveAll<IUnitOfWork>();
            ServiceCollection.AddScoped<IUnitOfWork, MockUnitOfWork>();
        }

        private void InitAutoMapper(IServiceProvider serviceProvider)
        {
            var profiles = serviceProvider.GetServices<Profile>();

            Mapper.Reset();
            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
        }

        public IServiceProvider CreateServiceProvider()
        {
            var serviceProvider =  ServiceCollection.BuildServiceProvider();
            InitAutoMapper(serviceProvider);
            return serviceProvider;
        }
    }
}