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



namespace Aris.Pages
{
    public partial class Viewer
    {
        private PdfiumViewer.PdfViewer _pdfViewer;
        private string _current_path;
        private int _page_number = 1; 
        private int _total_pages = 1;       



        public Viewer(string path)
        {
            InitializeComponent();   


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
                
                WordCanvas.Children.Clear();

                _pdfViewer.Document = PdfiumViewer.PdfDocument.Load(path);     
                
                          


                var extraction = PdfHandler.Extractor(path, page_number);

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
                
                               


               
                
                foreach (var word in extraction)
                {
                    var font_name = word.FontName ?? "Arial";

                    Draw_coordinates(
                        word.Text,
                        font_name,
                        (float)word.Letters[0].FontSize,
                        (float)word.BoundingBox.Left,
                        (float)(page.Height - word.BoundingBox.Bottom ),
                        (float)word.BoundingBox.Width,
                        (float)word.BoundingBox.Height,
                        (float)word.BoundingBox.Bottom  
                        );
                    
                }     
            }
            catch (Exception ex)
            {
                Sw.MessageBox.Show(ex.Message);
            }



        }

        private void Draw_coordinates(string text, string font, float font_size ,float x, float y, float width, float height, float base_y)
        {
            var fontWeight = font.Contains("-Bold", StringComparison.OrdinalIgnoreCase) 
                        ? FontWeights.Bold 
                        : FontWeights.Normal;

            var fontStyle = font.Contains("-Italic", StringComparison.OrdinalIgnoreCase) 
                        ? FontStyles.Italic 
                        : FontStyles.Normal;

            
            
            var wordBox = new Sw_c.TextBox
            {
                Text = text,
                FontFamily = new Sys_media.FontFamily(font),  
                FontWeight = fontWeight,
                FontStyle = fontStyle,                  
                Width = width + 7,
                Height = height + 10,
                FontSize = font_size,
                BorderThickness = new Thickness(0),
                Background = Sys_media_B.Transparent,
                Foreground = Sys_media_B.Black,
                Padding = new Thickness(0),
                Tag = new Word_data(text, x, y, width, height, font, font_size, base_y),
                IsReadOnly = true,
                Cursor = Sw.Input.Cursors.Hand
            };
            
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

                var position = wordBox.Tag as Word_data;
                if (position == null) return;

                if (wordBox.Text != position.Text)
                {
                    wordBox.Background = Sys_media_B.LightGreen;  
                }

                Refresh_Cl();

            };



            Canvas.SetLeft(wordBox, x);
            Canvas.SetTop(wordBox, y);
            WordCanvas.Children.Add(wordBox);  

        }


        private void Page_selector(object sender, SelectionChangedEventArgs e)
        {
            if (PageSelector.SelectedItem == null) return;

            _page_number = (int)PageSelector.SelectedItem;
            Load_Pdf(_current_path, _page_number);
        }



        private IEnumerable<Sw_c.TextBox> Changed_words()
        {
            return WordCanvas.Children.OfType<Sw_c.TextBox>().Where(box =>
            {
                var position = box.Tag as Word_data;
                return position != null && box.Text != position.Text;
            });
        }
        

        private void Apply_Changes(object sender, RoutedEventArgs e)
        {
            var changes = Changed_words().ToList();

            var output_path = System.IO.Path.Combine(Path.GetDirectoryName(_current_path),"[Edited]-" + System.IO.Path.GetFileName(_current_path));  

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
                Refresh_Cl();

                
            }       
            
        }

        private void Refresh_Cl()
        {
            var changes = Changed_words().Select(box =>
            {
                var pos = box.Tag as Word_data;
                return new
                {
                    Original = pos?.Text,
                    New = box.Text,
                    Position = $"({pos?.X:F0}, {pos?.Y:F0})",
                    tag_pos = pos
                };
            }).ToList();

            ChangesList.ItemsSource = changes;            
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Sw_c.Button;
            var tag_pos = button?.Tag as Word_data;
            if (tag_pos == null) return;

            var box = Changed_words().FirstOrDefault(b =>
            {
                var pos = b.Tag as Word_data;
                return pos?.X == tag_pos?.X && pos?.Y == tag_pos?.Y;
            });

            if (box != null)
            {
                box.Text = tag_pos.Text;
                box.Background = Sys_media_B.Transparent;
            }

            Refresh_Cl();

        }

        private void Toggle_changes(object sender, RoutedEventArgs e)
        {
            var button = sender as Sw_c.Button;
            if (ChangesBorder.Visibility == Visibility.Visible)
            {
                ChangesBorder.Visibility = Visibility.Collapsed;
                ChangesPanel.Visibility = Visibility.Collapsed;
                button.Content = "Changes ▼";
            }
            else
            {
                ChangesBorder.Visibility = Visibility.Visible;
                ChangesPanel.Visibility = Visibility.Visible;

                button.Content = "Changes ▲";

            }
        }


        private void Go_back(object sender, RoutedEventArgs e)
        {
            NavService.GoHome();
        }

        public record Word_data(string Text, float X, float Y, float Width, float Height, string Font, float Font_size, float Base_y);
    }
}