using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Text;
using ZeroMQSubscriber.Enums;
using ZeroMQSubscriber.Models;

namespace ZeroMQSubscriber.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly Subscriber _subscriber;

        private string _folderPath;
        private string _uriPath;

        #endregion Fields

        #region Constructor

        public MainViewModel(Subscriber subscriber)
        {
            _subscriber = subscriber;

            _folderPath = CreateDefaultDirectory();
            _uriPath = string.Empty;

            Ipv4 = string.Empty;
            Port = string.Empty;
            Topic = string.Empty;
            ReceivedMessage = string.Empty;
            LogFileName = string.Empty;
            LogFilePath = string.Empty;
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
            ReceivedMessage = string.Empty;
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
            ConfigureLogger();
            ShowLogFilePath = false;
        }

        [RelayCommand]
        private void StopLogging()
        {
            IsLogging = false;
            Log.CloseAndFlush();

            string latestLogFileName = GetLatestLogFileName();

            if (latestLogFileName != null)
            {
                LogFilePath = Path.Combine(_folderPath, latestLogFileName);
                _uriPath = new Uri(LogFilePath).AbsoluteUri;
                ShowLogFilePath = true;
            }
        }

        [RelayCommand]
        private void OpenLogFile()
        {
            Process.Start("explorer.exe", _uriPath);
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

            if (IsLogging)
            {
                Log.CloseAndFlush();
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Create a directory for storing log files in appdata.
        /// </summary>
        /// <returns>Created folder path.</returns>
        private static string CreateDefaultDirectory()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "SimpleTools", "ZeroMQ", "Logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            return folderPath;
        }

        /// <summary>
        /// Configure serilog logger.
        /// </summary>
        private void ConfigureLogger()
        {
            string fullPath = Path.Combine(_folderPath, LogFileName);
            Log.Logger = new LoggerConfiguration().WriteTo
                                                  .File($"{fullPath}_.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Message:lj}{NewLine}", rollingInterval: RollingInterval.Hour)
                                                  .CreateLogger();
        }

        /// <summary>
        /// Retreive the name of the latest updated log file.
        /// </summary>
        /// <returns>Name of most recently updated log file.</returns>
        private string GetLatestLogFileName()
        {
            string logPattern = $"{LogFileName}_*.txt";

            string latestLogFile = Directory.GetFiles(_folderPath, logPattern)
                                            .OrderByDescending(File.GetCreationTime)
                                            .FirstOrDefault();

            return latestLogFile;
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
                Log.Information(ReceivedMessage);
            }
        }

        #endregion Methods
    }
}