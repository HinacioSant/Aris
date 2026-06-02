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
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Aris.Pages;

namespace Aris.Services
{   
    public class ViewerViewModel : INotifyPropertyChanged
    {
        private List<Word_Model> _words = new();
        public ObservableCollection<Word_Model> Words { get; } = new();
        public ObservableCollection<Word_Model> ChangedWords { get; } = new();

        private string _pendingLabel;
        public string PendingLabel
        {
            get => _pendingLabel;
            set { _pendingLabel = value; OnPropertyChanged(); }
        }

        private bool _hasChanges;
        public bool HasChanges
        {
            get => _hasChanges;
            set { _hasChanges = value; OnPropertyChanged();}
        }

        public void LoadPage(string path, int pageNumber)
        {
            Words.Clear();
            using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
            var page = pigDoc.GetPage(1);

            foreach (var pos in PdfHandler.Extractor(path, pageNumber))
            {
                var word = new Word_Model
                {
                    Position    = new Word_data(pos.Text, (float)pos.BoundingBox.Left, (float)(page.Height - pos.BoundingBox.Bottom), 
                        (float)pos.BoundingBox.Width, (float)pos.BoundingBox.Height, pos.FontName ?? "Arial", (float)pos.Letters[0].FontSize, (float)pos.BoundingBox.Bottom),                   
                    New_Text = pos.Text,
                    Og_Text = pos.Text
                };

                // listen for changes on each word
                word.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Word_Model.Is_changed))
                        RefreshChangedWords();
                };

                Words.Add(word);
            }
        }

        public void RefreshChangedWords()
        {
            ChangedWords.Clear();
            foreach (var w in Words.Where(w => w.Is_changed))
                ChangedWords.Add(w);

            PendingLabel = $"{ChangedWords.Count} pending changes";
            HasChanges = ChangedWords.Count > 0; 
                      
        }

        public void Undo(Word_Model word)
        {
            word.New_Text = word.Og_Text;
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}