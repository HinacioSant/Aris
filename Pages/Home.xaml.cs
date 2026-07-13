using Mwin = Microsoft.Win32;
using Aris.Services;
using System.Windows;
using Aris.Helpers;
using System.Windows.Input;




namespace Aris.Pages
{
    public partial class Home : IPageSizing
    {       
        public double PreferredWidth  => 390;
        public double PreferredHeight => 195;
        public Home()
        {
            InitializeComponent(); 
            Animation();            
        }

        private void Page_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {  
            if (e.Key == Key.Enter)
            {
                Open_file();                
            }
        }
          
        private static void Open_file ()
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
        
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {           
            this.Focus();
        }
              

        private void Animation()
        {
            // intervalTime must be multiple of totalTime else the animation might end before completed            
            TypingAnimation.Start(HomeText, "Welcome to Aris!", 2.0, onComplete: () =>
            {
                CursorBlinkingAnimation.StarTimed(SecondBlock, intervalTime: 0.5, totalTime: 1.0, onComplete: () =>
                {
                    TypingAnimation.Start(SecondBlock, "Press ENTER to Start", 2.0, onComplete: () =>
                    {
                        CursorBlinkingAnimation.Start(ThirdBlock);
                    });
                });
            });
        }
    }
}