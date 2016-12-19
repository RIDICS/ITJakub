using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.MainService.Container
{
    public interface IContainer
    {
        void AddSingleton<TService>() where TService : class;
        void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddTransient<TService>() where TService : class;
        void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddPerWebRequest<TService>() where TService : class;
        void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class;
        void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService;
        IServiceProvider CreateServiceProvider(IServiceCollection services);
    }
}