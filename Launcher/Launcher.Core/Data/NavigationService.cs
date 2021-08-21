using System;
using System.Collections.Generic;
using System.Windows;
using Launcher.Core.Models;
using Launcher.Core.Services;

/*
 * We have two history stacks
 * Stack backHistory
 * Stack forwardHistory
 *
 * Navigation to Page       -> clear forward history -> pop last history content if count of items in back history stack is more than 5 -> push current
 * Go back to prev Page     -> pop history content from back history stack (push) to forward history stack -> do not do anything if back stack is empty
 * Go forward to next Page  -> pop history content from forward history stack (push) to back history stack -> do no do anything if forward stack is empty
 */

namespace Launcher.Core.Data
{
    public class NavigationService : INavigationService
    {
        #region Constants

        private const int HistoryDepth = 5;

        #endregion

        private readonly Stack<object> _backHistory;
        private readonly Stack<object> _forwardHistory;

        private object _currentContentRef;
        private string _currentSource;

        #region Ctor

        public NavigationService()
        {
            // populate internal state
            _backHistory = new Stack<object>(HistoryDepth);
            _forwardHistory = new Stack<object>(HistoryDepth);
        }

        #endregion

        #region INavigationService interface

        public event EventHandler<NavigatingEventArgs> Navigating;
        public event EventHandler<NavigatedEventArgs> Navigated;

        public object CurrentContent { get; }
        public string CurrentContentSource { get; }
        public bool CanGoBack { get; }
        public bool CanGoForward { get; }

        public void Navigate(string source)
        {
            if (_IsAlreadyNavigated(source))
            {
                return;
            }

            _OnNavigating(source);

            var sourceUri = new Uri(source, UriKind.Relative);
            var content = Application.LoadComponent(sourceUri);

            // TODO: Migrate all implementations to Launcher.App project

            _currentSource = source;
            _currentContentRef = content;

            _forwardHistory.Clear();

            if (_backHistory.Count == HistoryDepth)
            {
                _ = _backHistory.Pop();
            }

            _backHistory.Push(_currentContentRef);
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public void GoForward()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private helpers

        private bool _IsAlreadyNavigated(string source)
        {
            return _currentSource == source;
        }

        private void _OnNavigating(string source) => Navigating?.Invoke(this, new NavigatingEventArgs(source));
        private void _onNavigated(string source, object content) => Navigated?.Invoke(this, new NavigatedEventArgs(source, content));

        #endregion
    }
}