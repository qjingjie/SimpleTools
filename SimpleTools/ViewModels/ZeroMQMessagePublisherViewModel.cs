using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using SimpleTools.Enums;
using SimpleTools.Interfaces;
using SimpleTools.Models;
using SimpleTools.ViewModels.ObservableObjects;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Timers;

namespace SimpleTools.ViewModels
{
    public partial class ZeroMQMessagePublisherViewModel : ObservableObject
    {
        #region Fields

        private readonly ZeroMQPublisher _publisher;
        private readonly IValidation<MessageConversionType> _messageConversionService;

        private System.Timers.Timer _repeatTimer;

        #endregion Fields

        #region Constructor

        public ZeroMQMessagePublisherViewModel(IValidation<MessageConversionType> messageConversionService)
        {
            _publisher = new ZeroMQPublisher();
            _messageConversionService = messageConversionService;

            MessageConversionOptions =
            [
                new MessageConversionOption(MessageConversionType.String),
                new MessageConversionOption(MessageConversionType.JSON)
            ];

            SelectedOption = new MessageConversionOption(MessageConversionType.String);

            Ipv4 = string.Empty;
            Port = string.Empty;
            Topic = string.Empty;
            MessageEditorInput = string.Empty;
            MessagePreview = string.Empty;

            IsSaveEnabled = false;
            IsPublishRepeatEnabled = true;

            RepeatIntevalMs = 10;
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
        private int _repeatIntevalMs;

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
        private MessageConversionOption _selectedOption;

        public ObservableCollection<MessageConversionOption> MessageConversionOptions
        {
            get;
            private set;
        }

        #endregion Properties

        #region Commands / Command Definitions

        [RelayCommand]
        private async Task ConfigurePublisherAsync(ConnectionState state)
        {
            await Task.Run(() => ConnectionState = _publisher.Configure(state, Ipv4, Port));
        }

        [RelayCommand]
        private void UploadFile()
        {
            Task.Run(() => StartFileDialog("Text Files (*.txt)|*.txt|JSON Files (*.json)|*.json"));
        }

        [RelayCommand]
        private void SaveFile()
        {
            Task.Run(() => SaveFileDialog());
        }

        [RelayCommand]
        private void PublishOnce()
        {
            _publisher.SendMessage(MessagePreview, Topic);
        }

        [RelayCommand]
        private void ContinuousPublish()
        {
            _repeatTimer = new System.Timers.Timer
            {
                Interval = RepeatIntevalMs
            };
            _repeatTimer.Elapsed += (object sender, ElapsedEventArgs e) => _publisher.SendMessage(MessagePreview, Topic);
            _repeatTimer.Start();

            IsPublishRepeatEnabled = false;
        }

        [RelayCommand]
        private void StopContinuousPublish()
        {
            if (_repeatTimer != null)
            {
                _repeatTimer.Elapsed -= (object sender, ElapsedEventArgs e) => _publisher.SendMessage(MessagePreview, Topic);
                _repeatTimer.Stop();
            }
            IsPublishRepeatEnabled = true;
        }

        [RelayCommand]
        private void WindowClosing()
        {
            if (_repeatTimer != null)
            {
                _repeatTimer.Elapsed -= (object sender, ElapsedEventArgs e) => _publisher.SendMessage(MessagePreview, Topic);
                _repeatTimer.Stop();
                _repeatTimer.Dispose();
            }
            _publisher.ClosePublisher();
        }

        #endregion Commands / Command Definitions

        #region Methods

        /// <summary>
        /// Open file dialog to locate and select an existing file.
        /// </summary>
        /// <param name="dialogFilter"></param>
        private void StartFileDialog(string dialogFilter)
        {
            OpenFileDialog dialog = new()
            {
                Filter = dialogFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                // Load contents of the file into the message editor
                using StreamReader streamReader = new(dialog.FileName, Encoding.UTF8);
                MessageEditorInput = streamReader.ReadToEnd();
            }

            IsSaveEnabled = true;
        }

        /// <summary>
        /// Open file dialog to allow users to save the previewed message.
        /// </summary>
        private void SaveFileDialog()
        {
            string dialogFilter = "Text Files (*.txt)|*.txt";
            switch (SelectedOption.Type)
            {
                case MessageConversionType.String:
                    dialogFilter = "Text Files (*.txt)|*.txt";
                    break;

                case MessageConversionType.JSON:
                    dialogFilter = "JSON Files (*.json)|*.json";
                    break;

                default:
                    break;
            }

            SaveFileDialog dialog = new()
            {
                Filter = dialogFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (dialog.ShowDialog() == true)
            {
                string filePath = dialog.FileName;

                File.WriteAllText(filePath, MessagePreview);
            }
        }

        /// <summary>
        /// Check and update preview message as editor text changes.
        /// </summary>
        /// <param name="value"></param>
        partial void OnMessageEditorInputChanged(string value)
        {
            Tuple<bool, string> conversionCheck = _messageConversionService.Validate(SelectedOption.Type, MessageEditorInput);
            MessageError = conversionCheck.Item1;
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
        /// Check output preview upon switching message conversion type.
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        partial void OnSelectedOptionChanged(MessageConversionOption oldValue, MessageConversionOption newValue)
        {
            Tuple<bool, string> conversionCheck = _messageConversionService.Validate(newValue.Type, MessageEditorInput);
            MessageError = conversionCheck.Item1;
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

        #endregion Methods
    }
}