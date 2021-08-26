using System.Windows;

using Launcher.Models;
using Launcher.Services;

namespace Launcher.ViewModels
{
    public class WindowContentViewModel : DependencyObject
    {
        private readonly INavigationService _navigationService;

        #region Ctor

        public WindowContentViewModel(
            INavigationService navigationService)
        {
            _navigationService = navigationService;

            // track navigation
            _navigationService.Navigated += _OnNavigated;
        }

        #endregion

        #region Dependency properties

        public object CurrentContent
        {
            get => (object) GetValue(CurrentContentProperty);
            set => SetValue(CurrentContentProperty, value);
        }
        public static readonly DependencyProperty CurrentContentProperty =
            DependencyProperty.Register("CurrentContent", typeof(object), typeof(WindowContentViewModel), new PropertyMetadata(null));

        #endregion

        #region Private helpers

        private void _OnNavigated(object sender, NavigatedEventArgs e)
        {
            // set window content
            CurrentContent = e.Content;
        }

        #endregion
    }
}