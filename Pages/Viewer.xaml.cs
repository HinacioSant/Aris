using Sw = System.Windows;
using Sw_c = System.Windows.Controls;
using Sys_media = System.Windows.Media;
using Sys_shapes = System.Windows.Shapes;
using Sys_media_B = System.Windows.Media.Brushes;
using System.Windows.Controls;
using PdfiumViewer;
using System.Windows.Forms.Integration;
using System.Windows;
using iText.Layout.Element;
using System.Windows.Documents;
using UglyToad.PdfPig.Content;
using System.IO;
using Aris.Models;
using Aris.Services;
using System.ComponentModel;
using System.Windows.Data;



namespace Aris.Pages
{
    public partial class Viewer : Sw_c.UserControl
    {
        private PdfiumViewer.PdfViewer _pdfViewer;
        public string _current_path;
        private int _page_number = 1; 
        private int _total_pages = 1;          
        private readonly ViewerViewModel _viewModel;


        public Viewer(string path)
        {
            InitializeComponent(); 
            _viewModel = new ViewerViewModel();
            DataContext = _viewModel;


            _pdfViewer = new PdfiumViewer.PdfViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ShowToolbar = true,
                ShowBookmarks = false  
            };  

            PdfHost.Child = _pdfViewer;                    
            _current_path = path;

            Load_Pdf(path, _page_number);
           

        }     
            
        

        private void Load_Pdf(string path, int page_number)
        {
            try
            {     

                _pdfViewer.Document = PdfiumViewer.PdfDocument.Load(path);

                using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
                var page = pigDoc.GetPage(page_number);

                _total_pages = pigDoc.NumberOfPages;

                if (PageSelector.Items.Count != _total_pages)
                {
                    for (int i = 1; i<= _total_pages; i++)
                    {
                        PageSelector.Items.Add(i);
                    }
                }

                PageSelector.SelectionChanged -= Page_selector;
                PageSelector.SelectedItem = page_number;
                PageSelector.SelectionChanged += Page_selector;

                PageLabel.Text = $"of {_total_pages}";

                col_0.Width = new GridLength(page.Width);               
                WordCanvas.Height = page.Height;


                _viewModel.LoadPage(path, _page_number); 
                Draw_Words();               
                     
            }
            catch (Exception ex)
            {
                Sw.MessageBox.Show(ex.Message);
            }



        }
        private void Draw_Words()
        {
            WordCanvas.Children.Clear();
            _viewModel.RefreshChangedWords();

            foreach (var word in _viewModel.Words)
            {   
                var pos = word.Position;
                
                var fontWeight = pos.Font.Contains("-Bold", StringComparison.OrdinalIgnoreCase) 
                        ? FontWeights.Bold 
                        : FontWeights.Normal;

                var fontStyle = pos.Font.Contains("-Italic", StringComparison.OrdinalIgnoreCase) 
                            ? FontStyles.Italic 
                            : FontStyles.Normal;
                
                var wordBox = new Sw_c.TextBox
                {
                    Text = word.New_Text,
                    FontFamily = new Sys_media.FontFamily(pos.Font),  
                    FontWeight = fontWeight,
                    FontStyle = fontStyle,                  
                    Width = pos.Width + 7,
                    Height = pos.Height + 10,
                    FontSize = pos.Font_size,
                    BorderThickness = new Thickness(0),
                    Background = Sys_media_B.Transparent,
                    Foreground = Sys_media_B.Black,
                    Padding = new Thickness(0),                   
                    IsReadOnly = true,
                    Cursor = Sw.Input.Cursors.Hand
                };
                
                var binding = new Sw.Data.Binding("New_Text")
                {
                  Source = word,
                  Mode = BindingMode.TwoWay,
                  UpdateSourceTrigger = UpdateSourceTrigger.LostFocus  
                };
                wordBox.SetBinding(Sw_c.TextBox.TextProperty, binding);

                wordBox.PreviewMouseDown += (s,e) =>
                    {
                        wordBox.IsReadOnly = false;
                        wordBox.Background = Sys_media_B.LightYellow;
                        wordBox.Focus();  
                    };

                wordBox.LostFocus += (s,e) =>
                    {   
                        wordBox.IsReadOnly = true;   
                        wordBox.Background = Sys_media_B.Transparent;                        
                        if (word.Is_changed)
                        {
                            wordBox.Background = Sys_media_B.LightGreen;                            
                        }                                             
                    };

                word.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Word_Model.Is_changed)) wordBox.Background = word.Is_changed ? Sys_media_B.LightGreen : Sys_media_B.Transparent; 
                };
               

                Canvas.SetLeft(wordBox, pos.X);
                Canvas.SetTop(wordBox, pos.Y);
                WordCanvas.Children.Add(wordBox);

            }

            
        }



        private void Page_selector(object sender, SelectionChangedEventArgs e)
        {
            if (PageSelector.SelectedItem == null) return;

            _page_number = (int)PageSelector.SelectedItem;
            Load_Pdf(_current_path, _page_number);
        }

        private void Apply_Changes(object sender, RoutedEventArgs e)
        {
            var output_path = PdfChanges.Output_generator(_current_path);            

            var changes = _viewModel.ChangedWords.ToList();
            if (changes.Count == 0)
            {
                Sw.MessageBox.Show("No Changes to apply.");
                return;
            } 

            var result = Sw.MessageBox.Show($"Apply {changes.Count} changes", "Confirm", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {     

                PdfHandler.Replacer(_current_path, output_path, _page_number, changes);
                _current_path = output_path;                 
                Load_Pdf(_current_path, _page_number);   

            }

        }      

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            var word = (sender as Sw_c.Button)?.Tag as Word_Model;           
            _viewModel.Undo(word);
            

        }

        private void Go_back(object sender, RoutedEventArgs e)
        {
            NavService.GoHome();
        }
        
    }
}