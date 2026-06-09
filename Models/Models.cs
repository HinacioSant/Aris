using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aris.Models
{
    public record Word_data(string Text, float X, float Y, float Width, float Height, string Font, float Font_size, float Base_y);

    public class Word_Model : INotifyPropertyChanged    
    {    
        public int ID {get; set;}
        private string? _new_text;    
        public Word_data? Position { get; init;}
        public string? New_Text { get => _new_text; set
            {
                _new_text = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Is_changed));
            }}
        public string? Og_Text { get; init;}
        public bool Is_changed => New_Text != Og_Text;
       

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}