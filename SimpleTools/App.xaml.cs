using Microsoft.Extensions.DependencyInjection;
using SimpleTools.Enums;
using SimpleTools.Interfaces;
using SimpleTools.Services;
using SimpleTools.ViewModels;
using SimpleTools.Views;
using System.Windows;

namespace SimpleTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructor

        public App()
        {
            // Allow UpdateSourceTrigger = PropertyChanged to input decimal values
            FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
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
        /// <returns></returns>
        private IServiceProvider CreateServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            #region Register Services

            services.AddSingleton<IWindowService, WindowService>();
            services.AddSingleton<IValidation<MessageConversionType>, MessageConversionService>();

            #endregion Register Services

            #region Register ViewModels

            services.AddSingleton<MainViewModel>();
            services.AddTransient<ZeroMQMessagePublisherViewModel>();

            #endregion Register ViewModels

            #region Register Views

            services.AddSingleton<MainWindow>();
            services.AddTransient<ZeroMQMessagePublisherWindow>();

            #endregion Register Views

            return services.BuildServiceProvider();
        }

        #endregion Methods
    }
}