using System.Windows;
using System.Windows.Controls;
using Aris.Services;
using System.Diagnostics;


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

            Width  = SystemParameters.PrimaryScreenWidth  * 0.3;
            Height = SystemParameters.PrimaryScreenHeight * 0.85;

            Page_content.Content = new Pages.Home();

        }

        private void OpenViewer(string filepath)
        {
            if (Page_content.Content is Pages.Viewer existingViewer) existingViewer.Dispose();
            Page_content.Content = new Pages.Viewer(filepath);
        }

        private void GoHome()
        {
            if (Page_content.Content is Pages.Viewer existingViewer) existingViewer.Dispose();
            Page_content.Content = new Pages.Home();
        }

        public void Nav_Viewer(string path)
        {
            Page_content.Content = new Pages.Viewer(path);
        }
    }
}