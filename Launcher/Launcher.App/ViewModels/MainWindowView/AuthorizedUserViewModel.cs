using System.Windows;
using Zlo4NET.Api.Service;

namespace Launcher.ViewModels
{
    public class AuthorizedUserViewModel : DependencyObject
    {
        #region Ctor

        public AuthorizedUserViewModel(IZConnection connection)
        {
            // track connection changes
            connection.ConnectionChanged += (sender, args) =>
                Dispatcher.Invoke(() => UserName = args.AuthorizedUser?.UserName);
        }

        #endregion

        #region Depenency properties

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(AuthorizedUserViewModel), new PropertyMetadata(null));

        #endregion
    }
}