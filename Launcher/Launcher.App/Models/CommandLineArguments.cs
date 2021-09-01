using System.Collections.Generic;

namespace Launcher.Models
{
    public class CommandLineArguments
    {
        public IReadOnlyDictionary<string, string> KeyValueArguments { get; }
        public IReadOnlyCollection<string> ItselfValuedArguments { get; }
        public string OriginalCommandLine { get; }

        #region Ctor

        public CommandLineArguments(IReadOnlyDictionary<string, string> keyValueArguments, IReadOnlyCollection<string> itselfValuedArguments, string originalCommandLine)
        {
            KeyValueArguments = keyValueArguments;
            ItselfValuedArguments = itselfValuedArguments;
            OriginalCommandLine = originalCommandLine;
        }

        #endregion
    }
}