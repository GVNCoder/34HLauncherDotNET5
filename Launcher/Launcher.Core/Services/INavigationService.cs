using System;
using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface INavigationService
    {
        event EventHandler<NavigatingEventArgs> Navigating; // occurs when navigation initiated
        event EventHandler<NavigatedEventArgs> Navigated; // occurs when navigation is done

        object CurrentContent { get; }
        string CurrentContentSource { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        void Navigate(string source);
        void GoBack();
        void GoForward();
    }
}