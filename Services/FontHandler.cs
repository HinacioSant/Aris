using System.Diagnostics;
using System.IO;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Font;

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
        private static (string FontName, bool IsBuiltIn) RawFontNameHandler(string rawFont)
        {
            int plusIndex = rawFont.IndexOf('+');
            var fontName = plusIndex >= 0 ? rawFont[(plusIndex + 1) ..] : rawFont;            
            bool isBuiltIn = Standard14.Contains(fontName);
            return (fontName, isBuiltIn);       
        }

        private static string? FindLocalFont(string FontName)
        {
            var FontDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");            
            var exact = Path.Combine(FontDir, FontName + ".ttf");

            if (File.Exists(exact)) return exact;

            var candidate = Directory.EnumerateFiles(FontDir, "*.ttf").FirstOrDefault(f => Path.GetFileNameWithoutExtension(f).Contains(FontName, StringComparison.OrdinalIgnoreCase));
                                 
            return candidate;         
        }

        public static PdfFont LoadFont(string rawFont)
        {
            var (FontName, isBuiltIn) = RawFontNameHandler(rawFont);

            if (isBuiltIn)
            {
                return PdfFontFactory.CreateFont(FontName, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            }

            var path = FindLocalFont(FontName);

            try
            {
                var font = PdfFontFactory.CreateFont(path, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);
                Debug.WriteLine($"font created OK: {font}"); 
                return font;
            }          
            catch (Exception ex)
            {
                Debug.WriteLine($"FAILED: {ex.Message} Path:{path}");
                return PdfFontFactory.CreateFont(StandardFonts.HELVETICA, PdfEncodings.WINANSI, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);              
            }
        }
    }
}