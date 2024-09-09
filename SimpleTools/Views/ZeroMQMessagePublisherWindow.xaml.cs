using System.Windows;
using System.Windows.Input;

namespace SimpleTools.Views
{
    /// <summary>
    /// Interaction logic for ZeroMQMessagePublisherWindow.xaml
    /// </summary>
    public partial class ZeroMQMessagePublisherWindow : Window
    {
        #region Fields

        //private readonly MainViewModel _mainViewModel;

        #endregion Fields

        #region Constructor

        public ZeroMQMessagePublisherWindow()
        {
            //Application.Current.MainWindow = this;

            InitializeComponent();

            DefaultStyleKey = typeof(ZeroMQMessagePublisherWindow);
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, CloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, MaximizeWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, MinimizeWindow, CanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, RestoreWindow, CanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.ShowSystemMenuCommand, ShowSystemMenu));

            //_mainViewModel = ((App)Application.Current).ServiceProvider.GetRequiredService<MainViewModel>();
            //DataContext = _mainViewModel;
        }

        #endregion Constructor

        #region Methods For Custom Window

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (SizeToContent == SizeToContent.WidthAndHeight)
            {
                InvalidateMeasure();
            }
        }

        private void CanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void CanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }

        private void CloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void MaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void MinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void RestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        private void ShowSystemMenu(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is not FrameworkElement element)
            {
                return;
            }

            Point point = WindowState == WindowState.Maximized ? new Point(0, element.ActualHeight) : new Point(Left + BorderThickness.Left, element.ActualHeight + Top + BorderThickness.Top);
            point = element.TransformToAncestor(this).Transform(point);
            SystemCommands.ShowSystemMenu(this, point);
        }

        #endregion Methods For Custom Window
    }
}