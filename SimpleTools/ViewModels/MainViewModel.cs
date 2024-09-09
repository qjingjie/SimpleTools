using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SimpleTools.Enums;
using SimpleTools.Interfaces;

namespace SimpleTools.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        #region Fields

        private readonly IWindowService _windowService;

        #endregion Fields

        #region Constructor

        public MainViewModel(IWindowService windowService)
        {
            _windowService = windowService;
        }

        #endregion Constructor

        #region Commands / Command Definitions

        [RelayCommand]
        private void OpenWindow(AppSelector appSelector)
        {
            _windowService.OpenWindow(appSelector);
        }

        #endregion Commands / Command Definitions
    }
}