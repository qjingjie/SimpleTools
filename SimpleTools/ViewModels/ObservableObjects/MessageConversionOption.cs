using CommunityToolkit.Mvvm.ComponentModel;
using SimpleTools.Enums;

namespace SimpleTools.ViewModels.ObservableObjects
{
    public partial class MessageConversionOption : ObservableObject
    {
        #region Constructor

        public MessageConversionOption(MessageConversionType conversionType)
        {
            DisplayText = conversionType.ToString();
            Type = conversionType;
        }

        #endregion Constructor

        #region Properties

        [ObservableProperty]
        private string _displayText;

        [ObservableProperty]
        private MessageConversionType _type;

        #endregion Properties
    }
}