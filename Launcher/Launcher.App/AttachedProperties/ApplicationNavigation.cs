using System.Windows;
using System.Windows.Controls;

namespace Launcher.AttachedProperties
{
    public class ApplicationNavigation
    {
        public static readonly DependencyProperty HistoryEnabledProperty = DependencyProperty.RegisterAttached(
            "HistoryEnabled", typeof(bool), typeof(ApplicationNavigation), new PropertyMetadata(true));

        public static void SetHistoryEnabled(Page element, bool value)
        {
            element.SetValue(HistoryEnabledProperty, value);
        }

        public static bool GetHistoryEnabled(Page element)
        {
            return (bool) element.GetValue(HistoryEnabledProperty);
        }
    }
}