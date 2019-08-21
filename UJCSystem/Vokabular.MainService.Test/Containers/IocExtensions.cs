using System;
using System.IO;
using AutoMapper;
using DryIoc;
using Microsoft.Extensions.Configuration;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using Vokabular.DataEntities;
using Vokabular.Shared.Options;

namespace Vokabular.MainService.Test.Containers
{
    public static class IocExtensions
    {
        public static void InitAutoMapper(this DryIocContainer container)
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
    }
}
