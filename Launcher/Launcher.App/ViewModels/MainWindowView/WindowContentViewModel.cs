using System.Windows;

namespace Launcher.ViewModels
{
    public class WindowContentViewModel : DependencyObject
    {
        #region Ctor

        public WindowContentViewModel()
        {
            
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
    }
}