using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Launcher.Themes
{
    public static class ThemeManager
    {
        #region Constants

        private const string DefaultThemeKey = "DefaultDark";

        #endregion

        private static string _currentThemeKey = DefaultThemeKey;
        private static ThemeResourceDictionary _currentThemeDictionary;
        private static ResourceDictionary _rootDictionary;

        #region Public interface

        public static IEnumerable<string> AvailableThemes { get; } = new[] { "DefaultDark" };

        /// <exception cref="InvalidOperationException"></exception>
        public static void Init(ResourceDictionary rootDictionary)
        {
            // this potential error should be resolved at development time
            _currentThemeDictionary = rootDictionary.MergedDictionaries
                .OfType<ThemeResourceDictionary>()
                .First();

            _rootDictionary = rootDictionary;
        }

        /// <exception cref="ArgumentException"></exception>
        public static void ApplyTheme(string themeKey)
        {
            // this potential error should be resolved at development time
            if (_ValidateThemeKey(themeKey))
            {
                throw new ArgumentException("Requested localization not exists", nameof(themeKey));
            }

            if (themeKey == _currentThemeKey)
            {
                return;
            }

            var newThemeDictionary = new ThemeResourceDictionary
                { Source = new Uri($"pack://application:,,,/Localization/Resources/{themeKey}.xaml", UriKind.Absolute) };
            var countOfDictionaries = _rootDictionary.MergedDictionaries.Count;

            _rootDictionary.MergedDictionaries.Insert(countOfDictionaries, newThemeDictionary);
            _rootDictionary.MergedDictionaries.Remove(_currentThemeDictionary);

            _currentThemeDictionary = newThemeDictionary;
            _currentThemeKey = themeKey;
        }

        #endregion

        #region Private helpers

        private static bool _ValidateThemeKey(string themeKey)
        {
            return AvailableThemes.All(l => l != themeKey);
        }

        #endregion
    }
}