using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.Marc21ProjectParser.Test
{
    public static class MockIocFactory
    {
        public static IServiceProvider CreateMockIocContainer()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddMarc21ProjectParsingServices();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
