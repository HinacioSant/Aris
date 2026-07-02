using System.Diagnostics;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Font;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Aris.Services
{
    public static class GetFont
    { 
        private static readonly HashSet<string> Standard14 = new(StringComparer.OrdinalIgnoreCase)
        {
            "Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique",
            "Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic",
            "Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique",
            "Symbol", "ZapfDingbats"
        };   

        private static Dictionary<string, string>? _manifest;

        private static readonly string FontDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts"); 

        private static void LoadManifest()
        {
            var path = Path.Combine(FontDir, "manifest.json");
            _manifest = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path));
        }

        private static string Normalize(string s) => Regex.Replace(s, @"[^a-zA-Z0-9]", "").ToLowerInvariant();
        static string StripSize(string s) => Regex.Replace(s, @"\d+pt", "", RegexOptions.IgnoreCase);


        private static string? SeachLocalFont(string FontName)
        {
            LoadManifest();
            var key = Normalize(StripSize(FontName));
            var match = _manifest.FirstOrDefault(kvp => Normalize(StripSize(kvp.Key)) == key);

            return match.Value != null ? Path.Combine(FontDir, match.Value) : null;
        }

        private static (string FontName, bool IsBuiltIn) RawFontNameHandler(string rawFont)
        {
            int plusIndex = rawFont.IndexOf('+');
            var fontName = plusIndex >= 0 ? rawFont[(plusIndex + 1) ..] : rawFont;            
            bool isBuiltIn = Standard14.Contains(fontName);
            return (fontName, isBuiltIn);       
        }

        public static PdfFont LoadFont(string rawFont)
        {
            var (FontName, isBuiltIn) = RawFontNameHandler(rawFont);

            if (isBuiltIn)
            {
                return PdfFontFactory.CreateFont(FontName, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            }   

            var path = SeachLocalFont(FontName); 
            
            try
            {
                var font = PdfFontFactory.CreateFont(path, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                Debug.WriteLine($"font created OK: {font}"); 
                return font;
            }          
            catch (Exception ex)
            {
                Debug.WriteLine($"FAILED: {ex.Message} Path:{path} FontName:{FontName}");
                return PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);              
            }
        }
    }
}