using Aris.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;


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
                _Selected_Page = value; 
                OnPropertyChanged();                 
                OnPageChange?.Invoke(value);                  
            }
        }       


        public async Task<Result> LoadPage(int pageNumber)
        {
            try
            {                
                Words.Clear();
                var path = CurrentPath;
                using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
                var page = pigDoc.GetPage(pageNumber);     
                var total_pages = pigDoc.NumberOfPages;
                PageSelection(total_pages, pageNumber); 

                return Result.Ok(); 
            }

            catch (Exception ex)
            {
                return ErrorHandler.Handler(ex, nameof(LoadPage));
            }

        }

        public void RefreshChangedWords()
        {
            ChangedWords.Clear();
            foreach (var w in Words.Where(w => w.Is_changed))
                ChangedWords.Add(w);

            PendingLabel = $"[{ChangedWords.Count}]-Pending Changes";
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
      
        public async Task OnPageArrive(int page)
        {
            await PopulateWords(page);

            if (_ChangesPerPage.TryGetValue(page, out var saved))
            {
                foreach (var c in saved)
                {
                    var old_word = Words.FirstOrDefault(w => w.ID == c.ID);
                    old_word?.New_Text = c.New_Text; 
                }
            }
           RefreshChangedWords();
        }

        public Dictionary<int, List<Word_Model>> ChangesToApply()
        {
            var changes = new Dictionary<int, List<Word_Model>>(_ChangesPerPage);
            if (ChangedWords.Any()) changes[SelectedPage] = ChangedWords.ToList();          

            return changes;
        }
        private List<Word_Model> ExtractorHandler(int pageNumber)
        {
            var result = new List<Word_Model>();
            var path = CurrentPath;
            using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
            var page = pigDoc.GetPage(pageNumber);            
               
            foreach (var (pos, index) in PdfHandler.Extractor(path, pageNumber).Select((pos, i) => (pos, i)))
            {
                var cFont = pos.FontName;
                result.Add( new Word_Model
                {
                    ID = index,
                    Position  = new Word_data(pos.Text, (float)pos.BoundingBox.Left, (float)(page.Height - pos.BoundingBox.Bottom), 
                        (float)pos.BoundingBox.Width, (float)pos.BoundingBox.Height, cFont ?? "Helvetica", (float)pos.Letters[0].PointSize, (float)pos.BoundingBox.Bottom),                   
                    New_Text = pos.Text,
                    Og_Text = pos.Text
                });
            }
            return result;
        }

        public async Task PopulateWords(int pageNumber)
        {
            Words.Clear();
            var extraction = await Task.Run(() => ExtractorHandler(pageNumber));
            foreach (var word in extraction)
            {
                 // listen for changes on each word
                word.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(Word_Model.Is_changed))
                        RefreshChangedWords();
                };                
                Words.Add(word);   
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}