using System;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.Shared.Container
{
    public interface IContainer : IDisposable
    {
        void AddSingleton<TService>() where TService : class;
        void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddTransient<TService>() where TService : class;
        void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddPerWebRequest<TService>() where TService : class;
        void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService;
        void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class;
        void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService;
        void Install<T>() where T : IContainerInstaller;
        void Install(params IContainerInstaller[] installers);
        T Resolve<T>();
        T[] ResolveAll<T>();
        IServiceProvider CreateServiceProvider(IServiceCollection services);
    }
}