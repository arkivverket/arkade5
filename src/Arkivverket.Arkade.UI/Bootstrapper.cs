using System.Windows;
using Arkivverket.Arkade.UI.Views;
using Arkivverket.Arkade.Util;
using Autofac;
using Prism.Autofac;
using Prism.Modularity;

namespace Arkivverket.Arkade.UI
{
    public class Bootstrapper : AutofacBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog) ModuleCatalog;
            catalog.AddModule(typeof(ModuleAModule));
        }

        protected override void ConfigureContainerBuilder(ContainerBuilder builder)
        {
            base.ConfigureContainerBuilder(builder);
            builder.RegisterModule(new ArkadeAutofacModule());
        }
    }


    /*   public class Bootstrapper : UnityBootstrapper
       {

           protected override DependencyObject CreateShell()
           {
               return Container.Resolve<MainWindow>();
           }

           protected override void InitializeShell()
           {
               Application.Current.MainWindow.Show();
           }

           protected override void ConfigureContainer()
           {
               base.ConfigureContainer();
               Container.RegisterTypeForNavigation<View000Debug>("View000Debug");
               Container.RegisterTypeForNavigation<View100Status>("View100Status");

               ILogService logService = new RandomLogService();
               Container.RegisterInstance(logService);

           }

           protected override void ConfigureModuleCatalog()
           {
               ModuleCatalog catalog = (ModuleCatalog)ModuleCatalog;
               catalog.AddModule(typeof(ModuleAModule));
           }


       }

       public static class UnityExtensons
       {
           public static void RegisterTypeForNavigation<T>(this IUnityContainer container, string name)
           {
               container.RegisterType(typeof(object), typeof(T), name);
           }
       }
       */
}