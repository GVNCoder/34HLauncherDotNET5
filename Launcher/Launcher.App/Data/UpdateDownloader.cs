using System;
using System.ComponentModel;
using System.Net;

using Launcher.Models;
using Launcher.Services;

namespace Launcher.Data
{
    public class UpdateDownloader : IUpdateDownloader
    {
        private string _destinationFilePath;

        #region IUpdateDownloader

        public event EventHandler<DownloadProgressEventArgs> OnUpdateDownloadProgress;
        public event EventHandler<DownloadCompletedEventArgs> OnUpdateDownloadCompleted;
        public event EventHandler<ErrorOccuredEventArgs> OnError;

        public void DownloadAsync(string downloadLink, string destinationPath)
        {
            _destinationFilePath = destinationPath;

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFileCompleted += _OnWebClientFileDownloadComplete;
                    webClient.DownloadProgressChanged += _OnWebClientDownloadProgressChanged;

                    webClient.DownloadFileAsync(new Uri(downloadLink), destinationPath);
                }
            }
            catch (WebException webException)
            {
                _RaiseOnError($"{nameof(DownloadAsync)}", webException);
                _RaiseOnUpdateDownloadCompleted(false);
            }
        }

        #endregion

        #region Private helpers

        private void _OnWebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _RaiseOnUpdateDownloadProgress(e.ProgressPercentage);
        }

        private void _OnWebClientFileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error != null)
            {
                _RaiseOnError($"{nameof(_OnWebClientFileDownloadComplete)} Operation canceled or error occured", e.Error);
                _RaiseOnUpdateDownloadCompleted(false);
            }
            else
            {
                _RaiseOnUpdateDownloadCompleted(true, _destinationFilePath);
            }
        }

        private void _RaiseOnUpdateDownloadCompleted(bool successful, string downloadedFilePath = null)
            => OnUpdateDownloadCompleted?.Invoke(this, new DownloadCompletedEventArgs(successful, downloadedFilePath));

        private void _RaiseOnUpdateDownloadProgress(int percentProgress)
            => OnUpdateDownloadProgress?.Invoke(this, new DownloadProgressEventArgs(percentProgress));

        private void _RaiseOnError(string message, Exception exception = null)
            => OnError?.Invoke(this, new ErrorOccuredEventArgs(message, exception));

        #endregion
    }
}