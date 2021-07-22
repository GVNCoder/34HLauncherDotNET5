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

        #endregion
    }
}