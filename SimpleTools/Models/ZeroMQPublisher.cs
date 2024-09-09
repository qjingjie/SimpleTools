using NetMQ;
using NetMQ.Sockets;
using SimpleTools.Enums;
using System.Text;
using System.Threading.Channels;

namespace SimpleTools.Models
{
    public class ZeroMQPublisher
    {
        #region Fields

        private readonly PublisherSocket _publisher;
        private readonly Channel<ZmqMessage> _channel;

        private Task _zmqRuntime;
        private bool _stopCommunications;

        #endregion Fields

        #region Constructor

        public ZeroMQPublisher()
        {
            _publisher = new PublisherSocket();
            _publisher.Options.HeartbeatInterval = TimeSpan.FromSeconds(1000);

            _channel = Channel.CreateUnbounded<ZmqMessage>();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Configure publisher to connect / disconnect / bind / unbind.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="ipv4"></param>
        /// <param name="port"></param>
        /// <returns>Connection state upon configuration request.</returns>
        public ConnectionState Configure(ConnectionState state, string ipv4, string port)
        {
            ConnectionState connectionState = ConnectionState.Unconfigured;

            string configurationString = "tcp://" + ipv4 + ":" + port;

            switch (state)
            {
                case ConnectionState.Connected:
                    try
                    {
                        _publisher.Connect(configurationString);
                        connectionState = ConnectionState.Connected;
                        StartPublisher();
                    }
                    catch
                    {
                        connectionState = ConnectionState.Error;
                        StopPublisher();
                    }
                    break;

                case ConnectionState.Disconnected:
                    try
                    {
                        _publisher.Disconnect(configurationString);
                        connectionState = ConnectionState.Unconfigured;
                        StopPublisher();
                    }
                    catch
                    {
                        connectionState = ConnectionState.Error;
                        StopPublisher();
                    }
                    break;

                case ConnectionState.Bind:
                    try
                    {
                        _publisher.Bind(configurationString);
                        connectionState = ConnectionState.Bind;
                        StartPublisher();
                    }
                    catch
                    {
                        connectionState = ConnectionState.Error;
                        StopPublisher();
                    }
                    break;

                case ConnectionState.Unbind:
                    try
                    {
                        _publisher.Unbind(configurationString);
                        connectionState = ConnectionState.Unconfigured;
                        StopPublisher();
                    }
                    catch
                    {
                        connectionState = ConnectionState.Error;
                        StopPublisher();
                    }
                    break;

                default:
                    break;
            }

            return connectionState;
        }

        /// <summary>
        /// Close and dispose the publiser connection.
        /// </summary>
        public void ClosePublisher()
        {
            if (!_publisher.IsDisposed)
            {
                StopPublisher();
                _publisher.Close();
                _publisher.Dispose();
            }
        }

        /// <summary>
        /// Publish message.
        /// </summary>
        /// <param name="message"></param>
        public async void SendMessage(string message, string topic)
        {
            byte[] byteMessage = Encoding.Default.GetBytes(message);
            ZmqMessage zmqMessage = new(topic, byteMessage);

            if (!_stopCommunications)
            {
                await _channel.Writer.WriteAsync(zmqMessage);
            }
        }

        /// <summary>
        /// Initialise message sending task.
        /// </summary>
        private void StartPublisher()
        {
            if (_zmqRuntime == null)
            {
                _zmqRuntime = new(() =>
                {
                    using var runtime = new NetMQRuntime();
                    runtime.Run(ProcessOutgoingMessageAsync());
                });

                _stopCommunications = false;
                _zmqRuntime.Start();
            }
        }

        /// <summary>
        /// Stop publisher from sending messages.
        /// </summary>
        private void StopPublisher()
        {
            _stopCommunications = true;
        }

        /// <summary>
        /// Process outgoing messages available in the channel.
        /// </summary>
        /// <returns></returns>
        private async Task ProcessOutgoingMessageAsync()
        {
            ZmqMessage message;

            while (!_stopCommunications)
            {
                message = await _channel.Reader.ReadAsync();

                if (message.Topic == "")
                {
                    _publisher.SendFrame(message.Message);
                }
                else
                {
                    _publisher.SendMoreFrame(message.Topic).SendFrame(message.Message);
                }
            }
        }

        #endregion Methods
    }
}