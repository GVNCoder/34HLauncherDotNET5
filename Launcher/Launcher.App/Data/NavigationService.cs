// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Launcher.Models;
using Launcher.Services;

/*
 * We have two history stacks
 * Stack backHistory
 * Stack forwardHistory
 *
 * Navigation to Page       -> clear forward history -> pop last history content if count of items in back history stack is more than 5 -> push current
 * Go back to prev Page     -> pop history content from back history stack (push) to forward history stack -> do not do anything if back stack is empty
 * Go forward to next Page  -> pop history content from forward history stack (push) to back history stack -> do no do anything if forward stack is empty
 */

namespace Launcher.Data
{
    public class NavigationService : INavigationService
    {
        #region Constants

        private const int HistoryDepth = 5;

        #endregion

        #region Internal type

        private struct _NavigationItem
        {
            public Uri Source;
            public object ContentRef;
        }

        #endregion

        private readonly Stack<_NavigationItem> _backHistory;
        private readonly Stack<_NavigationItem> _forwardHistory;

        private _NavigationItem? _currentContent;

        #region Ctor

        public NavigationService()
        {
            // populate internal state
            _backHistory = new Stack<_NavigationItem>(HistoryDepth);
            _forwardHistory = new Stack<_NavigationItem>(HistoryDepth);
        }

        #endregion

        #region INavigationService interface

        public event EventHandler<NavigatingEventArgs> Navigating;
        public event EventHandler<NavigatedEventArgs> Navigated;

        public object CurrentContent => _currentContent?.ContentRef;
        public Uri CurrentContentSource => _currentContent?.Source;
        public bool CanGoBack => _backHistory.Any();
        public bool CanGoForward => _forwardHistory.Any();

        public void Navigate(string source)
        {
            var sourceUri = new Uri(source, UriKind.Relative);

            if (_IsAlreadyNavigated(sourceUri))
            {
                return;
            }

            // we clear the forward history, because a new transition provokes a new branch of transitions history
            _forwardHistory.Clear();

            // fire event
            _OnNavigating(sourceUri);

            // load a new content
            var content = Application.LoadComponent(sourceUri);
            var navigationItem = new _NavigationItem { Source = sourceUri, ContentRef = content };

            _currentContent = navigationItem;

            // save transition history
            if (_backHistory.Count == HistoryDepth)
            {
                _ = _backHistory.Pop();
            }

            _backHistory.Push(navigationItem);

            // fire event
            _onNavigated(navigationItem);
        }

        public void GoBack()
        {
            if (CanGoBack == false)
            {
                return;
            }

            var backNavigationItem = _backHistory.Pop();

            // fire event
            _OnNavigating(backNavigationItem.Source);

            _forwardHistory.Push(backNavigationItem);
            _currentContent = backNavigationItem;

            // fire event
            _onNavigated(backNavigationItem);
        }

        public void GoForward()
        {
            if (CanGoForward == false)
            {
                return;
            }

            var forwardNavigationItem = _forwardHistory.Pop();

            // fire event
            _OnNavigating(forwardNavigationItem.Source);

            _backHistory.Push(forwardNavigationItem);
            _currentContent = forwardNavigationItem;

            // fire event
            _onNavigated(forwardNavigationItem);
        }

        #endregion

        #region Private helpers

        private bool _IsAlreadyNavigated(Uri source)
        {
            if (_currentContent == null)
            {
                return false;
            }

            return _currentContent.Value.Source == source;
        }

        private void _OnNavigating(Uri source) => Navigating?.Invoke(this, new NavigatingEventArgs(source));
        private void _onNavigated(_NavigationItem navigationItem) => Navigated?.Invoke(this, new NavigatedEventArgs(navigationItem.Source, navigationItem.ContentRef));

        #endregion
    }
}