using System;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Excess.Web.App_Start
{
    public class ContainerBootstrapper : IContainerAccessor, IDisposable
    {
        private ContainerBootstrapper(IWindsorContainer container)
        {
            Container = container;
        }

        public IWindsorContainer Container { get; }

        public void Dispose()
        {
            Container.Dispose();
        }

        public static ContainerBootstrapper Bootstrap()
        {
            var container = new WindsorContainer().
                Install(FromAssembly.This()).
                Install(FromAssembly.Containing(typeof(ITranslationService)));
            return new ContainerBootstrapper(container);
        }
    }
}