using System;
using System.Linq;

using Serilog.Core;
using Serilog.Events;

namespace Launcher.Helpers
{
    public class SerilogCustomExceptionEnricher : ILogEventEnricher
    {
        private readonly int _stackTraceDepth;

        #region Ctor

        public SerilogCustomExceptionEnricher(int stackTraceDepth)
        {
            _stackTraceDepth = stackTraceDepth;
        }

        #endregion

        #region ILogEventEnricher interface

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var exception = logEvent.Exception;
            var formattedException = string.Empty;

            if (exception != null)
            {
                // append message
                formattedException = exception.Message;

                if (exception.StackTrace != null)
                {
                    var reducedStackTraceItems = exception.StackTrace
                        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                        .Take(_stackTraceDepth);
                    var reducedStackTrace = string.Join(Environment.NewLine, reducedStackTraceItems);

                    // append stack trace
                    formattedException = $"{formattedException}{Environment.NewLine}{reducedStackTrace}";
                }
            }

            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(
                "customException", formattedException));
        }

        #endregion
    }
}