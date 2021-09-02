// ReSharper disable SwitchStatementMissingSomeCases
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using Launcher.App.ViewModels;
using Launcher.Data;
using Launcher.Extensions;
using Launcher.Helpers;
using Launcher.Services;
using Launcher.Localization;
using Launcher.Models;
using Launcher.Themes;
using Launcher.Utilities;
using Launcher.ViewModels;

using Microsoft.Extensions.DependencyInjection;
using Serilog;

using Zlo4NET.Api.Service;
using Zlo4NET.Api.Shared;
using Zlo4NET.Data;

[assembly: AssemblyVersion("0.9.0.0")]

namespace Launcher
{
    public partial class LauncherApp : Application
    {
        #region Properties

        public static IServiceProvider Container { get; private set; }
        public static CommandLineArguments CommandLineArguments { get; private set; }
        public static bool IsDev { get; private set; }

        #endregion

        #region Ctor

        public LauncherApp()
        {
            // create logger
            Log.Logger = BuildApplicationLogger();

            // configure IoC container
            var serviceCollection = new ServiceCollection();

            // register services
            serviceCollection.AddUpdaterServices();
            serviceCollection.AddSingleton<ILogger>(Log.Logger);
            serviceCollection.AddSingleton<INavigationService, NavigationService>();
            serviceCollection.AddZloAPI();

            // register viewModels
            serviceCollection.AddViewModels();

            // create container
            Container = serviceCollection.BuildServiceProvider();
        }

        #endregion

        #region Startup

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InstallErrorLoggers();

            // init some static properties
            CommandLineArguments = CommandLineUtility.ParseCommandLineArguments(e.Args);
            IsDev = CommandLineArguments.ItselfValuedArguments.Contains(CommandLineUtility.DevChannel);

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
            // TODO: set stacktrace depth

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
                .Enrich.WithProperty("appVersion", $"{currentAssemblyName.Version}")
                .Enrich.With(new SerilogCustomExceptionEnricher(5))
                .WriteTo.File(outputFile,
                    fileSizeLimitBytes: 1048576, // 1 MBytes
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    buffered: true,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{appVersion}] [{Level:u3}] {Message:lj}{NewLine}{customException}")
                .CreateLogger();
        }

        private static void InstallErrorLoggers()
        {
            // track all unhandled exceptions
            AppDomain.CurrentDomain.UnhandledException += _OnAppDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            //Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;

            var apiLogger = ZApi.Instance.GetApiLogger();
#if DEBUG
            apiLogger.SetLoggingLevelFiltering(ZLoggingLevel.Debug | ZLoggingLevel.Warning | ZLoggingLevel.Info | ZLoggingLevel.Error);
#else
            apiLogger.SetLoggingLevelFiltering(ZLoggingLevel.Info | ZLoggingLevel.Warning | ZLoggingLevel.Error);
#endif
            apiLogger.ApiMessage += _OnApiLoggerMessage;
        }

        private static void _OnApiLoggerMessage(object sender, ZLoggerMessageEventArgs e)
        {
            switch (e.Level)
            {
                case ZLoggingLevel.Info:
                    Log.Information(e.Message);
                    break;
                case ZLoggingLevel.Debug:
                    Log.Debug(e.Message);
                    break;
                case ZLoggingLevel.Warning:
                    Log.Warning(e.Message);
                    break;
                case ZLoggingLevel.Error:
                    Log.Error(e.Message);
                    break;
            }
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

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
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
