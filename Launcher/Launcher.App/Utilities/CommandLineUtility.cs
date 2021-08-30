using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Launcher.Utilities
{
    public static class CommandLineUtility
    {
        #region Constants

        private const string KeyValueSeparator = "=";

        #endregion

        // enumeration of possible arguments
        public const string Channel = "--channel";
        public const string PostUpdateDescription = "--postUpdateDescription";

        public static IImmutableDictionary<string, string> ParseCommandLineArguments(IEnumerable<string> args)
        {
            var keyValuePairs = args
                .Select(a => a.Split(KeyValueSeparator, StringSplitOptions.RemoveEmptyEntries))
                .Where(kv => kv.Length == 2)
                .ToImmutableDictionary(k => k.First(), v => v.Last());

            return keyValuePairs;
        }
    }
}