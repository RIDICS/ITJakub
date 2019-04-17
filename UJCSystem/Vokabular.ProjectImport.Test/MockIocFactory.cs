using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.ProjectImport.Test
{
    public static class MockIocFactory
    {
        public static IServiceProvider CreateMockIocContainer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddProjectImportServices();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
