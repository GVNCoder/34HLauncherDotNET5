using System;
using System.Net;
using System.Threading.Tasks;

using Launcher.Core.Models;
using Launcher.Core.Services;

using Newtonsoft.Json;

namespace Launcher.Core.Data
{
    public class UpdateChecker : IUpdateChecker
    {
        #region IUpdaterChecker

        public event EventHandler<UpdateCheckErrorEventArgs> OnUpdateCheckError;
        public UpdateDescription UpdateDescription { get; private set; }

        public async Task<bool> IsNewVersionAvailableAsync(string checkLink, Version currentVersion)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    var contents = await webClient.DownloadStringTaskAsync(checkLink);
                
                    // parse update description
                    UpdateDescription = JsonConvert.DeserializeObject<UpdateDescription>(contents);
                }
            }
            catch (WebException webException)
            {
                _OnUpdateCheckError();
            }

            if (UpdateDescription == null)
            {
                return false;
            }

            return UpdateDescription.LatestVersion > currentVersion;
        }

        #endregion

        #region Private helpers

        private void _OnUpdateCheckError()
            => OnUpdateCheckError?.Invoke(this, new UpdateCheckErrorEventArgs());

        #endregion
    }
}