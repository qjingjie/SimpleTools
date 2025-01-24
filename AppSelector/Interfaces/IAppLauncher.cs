namespace AppSelector.Interfaces
{
    public interface IAppLauncher<TEnum> where TEnum : Enum
    {
        #region Methods

        /// <summary>
        /// Prepare and launch a desired application.
        /// </summary>
        /// <param name="appName"></param>
        public void LaunchApp(TEnum appName);

        #endregion Methods
    }
}