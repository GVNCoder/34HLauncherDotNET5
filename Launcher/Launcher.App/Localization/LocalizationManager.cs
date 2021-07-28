using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Launcher.Localization
{
    public static class LocalizationManager
    {
        #region Constants

        private const string DefaultLocalizationKey = "en-US";

        #endregion

        private static string _currentLocalizationKey = DefaultLocalizationKey;
        private static LocalizationResourceDictionary _currentLocalizationDictionary;
        private static ResourceDictionary _rootDictionary;

        #region Public interface

        public static IEnumerable<string> Localizations { get; } = new[] { "en-US" };

        /// <exception cref="InvalidOperationException"></exception>
        public static void Init(ResourceDictionary rootDictionary)
        {
            // this potential error should be resolved at development time
            _currentLocalizationDictionary = rootDictionary.MergedDictionaries
                .OfType<LocalizationResourceDictionary>()
                .First();

            _rootDictionary = rootDictionary;
        }

        /// <exception cref="ArgumentException"></exception>
        public static void ApplyLocalization(string localizationKey)
        {
            // this potential error should be resolved at development time
            if (_ValidateLocalizationKey(localizationKey))
            {
                throw new ArgumentException("Requested localization not exists", nameof(localizationKey));
            }

            if (localizationKey == _currentLocalizationKey)
            {
                return;
            }

            var newLocaleDictionary = new LocalizationResourceDictionary
                { Source = new Uri($"pack://application:,,,/Localization/Resources/{localizationKey}.xaml", UriKind.Absolute) };
            var countOfDictionaries = _rootDictionary.MergedDictionaries.Count;

            _rootDictionary.MergedDictionaries.Insert(countOfDictionaries, newLocaleDictionary);
            _rootDictionary.MergedDictionaries.Remove(_currentLocalizationDictionary);

            _currentLocalizationDictionary = newLocaleDictionary;
            _currentLocalizationKey = localizationKey;
        }

        #endregion

        #region Private helpers

        private static bool _ValidateLocalizationKey(string localizationKey)
        {
            return Localizations.All(l => l != localizationKey);
        }

        #endregion
    }
}