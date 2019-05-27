using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Extensions.DependencyInjection;
using ServiceDescriptor = Microsoft.Extensions.DependencyInjection.ServiceDescriptor;

namespace Vokabular.Shared.WcfService
{
    /// <summary>
    /// Code from: https://github.com/volosoft/castle-windsor-ms-adapter
    /// modified for WCF
    /// </summary>
    public static class WindsorRegistrationExtensions
    {
        public static void AddServicesCollection(this IWindsorContainer container, IServiceCollection services)
        {
            foreach (var serviceDescriptor in services)
            {
                if (serviceDescriptor.ImplementationInstance == container)
                {
                    //Already registered before
                    continue;
                }

                RegisterServiceDescriptor(container, serviceDescriptor);
            }
        }

        private static void RegisterServiceDescriptor(IWindsorContainer container, ServiceDescriptor serviceDescriptor)
        {
            // MS allows the same type to be registered multiple times.
            // Castle Windsor throws an exception in that case - it requires an unique name.
            // For that reason, we use Guids to ensure uniqueness.
            string uniqueName = serviceDescriptor.ServiceType.FullName + "_" + Guid.NewGuid();

            // The IsDefault() calls are important because this makes sure that the last service
            // is returned in case of multiple registrations. This is by design in the MS library:
            // https://github.com/aspnet/DependencyInjection/blob/dev/src/Microsoft.Extensions.DependencyInjection.Specification.Tests/DependencyInjectionSpecificationTests.cs#L254

            if (serviceDescriptor.ImplementationType != null)
            {
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .ImplementedBy(serviceDescriptor.ImplementationType)
                        .ConfigureLifecycle(serviceDescriptor.Lifetime));
            }
            else if (serviceDescriptor.ImplementationFactory != null)
            {
                var serviceDescriptorRef = serviceDescriptor;
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .UsingFactoryMethod(c => serviceDescriptorRef.ImplementationFactory(c.Resolve<IServiceProvider>()))
                        .ConfigureLifecycle(serviceDescriptor.Lifetime)
                    );
            }
            else
            {
                container.Register(
                    Component.For(serviceDescriptor.ServiceType)
                        .Named(uniqueName)
                        .IsDefault()
                        .Instance(serviceDescriptor.ImplementationInstance)
                        .ConfigureLifecycle(serviceDescriptor.Lifetime)
                    );
            }
        }

        private static ComponentRegistration<object> ConfigureLifecycle(this ComponentRegistration<object> registrationBuilder, ServiceLifetime serviceLifetime)
        {
            switch (serviceLifetime)
            {
                case ServiceLifetime.Transient:
                    registrationBuilder.LifestyleTransient();
                    break;
                case ServiceLifetime.Scoped:
                    registrationBuilder.LifestylePerWebRequest();
                    break;
                case ServiceLifetime.Singleton:
                    registrationBuilder.LifestyleSingleton();
                    break;
                default:
                    throw new InvalidOperationException("Unknown ServiceLifetime: " + serviceLifetime);
            }

            return registrationBuilder;
        }
    }
}
