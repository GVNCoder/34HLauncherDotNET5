using Launcher.Core.Models;
using Launcher.Core.Services;

using System;
using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace Launcher.Core.Data
{
    public class UpdateChecker : IUpdateChecker
    {
        #region IUpdateChecker

        public event EventHandler<CheckForUpdateEventArgs> OnCheckForUpdateCompleted;
        public event EventHandler<ErrorOccuredEventArgs> OnError;

        public async Task CheckForUpdatesAsync(string checkLink)
        {
            var updateDescription = default(UpdateDescription);

            try
            {
                using (var webClient = new WebClient())
                {
                    var contents = await webClient.DownloadStringTaskAsync(checkLink);

                    // parse update description
                    updateDescription = JsonConvert.DeserializeObject<UpdateDescription>(contents);
                }
            }
            catch (WebException webException)
            {
                _RaiseOnError($"{nameof(CheckForUpdatesAsync)}", webException);
            }

            _RaiseOnCheckForUpdateCompleted(updateDescription);
        }

        #endregion

        #region Private helpers

        private void _RaiseOnCheckForUpdateCompleted(UpdateDescription updateDescription)
            => OnCheckForUpdateCompleted?.Invoke(this, new CheckForUpdateEventArgs(updateDescription));

        private void _RaiseOnError(string message, Exception exception = null)
            => OnError?.Invoke(this, new ErrorOccuredEventArgs(message, exception));

        #endregion
    }
}