using AppSelector.Enums;
using AppSelector.Interfaces;
using AppSelector.Services;
using AppSelector.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AppSelector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructor

        public App()
        {
        }

        #endregion Constructor

        #region Properties

        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Prepare and load necessary startup items.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ServiceProvider = CreateServiceProvider();

            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            Current.MainWindow = mainWindow;
            Current.MainWindow.Show();
        }

        /// <summary>
        /// Creates a service provider used to store registered services for dependency injection.
        /// </summary>
        /// <returns>Service provider.</returns>
        private static ServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            #region Register Services

            services.AddSingleton<IAppLauncher<AppName>, AppLauncher>();

            #endregion Register Services

            #region Register ViewModels

            services.AddSingleton<MainViewModel>();

            #endregion Register ViewModels

            #region Register Views

            services.AddSingleton<MainWindow>();

            #endregion Register Views

            #region Referenced Projects

            ZeroMQBroker.App.ConfigureServices(services);
            ZeroMQPublisher.App.ConfigureServices(services);
            ZeroMQSubscriber.App.ConfigureServices(services);

            #endregion Referenced Projects

            return services.BuildServiceProvider();
        }

        #endregion Methods
    }
}