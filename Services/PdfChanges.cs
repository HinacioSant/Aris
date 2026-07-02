using System.IO;


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
    }
}

