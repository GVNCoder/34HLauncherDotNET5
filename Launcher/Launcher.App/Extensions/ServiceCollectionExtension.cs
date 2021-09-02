using Launcher.App.ViewModels;
using Launcher.Data;
using Launcher.Services;
using Launcher.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Zlo4NET.Api.Service;
using Zlo4NET.Data;

namespace Launcher.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void AddZloAPI(this IServiceCollection serviceCollection)
        {
            var instance = ZApi.Instance;

            serviceCollection.AddSingleton<IZApi>(instance);
            serviceCollection.AddSingleton<IZConnection>(instance.GetApiConnection());
            serviceCollection.AddSingleton<IZGameFactory>(instance.GetGameFactory());
            serviceCollection.AddSingleton<IZInjector>(instance.GetInjectorService());
            serviceCollection.AddSingleton<IZInstalledGames>(instance.GetInstalledGamesService());
            serviceCollection.AddSingleton<IZPlayerStats>(instance.GetPlayerStatsService());
        }

        public static void AddViewModels(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<StartupWindowViewModel>();
            serviceCollection.AddScoped<MainWindowViewModel>();
            serviceCollection.AddScoped<WindowNonClientViewModel>();
            serviceCollection.AddScoped<WindowContentViewModel>();
            serviceCollection.AddScoped<NavigationPanelViewModel>();
            serviceCollection.AddScoped<LoginPageViewModel>(); // TODO: Scoped ?
            serviceCollection.AddScoped<AuthorizedUserViewModel>();
        }

        public static void AddUpdaterServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IUpdateChecker, UpdateChecker>();
            serviceCollection.AddTransient<IUpdateDownloader, UpdateDownloader>();
            serviceCollection.AddTransient<IUpdateInstaller, UpdateInstaller>();
        }
    }
}