using System;
using System.Net;
using System.Threading.Tasks;

using Launcher.Core.Models;
using Newtonsoft.Json;

namespace Launcher.Core.Data
{
    public partial class ApplicationUpdater
    {
        private UpdateDescription _updateDescription;

        #region IApplicationUpdater

        public event EventHandler<CheckForUpdateEventArgs> OnCheckForUpdateCompleted;

        public async Task CheckForUpdatesAsync(string checkLink, Version currentVersion)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var contents = await webClient.DownloadStringTaskAsync(checkLink);
                
                    // parse update description
                    _updateDescription = JsonConvert.DeserializeObject<UpdateDescription>(contents);
                }
            }
            catch (WebException webException)
            {
                _RaiseOnError($"{nameof(CheckForUpdatesAsync)}", webException);
            }

            if (_updateDescription != null && _updateDescription.LatestVersion > currentVersion)
            {
                _RaiseOnCheckForUpdateCompleted(true, _updateDescription);
            }
            else
            {
                _RaiseOnCheckForUpdateCompleted(false, null);
            }
        }

        #endregion

        #region Private helpers

        private void _RaiseOnCheckForUpdateCompleted(bool isUpdateAvailable, UpdateDescription updateDescription)
            => OnCheckForUpdateCompleted?.Invoke(this, new CheckForUpdateEventArgs(isUpdateAvailable, updateDescription));

        #endregion
    }
}