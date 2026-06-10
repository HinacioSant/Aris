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
        public string? CurrentPath {get; set;}
        public ObservableCollection<Word_Model> Words { get; } = new();
        public ObservableCollection<Word_Model> ChangedWords { get; } = new();
        private Dictionary<int, List<Word_Model>> _ChangesPerPage = new();
        public ObservableCollection<int> Pages { get; }= new();
        public Action<int>? OnPageChange { get; set; }

        private string? _pendingLabel;
        public string? PendingLabel
        {
            get => _pendingLabel;
            set { _pendingLabel = value; OnPropertyChanged();                               
            }
        }

        private bool _hasChanges;
        public bool HasChanges
        {
            get => _hasChanges;
            set { _hasChanges = value; OnPropertyChanged();}
        }

        private int _Selected_Page;

        public int SelectedPage
        {
            get => _Selected_Page;
            set
            {
                if (_Selected_Page == value || value < 1) return;
                if (_Selected_Page > 0) OnPageLeave(_Selected_Page);
                _Selected_Page = value; OnPropertyChanged(); 
                OnPageArrive(value);  
                OnPageChange?.Invoke(value);                  
            }
        }
        


        public void LoadPage(int pageNumber)
        {
            Words.Clear();
            var path = CurrentPath;
            using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
            var page = pigDoc.GetPage(pageNumber);            
               
            foreach (var (pos, index) in PdfHandler.Extractor(path, pageNumber).Select((pos, i) => (pos, i)))
            {
                var word = new Word_Model
                {
                    ID = index,
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

        public void PageSelection(int total_pages, int selected_page)
        {
            Pages.Clear();
            for (int i =1; i <= total_pages; i++)
            {
                Pages.Add(i);
            }
            _Selected_Page = 0; // buffer so page can load before calling selectedPage | else selected page number may appear blank
            SelectedPage = selected_page;
            _ChangesPerPage.Clear();
        }

        public void OnPageLeave(int page) // Holds Previous page changed words on page leave
        {
            _ChangesPerPage[page] = ChangedWords.ToList();
        }
        public void OnPageArrive(int page)
        {
            LoadPage(page);
            var saved = new ObservableCollection<Word_Model>(_ChangesPerPage.TryGetValue(page, out var list) ? list : Enumerable.Empty<Word_Model>());
            ChangedWords.Clear();
            foreach (var c in saved)
            {
                ChangedWords.Add(c);
                var old_words = Words.Where(w => w.ID == c.ID);
                foreach (var w in old_words)
                {
                    w.New_Text = c.New_Text;                    
                }
            }
        }

        public Dictionary<int, List<Word_Model>> ChangesToApply()
        {
            var changes = new Dictionary<int, List<Word_Model>>(_ChangesPerPage);
            if (ChangedWords.Any()) changes[SelectedPage] = ChangedWords.ToList();          

            return changes;
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}