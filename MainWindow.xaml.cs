using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Aris.Services;


namespace Aris
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();   

            NavService.On_OpenViewer += OpenViewer;
            NavService.On_GoHome += GoHome;
            Closing += MainWindow_Closing;
            NavService.SetContent(Page_content, new Pages.Home());
        }

        private void OpenViewer(string filepath)
        {
            NavService.SetContent(Page_content, new Pages.Viewer(filepath));
        }

        private void GoHome()
        {
           NavService.SetContent(Page_content, new Pages.Home());
        }

        public void Nav_Viewer(string path)
        {
            NavService.SetContent(Page_content, new Pages.Viewer(path));           
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (Page_content.Content is IPageSizing)
            {
                var isMaximized = WindowState == WindowState.Maximized;
                var (width, height) = isMaximized ? (RestoreBounds.Width, RestoreBounds.Height) : (Width, Height);
                NavService.SizeStore.Save(Page_content.Content.GetType().Name, width, height, isMaximized);
            }
        }
    }
}