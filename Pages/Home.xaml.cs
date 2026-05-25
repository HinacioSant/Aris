using Sw = System.Windows;
using Mwin = Microsoft.Win32;
using Aris.Services;




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
                NavService.OpenViewer(dialog.FileName);
            }
            
        }

    }
}