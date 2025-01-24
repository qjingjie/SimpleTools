using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ZeroMQBroker.Models;
using ZeroMQBroker.ViewModels;

namespace ZeroMQBroker
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

        #region Methods

        /// <summary>
        /// Register objects for dependency injection.
        /// </summary>
        public static void ConfigureServices(IServiceCollection services)
        {
            #region Register Models

            services.AddTransient<Broker>();

            #endregion Register Models

            #region Register ViewModels

            services.AddTransient<MainViewModel>();

            #endregion Register ViewModels

            #region Register Views

            services.AddTransient<MainWindow>();

            #endregion Register Views
        }

        #endregion Methods
    }
}