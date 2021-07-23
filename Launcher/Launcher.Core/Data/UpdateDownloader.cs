using Launcher.Core.Models;
using Launcher.Core.Services;

using System;
using System.ComponentModel;
using System.Net;

namespace Launcher.Core.Data
{
    public class UpdateDownloader : IUpdateDownloader
    {
        private string _destinationFilePath;

        #region IUpdateDownloader

        public event EventHandler<UpdateDownloadCompletedEventArgs> OnDownloadCompleted;
        public event EventHandler<UpdateDownloadErrorEventArgs> OnDownloadError;
        public event EventHandler<UpdateDownloadProgressEventArgs> OnDownloadProgress;

        public void Download(string downloadLink, string destinationPath)
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
                _RaiseOnDownloadError($"{nameof(Download)}", webException);
            }
        }

        #endregion

        #region Private helpers

        private void _OnWebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            _RaiseOnDownloadProgress(e.ProgressPercentage);
        }

        private void _OnWebClientFileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            _RaiseOnDownloadCompleted(_destinationFilePath);
        }

        private void _RaiseOnDownloadCompleted(string downloadedFilePath)
            => OnDownloadCompleted?.Invoke(this, new UpdateDownloadCompletedEventArgs(downloadedFilePath));

        private void _RaiseOnDownloadProgress(int percentProgress)
            => OnDownloadProgress?.Invoke(this, new UpdateDownloadProgressEventArgs(percentProgress));

        private void _RaiseOnDownloadError(string message, Exception exception = null)
            => OnDownloadError?.Invoke(this, new UpdateDownloadErrorEventArgs(message, exception));

        #endregion
    }
}