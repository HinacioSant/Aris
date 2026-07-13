using Sw = System.Windows;
using Mwin = Microsoft.Win32;
using Aris.Services;
using System.Windows;
using Aris.Helpers;




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