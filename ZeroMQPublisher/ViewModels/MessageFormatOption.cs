using CommunityToolkit.Mvvm.ComponentModel;
using ZeroMQPublisher.Enums;

namespace ZeroMQPublisher.ViewModels
{
    public partial class MessageFormatOption : ObservableObject
    {
        #region Constructor

        public MessageFormatOption(MessageFormat messageFormat, string messageFormatFilter)
        {
            MessageFormatString = messageFormat.ToString();
            MessageFormatType = messageFormat;
            MessageFormatFilter = messageFormatFilter;
        }

        #endregion Constructor

        #region Properties

        [ObservableProperty]
        private string _messageFormatString;

        [ObservableProperty]
        private MessageFormat _messageFormatType;

        [ObservableProperty]
        private string _messageFormatFilter;

        #endregion Properties
    }
}