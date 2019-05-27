using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.Shared.Container
{
    public interface IContainerInstaller
    {
        void Install(IServiceCollection services);
    }
}