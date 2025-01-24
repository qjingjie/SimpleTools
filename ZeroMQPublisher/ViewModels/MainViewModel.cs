using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Utils;
using Utils.Interfaces;
using ZeroMQPublisher.Enums;
using ZeroMQPublisher.Models;

namespace ZeroMQPublisher.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly IValidation<MessageFormat> _messageConversion;
        private readonly Publisher _publisher;

        private HighResolutionTimer _repeatTimer;

        #endregion Fields

        #region Constructor

        public MainViewModel(IValidation<MessageFormat> messageConversion,
                             Publisher publisher)
        {
            _messageConversion = messageConversion;
            _publisher = publisher;

            MessageFormatOptions =
            [
                new MessageFormatOption(MessageFormat.String, "Text Files (*.txt)|*.txt"),
                new MessageFormatOption(MessageFormat.Json, "JSON Files (.json)|.json")
            ];

            SelectedMessageFormatOption = MessageFormatOptions[0];

            Ipv4 = string.Empty;
            Port = string.Empty;
            Topic = string.Empty;
            MessageEditorInput = string.Empty;
            MessagePreview = string.Empty;

            IsSaveEnabled = false;
            IsPublishRepeatEnabled = true;

            RepeatIntervalMs = 100;

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
        private int _repeatIntervalMs;

        [ObservableProperty]
        private string _messageEditorInput;

        [ObservableProperty]
        private string _messagePreview;

        [ObservableProperty]
        private bool _isSaveEnabled;

        [ObservableProperty]
        private bool _isPublishRepeatEnabled;

        [ObservableProperty]
        private bool _messageError;

        [ObservableProperty]
        private ObservableCollection<MessageFormatOption> _messageFormatOptions;

        [ObservableProperty]
        private MessageFormatOption _selectedMessageFormatOption;

        #endregion Properties

        #region Commands

        [RelayCommand]
        private async Task ConfigurePublisherAsync(ConnectionType connectionType)
        {
            ConnectionState = await _publisher.Configure(connectionType, Ipv4, int.Parse(Port));
        }

        [RelayCommand]
        private async Task UploadAsync()
        {
            string dialogFilter = string.Empty;

            for (int i = 0; i < MessageFormatOptions.Count; i++)
            {
                dialogFilter += MessageFormatOptions[i].MessageFormatFilter;

                if (i != MessageFormatOptions.Count - 1)
                {
                    dialogFilter += "|";
                }
            }

            await Task.Run(() => UploadFileDialog(dialogFilter));
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            await Task.Run(() => SaveFileDialog(SelectedMessageFormatOption.MessageFormatFilter));
        }

        [RelayCommand]
        private async Task PublishOnceAsync()
        {
            await _publisher.EnqueueMessageAsync(Topic, Encoding.ASCII.GetBytes(MessagePreview));
        }

        [RelayCommand]
        private void StartPublishRepeat()
        {
            _repeatTimer = new()
            {
                IntervalMs = RepeatIntervalMs
            };

            _repeatTimer.Elapsed += async () => await _publisher.EnqueueMessageAsync(Topic, Encoding.ASCII.GetBytes(MessagePreview));
            _repeatTimer.Start();

            IsPublishRepeatEnabled = false;
        }

        [RelayCommand]
        private void StopPublishRepeat()
        {
            if (_repeatTimer != null)
            {
                _repeatTimer.Elapsed -= async () => await _publisher.EnqueueMessageAsync(Topic, Encoding.ASCII.GetBytes(MessagePreview));
                _repeatTimer.Stop();

                IsPublishRepeatEnabled = true;
            }
        }

        [RelayCommand]
        private async Task WindowClosingAsync()
        {
            if (ConnectionState == ConnectionState.Connected)
            {
                await _publisher.Configure(ConnectionType.Disconnect, Ipv4, int.Parse(Port));
            }

            if (ConnectionState == ConnectionState.Bound)
            {
                await _publisher.Configure(ConnectionType.Unbind, Ipv4, int.Parse(Port));
            }
        }

        #endregion Commands

        #region Methods

        /// <summary>
        /// Create a directory for storing message files in appdata.
        /// </summary>
        private static void CreateDefaultDirectory()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "SimpleTools", "ZeroMQ", "Messages");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        /// <summary>
        /// Open file dialog for locating and selecting an existing message file.
        /// </summary>
        /// <param name="fileTypeFilter"></param>
        private void UploadFileDialog(string fileTypeFilter)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "SimpleTools", "ZeroMQ", "Messages");

            OpenFileDialog fileDialog = new()
            {
                Filter = fileTypeFilter,
                InitialDirectory = folderPath
            };

            if (fileDialog.ShowDialog() == true)
            {
                // Load contents of the file into the message editor
                using StreamReader streamReader = new(fileDialog.FileName, Encoding.UTF8);
                MessageEditorInput = streamReader.ReadToEnd();
            }

            IsSaveEnabled = true;
        }

        /// <summary>
        /// Open file dialog for saving previewed message.
        /// </summary>
        /// <param name="fileTypeFilter"></param>
        private void SaveFileDialog(string fileTypeFilter)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folderPath = Path.Combine(appDataPath, "SimpleTools", "ZeroMQ", "Messages");

            SaveFileDialog fileDialog = new()
            {
                Filter = fileTypeFilter,
                InitialDirectory = folderPath
            };

            if (fileDialog.ShowDialog() == true)
            {
                string filePath = fileDialog.FileName;
                File.WriteAllText(filePath, MessagePreview);
            }
        }

        /// <summary>
        /// Check and update preview message as editor input text changes.
        /// </summary>
        private void CheckAndUpdateMessagePreview()
        {
            (bool, string) conversionCheck = _messageConversion.Validate(SelectedMessageFormatOption.MessageFormatType, MessageEditorInput);
            MessageError = !conversionCheck.Item1;
            MessagePreview = conversionCheck.Item2;

            if (MessageError || MessagePreview == string.Empty || string.IsNullOrWhiteSpace(MessagePreview))
            {
                IsSaveEnabled = false;
            }
            else
            {
                IsSaveEnabled = true;
            }
        }

        /// <summary>
        /// Check and update preview message as editor input text changes.
        /// </summary>
        /// <param name="value"></param>
        partial void OnMessageEditorInputChanged(string value)
        {
            CheckAndUpdateMessagePreview();
        }

        /// <summary>
        /// Check and update preview message as editor input text changes.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        partial void OnSelectedMessageFormatOptionChanged(MessageFormatOption oldValue, MessageFormatOption newValue)
        {
            CheckAndUpdateMessagePreview();
        }

        #endregion Methods
    }
}