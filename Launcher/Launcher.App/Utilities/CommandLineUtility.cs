using System;
using System.Linq;

using Launcher.Models;

namespace Launcher.Utilities
{
    public static class CommandLineUtility
    {
        #region Constants



        #endregion

        #region Possible argument keys

        public const string DevChannel = "-dev";
        public const string PostUpdateDescription = "-postUpdDesc";

        #endregion

        public static CommandLineArguments ParseCommandLineArguments(string[] args, char valueSeparator = '=')
        {
            // parse arguments
            var argumentsKeyValue = args
                .Select(a => a.Split(valueSeparator, StringSplitOptions.RemoveEmptyEntries))
                .ToArray();

            // get itself-valued arguments
            var itselfValuedArguments = argumentsKeyValue
                .Where(i => i.Length == 1)
                .Select(a => a.First())
                .ToList();

            // get key-value pair arguments
            var keyValuePairArguments = argumentsKeyValue
                .Where(i => i.Length == 2)
                .ToDictionary(k => k.First(), v => v.Last());

            // create merged arguments string
            var commandLine = string.Join(' ', args);

            return new CommandLineArguments(keyValuePairArguments, itselfValuedArguments, commandLine);
        }
    }
}