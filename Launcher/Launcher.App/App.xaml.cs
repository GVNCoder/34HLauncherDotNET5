using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using Launcher.App.ViewModels;
using Launcher.Core.Data;
using Launcher.Core.Services;

namespace Launcher
{
    public partial class LauncherApp : Application
    {
        #region Properties

        public static IServiceProvider Container { get; private set; }

        #endregion

        #region Ctor

        public LauncherApp()
        {
            // configure IoC container
            var serviceCollection = new ServiceCollection();

            // register services
            serviceCollection.AddTransient<IUpdateChecker, UpdateChecker>();
            serviceCollection.AddTransient<IUpdateDownloader, UpdateDownloader>();

            // register viewModels
            serviceCollection.AddScoped<StartupWindowViewModel>();

            // create container
            Container = serviceCollection.BuildServiceProvider();
        }

        #endregion
    }
}
