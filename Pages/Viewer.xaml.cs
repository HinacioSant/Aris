using Sw = System.Windows;
using Sw_c = System.Windows.Controls;
using System.Windows.Controls;
using System.Windows;
using Aris.Models;
using Aris.Services;
using System.Windows.Data;



namespace Aris.Pages
{
    public partial class Viewer : Sw_c.UserControl, IDisposable, IPageSizing
    {

        public double PreferredWidth  => SystemParameters.PrimaryScreenWidth  * 0.7;
        public double PreferredHeight => SystemParameters.PrimaryScreenHeight * 0.9;
        
        private PdfiumViewer.PdfViewer _pdfViewer;
        public string _current_path;
        private int _page_number = 1;              
        private readonly ViewerViewModel _viewModel;


        public Viewer(string path)
        {
            InitializeComponent(); 
            _viewModel = new ViewerViewModel();
            DataContext = _viewModel;

            _viewModel.OnPageChange = async n_page => {
                _page_number = n_page;
                await _viewModel.OnPageArrive(n_page);
                Draw_Words();
                };

            _pdfViewer = new PdfiumViewer.PdfViewer
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                ShowToolbar = false,
                ShowBookmarks = false  
            };  

            PdfHost.Child = _pdfViewer;                    
            _current_path = path;
            Loaded += async (s,e) => await Load_Pdf(path);
        }                 
        

        private async Task Load_Pdf(string path)
        {
            try
            {     
                _pdfViewer.Document?.Dispose();
                _pdfViewer.Document = null;
                _pdfViewer.Document = PdfiumViewer.PdfDocument.Load(path);  
            }
            catch (Exception ex)
            {
                var result = ErrorHandler.Handler(ex, nameof(Load_Pdf));                
                Sw.MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
           
            _viewModel.CurrentPath = path;           

            using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
            var page = pigDoc.GetPage(_page_number);
            var total_pages = pigDoc.NumberOfPages;
            _viewModel.PageSelection(total_pages, _page_number); 
            col_0.Width = new GridLength(page.Width);              
            WordCanvas.Height = page.Height;
            
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
               
               var fontSize = pos.Font_size;
                
                var wordBox = new Sw_c.TextBox
                {
                    Text = word.New_Text,
                    FontFamily = GetFont.GetTextBlockFont(pos.Font),  
                    FontWeight = fontWeight,
                    FontStyle = fontStyle,  
                    Height = pos.Height + 10,
                    FontSize = fontSize,
                    BorderThickness = new Thickness(0),                                        
                    Padding = new Thickness(0),                                   
                    IsReadOnly = true,
                    Cursor = Sw.Input.Cursors.Hand,
                    DataContext = word
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
                        wordBox.Focus();  
                    };

                wordBox.LostFocus += (s,e) =>
                    {   
                        wordBox.IsReadOnly = true;                               
                    };

             
                Canvas.SetLeft(wordBox, pos.X);
                Canvas.SetTop(wordBox, pos.Y);
                WordCanvas.Children.Add(wordBox);
            }
        }

        private void CollapseToggle_Checked(object sender, RoutedEventArgs e)
        {
            ChangesList.Visibility = Visibility.Collapsed;
            ChangesBorder.Width = 1; // just enough to show the toggle
            CollapseToggle.Margin = new Sw.Thickness(-43,0,0,0);
            CollapseToggle.Content = "<";
        }

        private void CollapseToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangesList.Visibility = Visibility.Visible;
            ChangesBorder.Width = double.NaN; // reset to auto/star sizing
            CollapseToggle.Margin = new Sw.Thickness(-28,0,0,0);
            CollapseToggle.Content = ">";
        }


        private async void Apply_Changes(object sender, RoutedEventArgs e)
        {
            try
            {                    
                var output_path = PdfChanges.Output_generator(_current_path);  
                var changes = _viewModel.ChangesToApply();
                if (changes.Count == 0)
                {
                    Sw.MessageBox.Show("No Changes to apply.");
                    return;
                } 

                var result = Sw.MessageBox.Show($"Apply {changes.Values.Sum(list => list.Count)} changes", "Confirm", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {     
                    ReleaseDocument();
                    PdfHandler.Replacer(_current_path, output_path, changes);
                    _current_path = output_path;                 
                    await Load_Pdf(_current_path); 
                }
            }
            catch (Exception ex)
            {
                var result = ErrorHandler.Handler(ex, nameof(Apply_Changes));                
                Sw.MessageBox.Show(result.Error, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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

        private void ReleaseDocument()
        {
            _pdfViewer.Document?.Dispose();
            _pdfViewer.Document = null;
        }

        public void Dispose()
        {
            ReleaseDocument();
            _pdfViewer.Dispose();
            GC.SuppressFinalize(this);
        }
        
    }
}