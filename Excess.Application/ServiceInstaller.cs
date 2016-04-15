using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Excess.Services;

namespace Excess
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ITranslationService>().ImplementedBy<TranslationService>(),
                Component.For<IProjectManager>().ImplementedBy<ProjectService>());
        }
    }
}