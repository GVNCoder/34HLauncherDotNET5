using System;
using Microsoft.Extensions.DependencyInjection;

using Launcher.App.ViewModels;

namespace Launcher.ViewModels
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _container;

        #region Ctor

        public ViewModelLocator()
        {
            // populate internal state
            _container = LauncherApp.Container;
        }

        #endregion

        #region ViewModels fields

        public StartupWindowViewModel StartupWindowViewModel
            => _container.GetRequiredService<StartupWindowViewModel>();

        public MainWindowViewModel MainWindowViewModel
            => _container.GetRequiredService<MainWindowViewModel>();

        public WindowNonClientViewModel WindowNonClientViewModel
            => _container.GetRequiredService<WindowNonClientViewModel>();

        public WindowContentViewModel WindowContentViewModel
            => _container.GetRequiredService<WindowContentViewModel>();

        public NavigationPanelViewModel NavigationPanelViewModel
            => _container.GetRequiredService<NavigationPanelViewModel>();

        public LoginPageViewModel LoginPageViewModel
            => _container.GetRequiredService<LoginPageViewModel>();

        public AuthorizedUserViewModel AuthorizedUserViewModel
            => _container.GetRequiredService<AuthorizedUserViewModel>();

        #endregion
    }
}