using AppSelector.Enums;
using AppSelector.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace AppSelector.Services
{
    public class AppLauncher : IAppLauncher<AppName>
    {
        #region Methods

        /// <summary>
        /// Prepare and launch a desired application.
        /// </summary>
        /// <param name="appName"></param>
        public void LaunchApp(AppName appName)
        {
            switch (appName)
            {
                case AppName.ZmqBroker:
                    var zeroMQBroker = new ZeroMQBroker.MainWindow
                    {
                        DataContext = ((App)Application.Current).ServiceProvider.GetService<ZeroMQBroker.ViewModels.MainViewModel>()
                    };
                    zeroMQBroker.Show();
                    break;

                case AppName.ZmqPublisher:
                    var zeroMQPublisher = new ZeroMQPublisher.MainWindow
                    {
                        DataContext = ((App)Application.Current).ServiceProvider.GetService<ZeroMQPublisher.ViewModels.MainViewModel>()
                    };
                    zeroMQPublisher.Show();
                    break;

                case AppName.ZmqSubscriber:
                    var zeroMQSubscriber = new ZeroMQSubscriber.MainWindow
                    {
                        DataContext = ((App)Application.Current).ServiceProvider.GetService<ZeroMQSubscriber.ViewModels.MainViewModel>()
                    };
                    zeroMQSubscriber.Show();
                    break;

                default:
                    break;
            }
        }

        #endregion Methods
    }
}