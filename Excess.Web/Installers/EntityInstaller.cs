using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Excess.Web.Services;

namespace Excess.Web.Installers
{
    public class EntityInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IUserServices>().ImplementedBy<UserService>());
        }
    }
}