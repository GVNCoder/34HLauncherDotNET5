using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;

using Launcher.App.ViewModels;
using Launcher.Core.Data;
using Launcher.Core.Services;
using Launcher.Localization;
using Launcher.Themes;
using Launcher.ViewModels;
using Serilog;

// ReSharper disable MemberCanBePrivate.Global

[assembly: AssemblyVersion("0.9.0.0")]

namespace Launcher
{
    public partial class LauncherApp : Application
    {
        #region Properties

        public static IServiceProvider Container { get; private set; }
        public static string ReleaseCandidature { get; } = "rc1";

        #endregion

        #region Ctor

        public LauncherApp()
        {
            // create logger
            Log.Logger = BuildApplicationLogger();

            // configure IoC container
            var serviceCollection = new ServiceCollection();

            // register services
            serviceCollection.AddTransient<IUpdateChecker, UpdateChecker>();
            serviceCollection.AddTransient<IUpdateDownloader, UpdateDownloader>();
            serviceCollection.AddTransient<IUpdateInstaller, UpdateInstaller>();
            serviceCollection.AddSingleton<ILogger>(Log.Logger);

            // register viewModels
            serviceCollection.AddScoped<StartupWindowViewModel>();
            serviceCollection.AddScoped<MainWindowViewModel>();
            serviceCollection.AddScoped<WindowNonClientViewModel>();

            // create container
            Container = serviceCollection.BuildServiceProvider();
        }

        #endregion

        #region Startup

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // track all unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += _OnAppDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            //Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;

            // init some static managers
            LocalizationManager.Init(Resources);
            ThemeManager.Init(Resources);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // flush logger buffer
            Log.CloseAndFlush();
        }

        #endregion

        #region Private helpers

        private static ILogger BuildApplicationLogger()
        {
            var currentAssemblyName = Assembly.GetExecutingAssembly()
                .GetName();
            var outputFile = Path.Combine("log", "launcher.log");

            // configure logger
            return new LoggerConfiguration()
#if DEBUG
                .MinimumLevel.Verbose()
#else
                .MinimumLevel.Warning()
#endif
                .Enrich.WithProperty("appVersion", $"{currentAssemblyName.Version}-{ReleaseCandidature}")
                .WriteTo.File(outputFile,
                    fileSizeLimitBytes: 1048576, // 1 MBytes
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    buffered: true,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{appVersion}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        private static void _OnAppDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                Log.Fatal((Exception) e.ExceptionObject, $"{nameof(AppDomain)}");
            }
            else
            {
                Log.Error((Exception) e.ExceptionObject, $"{nameof(AppDomain)}");
            }
        }

        private static void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Handled)
            {
                Log.Error(e.Exception, $"{nameof(LauncherApp)}");
            }
            else
            {
                Log.Fatal(e.Exception, $"{nameof(LauncherApp)}");
            }

            //e.Handled = true;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            if (e.Observed)
            {
                Log.Error(e.Exception, $"{nameof(LauncherApp)}");
            }
            else
            {
                Log.Fatal(e.Exception, $"{nameof(LauncherApp)}");
            }
        }

        #endregion
    }
}
