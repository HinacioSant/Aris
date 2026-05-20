using Sw = System.Windows;
using Mwin = Microsoft.Win32;



namespace Aris.Pages
{
    public partial class Home
    {
        public Home()
        {
            InitializeComponent();            
        }

        private void Open_file (object sender, Sw.RoutedEventArgs e)
        {
            var dialog = new Mwin.OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Select PDF file"
            };

            if (dialog.ShowDialog() == true)
            {
                var mainWindow = Sw.Application.Current.MainWindow as MainWindow;
                mainWindow?.Nav_Viewer(dialog.FileName);
            }
            
        }

    }
}