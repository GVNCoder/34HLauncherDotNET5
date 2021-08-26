using System;
using System.Windows.Controls;

using Launcher.Models;

namespace Launcher.Services
{
    public interface INavigationService
    {
        event EventHandler<NavigatingEventArgs> Navigating; // occurs when navigation initiated
        event EventHandler<NavigatedEventArgs> Navigated; // occurs when navigation is done

        Page CurrentContent { get; }
        Uri CurrentContentSource { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        void Navigate(string source);
        void GoBack();
        void GoForward();
    }
}