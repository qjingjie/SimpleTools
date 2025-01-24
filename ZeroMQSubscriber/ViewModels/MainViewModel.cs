using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text;
using System.Threading.Tasks;
using Utils;
using ZeroMQSubscriber.Enums;
using ZeroMQSubscriber.Models;

namespace ZeroMQSubscriber.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly Subscriber _subscriber;

        private string _folderPath;

        #endregion Fields

        #region Constructor

        public MainViewModel(Subscriber subscriber)
        {
            _subscriber = subscriber;

            Ipv4 = string.Empty;
            Port = string.Empty;
            Topic = string.Empty;
            ReceivedMessage = string.Empty;
            LogFileName = string.Empty;

            CreateDefaultDirectory();
        }

        #endregion Constructor

        #region Properties

        [ObservableProperty]
        private string _ipv4;

        [ObservableProperty]
        private string _port;

        [ObservableProperty]
        private ConnectionState _connectionState;

        [ObservableProperty]
        private string _topic;

        [ObservableProperty]
        private bool _isSubscribed;

        [ObservableProperty]
        private string _receivedMessage;

        [ObservableProperty]
        private string _logFileName;

        [ObservableProperty]
        private string _logFilePath;

        [ObservableProperty]
        private bool _isLogging;

        [ObservableProperty]
        private bool _showLogFilePath;

        #endregion Properties

        #region Commands

        [RelayCommand]
        private async Task ConfigurePublisherAsync(ConnectionType connectionType)
        {
            ConnectionState = await _subscriber.Configure(connectionType, Ipv4, int.Parse(Port));

            if (ConnectionState == ConnectionState.Connected || ConnectionState == ConnectionState.Bound)
            {
                _subscriber.OnMessageReceiveEvent += HandleMessageReceive;
            }
        }

        [RelayCommand]
        private void Subscribe()
        {
            _subscriber.SubscribeTopic(Topic);
            IsSubscribed = true;
        }

        [RelayCommand]
        private void Unsubscribe()
        {
            _subscriber.UnsubscribeTopic(Topic);
            IsSubscribed = false;
        }

        [RelayCommand]
        private void StartLogging()
        {
            IsLogging = true;
            LogFilePath = Path.Combine(_folderPath, $"{LogFileName}.txt");
            ShowLogFilePath = false;
        }

        [RelayCommand]
        private void StopLogging()
        {
            IsLogging = false;
            ShowLogFilePath = true;
        }

        [RelayCommand]
        private async Task WindowClosingAsync()
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                await _subscriber.Configure(ConnectionType.Disconnect, Ipv4, int.Parse(Port));
            }

            if (ConnectionState == ConnectionState.Bound)
            {
                await _subscriber.Configure(ConnectionType.Unbind, Ipv4, int.Parse(Port));
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Create a directory for storing log files in appdata.
        /// </summary>
        private void CreateDefaultDirectory()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "SimpleTools", "ZeroMQ", "Logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            _folderPath = folderPath;
        }

        /// <summary>
        /// Handles / log messages received from the subscriber.
        /// </summary>
        /// <param name="message"></param>
        private void HandleMessageReceive(byte[] message)
        {
            ReceivedMessage = Encoding.Default.GetString(message);

            if (IsLogging)
            {
                File.AppendAllText(LogFilePath, $"{ReceivedMessage}\n");
            }
        }

        #endregion Methods
    }
}