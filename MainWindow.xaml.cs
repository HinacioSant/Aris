using System.Windows;


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
            Width  = SystemParameters.PrimaryScreenWidth  * 0.3;
            Height = SystemParameters.PrimaryScreenHeight * 0.85;

            Page_content.Content = new Pages.Home();

        }

        public void Nav_Viewer(string path)
        {
            Page_content.Content = new Pages.Viewer(path);
        }
    }
}