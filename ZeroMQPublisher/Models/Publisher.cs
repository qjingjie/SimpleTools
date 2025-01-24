using NetMQ;
using NetMQ.Sockets;
using System.Threading.Channels;
using ZeroMQPublisher.Enums;

namespace ZeroMQPublisher.Models
{
    public class Publisher
    {
        #region Fields

        private readonly Channel<ZmqMessage> _messageChannel;

        private PublisherSocket _publisher;
        private Task _zmqRuntime;
        private CancellationTokenSource _cts;

        #endregion Fields

        #region Constructor

        public Publisher()
        {
            _messageChannel = Channel.CreateBounded<ZmqMessage>(new BoundedChannelOptions(50) { SingleReader = true, FullMode = BoundedChannelFullMode.DropOldest });
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Configure publisher to connect / disconnect / bind / unbind.
        /// </summary>
        /// <param name="connectionType"></param>
        /// <param name="ipv4"></param>
        /// <param name="port"></param>
        /// <returns>Connection state upon configuration request.</returns>
        public async Task<ConnectionState> Configure(ConnectionType connectionType, string ipv4, int port)
        {
            ConnectionState result = ConnectionState.Unconfigured;
            string address = $"tcp://{ipv4}:{port}";

            try
            {
                switch (connectionType)
                {
                    case ConnectionType.Connect:
                        _publisher = new PublisherSocket();
                        _publisher.Connect(address);

                        _cts = new CancellationTokenSource();
                        StartPublisher(_cts.Token);
                        result = ConnectionState.Connected;
                        break;

                    case ConnectionType.Disconnect:
                        _cts?.Cancel();
                        _cts?.Dispose();

                        if (_zmqRuntime != null)
                        {
                            while (!_zmqRuntime.IsCompleted)
                            {
                                await Task.Delay(100);
                            }

                            _publisher.Disconnect(address);
                            _publisher.Dispose();
                        }
                        break;

                    case ConnectionType.Bind:
                        _publisher = new PublisherSocket();
                        _publisher.Bind(address);

                        _cts = new CancellationTokenSource();
                        StartPublisher(_cts.Token);

                        result = ConnectionState.Bound;
                        break;

                    case ConnectionType.Unbind:
                        _cts?.Cancel();
                        _cts?.Dispose();

                        if (_zmqRuntime != null)
                        {
                            while (!_zmqRuntime.IsCompleted)
                            {
                                await Task.Delay(100);
                            }

                            _publisher.Unbind(address);
                            _publisher.Dispose();
                        }
                        break;

                    default:
                        break;
                }
            }
            catch
            {
                result = ConnectionState.Error;
            }

            return result;
        }

        /// <summary>
        /// Enqueue a message to the publisher's message channel.
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns>Task object.</returns>
        public async Task EnqueueMessageAsync(string topic, byte[] message)
        {
            ZmqMessage zmqMessage = new(topic, message);

            try
            {
                await _messageChannel.Writer.WriteAsync(zmqMessage, _cts.Token);
            }
            catch (Exception)
            {
                // Triggered by cancellation token
            }
        }

        /// <summary>
        /// Starts the ZeroMQ publisher runtime.
        /// </summary>
        /// <param name="ct"></param>
        private void StartPublisher(CancellationToken ct)
        {
            if (_zmqRuntime == null)
            {
                _zmqRuntime = Task.Run(() =>
                {
                    using var runtime = new NetMQRuntime();
                    runtime.Run(ct, ProcessMessagesAsync(ct));
                }, ct);
            }
        }

        /// <summary>
        /// Asynchronously process and sends message.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>Task object.</returns>
        private async Task ProcessMessagesAsync(CancellationToken ct)
        {
            ZmqMessage message;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    message = await _messageChannel.Reader.ReadAsync(ct);

                    if (message.Topic == "")
                    {
                        _publisher.SendFrame(message.Message);
                    }
                    else
                    {
                        _publisher.SendMoreFrame(message.Topic).SendFrame(message.Message);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Triggered by cancellation token
                }
            }
        }

        #endregion Methods
    }
}