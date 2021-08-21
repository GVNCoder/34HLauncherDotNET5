using System.Windows;
using System.Windows.Controls;

namespace Launcher.AttachedProperties
{
    public class ApplicationNavigation
    {
        public static readonly DependencyProperty SaveInHistoryProperty = DependencyProperty.RegisterAttached(
            "SaveInHistory", typeof(bool), typeof(ApplicationNavigation), new PropertyMetadata(false));

        public static void SetSaveInHistory(Page element, bool value)
        {
            element.SetValue(SaveInHistoryProperty, value);
        }

        public static bool GetSaveInHistory(Page element)
        {
            return (bool) element.GetValue(SaveInHistoryProperty);
        }
    }
}