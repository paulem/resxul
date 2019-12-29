using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Resxul.Properties;
using Resxul.Services;
using Resxul.ViewModels;
using Serilog;

namespace Resxul
{
    public class AppBootstrapper : BootstrapperBase
    {
        SimpleContainer _container;

        public AppBootstrapper()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(Global.LogsFolderPath, Global.LogFileName), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();

            _container.Singleton<ProfileService>();
            _container.Singleton<CompilerService>();

            _container.Singleton<IWindowManager, WindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.PerRequest<IShell, MainViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<IShell>();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, Resources.UnhandledExceptionOccurred);
            MessageBox.Show($"{Resources.UnhandledExceptionOccurred}{Environment.NewLine}{string.Format(Resources.ViewLogFile, Path.GetFullPath(Global.LogsFolderPath))}");
            base.OnUnhandledException(sender, e);
        }
    }
}