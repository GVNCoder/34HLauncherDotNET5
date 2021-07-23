﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

using Launcher.App.ViewModels;
using Launcher.Core.Data;
using Launcher.Core.Services;

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
            serviceCollection.AddSingleton<ILogger>(Log.Logger);

            // register viewModels
            serviceCollection.AddScoped<StartupWindowViewModel>();

            // create container
            Container = serviceCollection.BuildServiceProvider();

            Log.Debug("Some text here");
            Log.Debug("Some text here");
        }

        #endregion

        #region Startup

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
                    fileSizeLimitBytes: 1024,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: null,
                    buffered: true,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{appVersion}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

#endregion
    }
}
