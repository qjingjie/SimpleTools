using NetMQ;
using NetMQ.Monitoring;
using NetMQ.Sockets;

namespace ZeroMQBroker.Models
{
    public class Broker
    {
        #region Fields

        private XPublisherSocket _publisher;
        private XSubscriberSocket _subscriber;

        private int _publishPort;
        private bool _isPublisherBound;

        private int _subscribePort;
        private bool _isSubscriberBound;

        private Proxy _proxy;
        private Task _proxyTask;

        #endregion Fields

        #region Constructor

        public Broker()
        {
        }

        #endregion Constructor

        #region Properties

        public NetMQMonitor PublisherMonitor
        {
            get;
            private set;
        }

        public NetMQMonitor SubscriberMonitor
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initialise a ZeroMQ broker instance.
        /// </summary>
        /// <param name="publishPort"></param>
        /// <param name="subscribePort"></param>
        /// <returns>True if broker is successfully initialised, false otherwise.</returns>
        public async Task<bool> InitBroker(int publishPort, int subscribePort)
        {
            bool isInitialised = true;
            Random randomEndpoint = new();

            _publishPort = publishPort;
            _subscribePort = subscribePort;

            // Set up publisher
            await Task.Run(() =>
            {
                try
                {
                    _publisher = new XPublisherSocket();
                    _publisher.Bind($"tcp://*:{publishPort}");
                    _isPublisherBound = true;

                    PublisherMonitor = new(_publisher, $"inproc://broker.pub.{randomEndpoint.Next(1, 65535)}", SocketEvents.All);
                    _ = PublisherMonitor.StartAsync();
                }
                catch
                {
                    isInitialised = false;
                }
            });

            // Set up subscriber
            await Task.Run(() =>
            {
                try
                {
                    _subscriber = new XSubscriberSocket();
                    _subscriber.Bind($"tcp://*:{subscribePort}");
                    _isSubscriberBound = true;

                    SubscriberMonitor = new(_subscriber, $"inproc://broker.sub.{randomEndpoint.Next(1, 65535)}", SocketEvents.All);
                    _ = SubscriberMonitor.StartAsync();
                }
                catch
                {
                    isInitialised = false;
                }
            });

            if (isInitialised)
            {
                _proxy = new(_subscriber, _publisher);

                _proxyTask = Task.Run(() =>
               {
                   _proxy.Start();
               });
            }

            return isInitialised;
        }

        /// <summary>
        /// Stop the broker instance and dispose objects.
        /// </summary>
        public async Task CloseBroker()
        {
            await Task.Run(() =>
            {
                if (_proxyTask != null && !_proxyTask.IsCompleted)
                {
                    _proxy.Stop();
                }

                if (_isPublisherBound)
                {
                    PublisherMonitor.Stop();
                    PublisherMonitor.Dispose();

                    _publisher?.Unbind($"tcp://*:{_publishPort}");
                    _isPublisherBound = false;
                    _publisher.Dispose();
                }

                if (_isSubscriberBound)
                {
                    SubscriberMonitor.Stop();
                    SubscriberMonitor.Dispose();

                    _subscriber?.Unbind($"tcp://*:{_subscribePort}");
                    _isSubscriberBound = false;
                    _subscriber.Dispose();
                }
            });
        }

        #endregion Methods
    }
}