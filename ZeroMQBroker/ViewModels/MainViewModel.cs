using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NetMQ;
using NetMQ.Monitoring;
using Utils.Enums;
using ZeroMQBroker.Models;

namespace ZeroMQBroker.ViewModels
{
    public partial class MainViewModel(Broker broker) : ObservableObject
    {
        #region Fields

        private readonly Broker _broker = broker;

        #endregion Fields

        #region Properties

        [ObservableProperty]
        private int _publisherCount;

        [ObservableProperty]
        private int _subscriberCount;

        [ObservableProperty]
        private int _publishPort;

        [ObservableProperty]
        private int _subscribePort;

        [ObservableProperty]
        private State _InitialisationState;

        #endregion Properties

        #region Commands

        [RelayCommand]
        private async Task InitBrokerAsync()
        {
            bool isInitialised = await _broker.InitBroker(PublishPort, SubscribePort);

            if (isInitialised)
            {
                InitialisationState = State.Success;
                _broker.PublisherMonitor.EventReceived += HandlePublishEventReceive;
                _broker.SubscriberMonitor.EventReceived += HandleSubscribeEventReceive;
            }
            else
            {
                InitialisationState = State.Danger;
            }
        }

        [RelayCommand]
        private async Task CloseBrokerAsync()
        {
            if (_broker.PublisherMonitor != null)

            {
                _broker.PublisherMonitor.EventReceived -= HandlePublishEventReceive;
            }

            if (_broker.SubscriberMonitor != null)
            {
                _broker.SubscriberMonitor.EventReceived -= HandleSubscribeEventReceive;
            }

            await _broker.CloseBroker();

            PublisherCount = 0;
            SubscriberCount = 0;

            InitialisationState = State.Unknown;
        }

        [RelayCommand]
        private async Task WindowClosingAsync()
        {
            await _broker.CloseBroker();
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Increase / decrease count of subscribers connected to the publish port based on the type of socket event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandlePublishEventReceive(object sender, NetMQMonitorEventArgs e)
        {
            if (e.SocketEvent.Equals(SocketEvents.Accepted))
            {
                SubscriberCount++;
            }
            else if (e.SocketEvent.Equals(SocketEvents.Disconnected))
            {
                SubscriberCount--;
            }
        }

        /// <summary>
        /// Increase / decrease count of publishers connected to the subscribe port based on the type of socket event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSubscribeEventReceive(object sender, NetMQMonitorEventArgs e)
        {
            if (e.SocketEvent.Equals(SocketEvents.Accepted))
            {
                PublisherCount++;
            }
            else if (e.SocketEvent.Equals(SocketEvents.Disconnected))
            {
                PublisherCount--;
            }
        }

        #endregion Methods
    }
}