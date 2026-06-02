using Aris.Models;
using Aris.Pages;
using System.IO;
using System.Windows;


namespace Aris.Services

{
   public class PdfChanges
   {

    
    public static string Output_generator(string current_path)
        {
            
            var output_path = System.IO.Path.Combine(Path.GetDirectoryName(current_path),"[Edited]-" + System.IO.Path.GetFileName(current_path));  
            return output_path;
        }
    public static IEnumerable<Word_Model> Changed_words(List<Word_Model> words)
        {
            return words.Where(w => w.Is_changed);
        }  
    }
}

