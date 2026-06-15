using Aris.Models;
using Aris.Pages;
using System.IO;
using System.Windows;


namespace Aris.Services

{
   public class PdfChanges
   {    

    private const string Edited_Prefix = "[Edited]-";
    public static string Output_generator(string current_path)
        {
            var directory = Path.GetDirectoryName(current_path);
            var filename = Path.GetFileName(current_path);

            if (filename.StartsWith(Edited_Prefix)) filename = filename[Edited_Prefix.Length..];

            return Path.Combine(directory, Edited_Prefix + filename);
        }
    public static IEnumerable<Word_Model> Changed_words(List<Word_Model> words)
        {
            return words.Where(w => w.Is_changed);
        }  
    }
}

