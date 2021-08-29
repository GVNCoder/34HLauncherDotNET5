using System.Windows;
using System.Windows.Input;

using Launcher.Commands;

using Zlo4NET.Api.Service;

namespace Launcher.ViewModels
{
    public class NavigationPanelViewModel : DependencyObject
    {
        private readonly IZConnection _connection;

        #region Ctor

        public NavigationPanelViewModel(
            IZConnection connection)
        {
            _connection = connection;

            // create commands
            ViewLoadedCommand = new RelayCommand<object>(_ViewLoadedExecuteCommand);
            ViewUnloadedCommand = new RelayCommand<object>(_ViewUnloadedExecuteCommand);
        }

        #endregion

        #region Dependency properties

        public bool IsPanelEnabled
        {
            get => (bool)GetValue(IsPanelEnabledProperty);
            set => SetValue(IsPanelEnabledProperty, value);
        }
        public static readonly DependencyProperty IsPanelEnabledProperty =
            DependencyProperty.Register("IsPanelEnabled", typeof(bool), typeof(NavigationPanelViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(object parameter)
        {
            // track connection changes
            _connection.ConnectionChanged += (sender, args) => Dispatcher.Invoke(() => IsPanelEnabled = args.IsConnected);
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
        }

        #endregion
    }
}