using Microsoft.Extensions.DependencyInjection;
using SimpleTools.Enums;
using SimpleTools.Interfaces;
using SimpleTools.ViewModels;
using SimpleTools.Views;
using System.Windows;

namespace SimpleTools.Services
{
    public class WindowService : IWindowService
    {
        #region Fields

        private int _zeroMQMessagePublisherWindowCount;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Open a new window displaying the selected application.
        /// </summary>
        /// <param name="selector"></param>
        public void OpenWindow(AppSelector appSelector)
        {
            switch (appSelector)
            {
                case AppSelector.ZeroMQMessagePublisher:
                    if (_zeroMQMessagePublisherWindowCount < 5)
                    {
                        var window = new ZeroMQMessagePublisherWindow();
                        window.Closed += (object sender, EventArgs e) => { _zeroMQMessagePublisherWindowCount--; };
                        window.DataContext = ((App)Application.Current).ServiceProvider.GetService<ZeroMQMessagePublisherViewModel>();
                        window.Show();

                        _zeroMQMessagePublisherWindowCount++;
                    }
                    break;

                default:
                    break;
            }
        }

        #endregion Methods
    }
}