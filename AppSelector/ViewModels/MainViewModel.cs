using AppSelector.Enums;
using AppSelector.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AppSelector.ViewModels
{
    public partial class MainViewModel(IAppLauncher<AppName> appLauncher) : ObservableObject
    {
        #region Fields

        private readonly IAppLauncher<AppName> _appLauncher = appLauncher;

        #endregion Fields

        #region Commands / Command Definitions

        [RelayCommand]
        private void LaunchApp(AppName appName)
        {
            _appLauncher.LaunchApp(appName);
        }

        #endregion Commands / Command Definitions
    }
}