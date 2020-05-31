using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Resxul.Framework.Shortcuts;
using Resxul.Framework.ToastNotifications;
using Resxul.Properties;
using Resxul.Services;
using Resxul.ViewModels;
using Serilog;
using MessageBox = System.Windows.MessageBox;

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
            Shortcut.Create(Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\Resxul.lnk", @"C:\Users\Pavel\Dropbox\Development\Resxul\src\Resxul\bin\Debug\netcoreapp3.1\Resxul.exe", null, null, null, ShortcutWindowState.Normal, null,
                "7room.Resxul", Guid.Parse("4BF727D6-C681-4285-8FF7-7E05A331E6BB"));

            // Register AUMID, COM server, and activator
            DesktopNotificationManagerCompat.RegisterAumidAndComServer<ResxulNotificationActivator>("7room.Resxul");
            DesktopNotificationManagerCompat.RegisterActivator<ResxulNotificationActivator>();

            // If launched from a toast
            // This launch arg was specified in our WiX installer where we register the LocalServer32 exe path.
            if (e.Args.Contains(DesktopNotificationManagerCompat.TOAST_ACTIVATED_LAUNCH_ARG))
            {
                MessageBox.Show("TOAST MSG");
                // Our NotificationActivator code will run after this completes,
                // and will show a window if necessary.
            }

            else
            {
                // Show the window
                // In App.xaml, be sure to remove the StartupUri so that a window doesn't
                // get created by default, since we're creating windows ourselves (and sometimes we
                // don't want to create a window if handling a background activation).
                DisplayRootViewFor<IShell>();
            }
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, Resources.UnhandledExceptionOccurred);
            MessageBox.Show($"{Resources.UnhandledExceptionOccurred}{Environment.NewLine}{string.Format(Resources.ViewLogFile, Path.GetFullPath(Global.LogsFolderPath))}");
            base.OnUnhandledException(sender, e);
        }
    }
}