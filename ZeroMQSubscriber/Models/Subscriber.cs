using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZeroMQSubscriber.Enums;

namespace ZeroMQSubscriber.Models
{
    public class Subscriber
    {
        #region Fields

        private SubscriberSocket _subscriber;
        private Task _zmqRuntime;
        private CancellationTokenSource _cts;

        #endregion Fields

        #region Constructor

        public Subscriber()
        {
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Configure subscriber to connect / disconnect / bind / unbind.
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
                        _subscriber = new SubscriberSocket();
                        _subscriber.Options.HeartbeatInterval = TimeSpan.FromSeconds(1);
                        _subscriber.Connect(address);

                        _cts = new CancellationTokenSource();
                        StartSubscriber(_cts.Token);
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

                            _subscriber.Disconnect(address);
                            _subscriber.Dispose();
                        }
                        break;

                    case ConnectionType.Bind:
                        _subscriber = new SubscriberSocket();
                        _subscriber.Options.HeartbeatInterval = TimeSpan.FromSeconds(1);
                        _subscriber.Bind(address);

                        _cts = new CancellationTokenSource();
                        StartSubscriber(_cts.Token);

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

                            _subscriber.Unbind(address);
                            _subscriber.Dispose();
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
        /// Subscribe to a desired topic.
        /// </summary>
        /// <param name="topic"></param>
        public void SubscribeTopic(string topic)
        {
            _subscriber.Subscribe(topic);
        }

        /// <summary>
        /// Unsubscribe a topic.
        /// </summary>
        /// <param name="topic"></param>
        public void UnsubscribeTopic(string topic)
        {
            _subscriber.Unsubscribe(topic);
        }

        /// <summary>
        /// Starts the ZeroMQ subscriber runtime.
        /// </summary>
        /// <param name="ct"></param>
        private void StartSubscriber(CancellationToken ct)
        {
            if (_zmqRuntime == null)
            {
                _zmqRuntime = Task.Run(() =>
                {
                    using var runtime = new NetMQRuntime();
                    runtime.Run(ct, ProcessIncomingMessageAsync(ct));
                }, ct);
            }
        }

        /// <summary>
        /// Process outgoing messages available in the channel.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns>Task object.</returns>
        private async Task ProcessIncomingMessageAsync(CancellationToken ct)
        {
            string messageA;
            bool moreFrames;
            byte[] messageB;

            while (!ct.IsCancellationRequested)
            {
                try
                {
                    (messageA, moreFrames) = await _subscriber.ReceiveFrameStringAsync(ct);

                    if (messageA != null)
                    {
                        if (moreFrames)
                        {
                            (messageB, _) = await _subscriber.ReceiveFrameBytesAsync(ct);
                            OnMessageReceiveEvent?.Invoke(messageB);
                        }
                        else
                        {
                            OnMessageReceiveEvent?.Invoke(Encoding.Default.GetBytes(messageA));
                        }
                    }
                }
                catch
                {
                    // Triggered by cancellation token
                }
            }
        }

        #endregion Methods

        #region Events

        public event Action<byte[]> OnMessageReceiveEvent;

        #endregion Events
    }
}