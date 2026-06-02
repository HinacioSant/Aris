using Ut = UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using iText.Kernel.Pdf; 
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Aris.Pages;
using Aris.Models;
using Sw_c = System.Windows.Controls;

namespace Aris.Services
{
    public class PdfHandler

    {
        // EXTRACTOR OF EVERY WORD ON PDF 
        public static IEnumerable<Word> Extractor(string path, int page_number) {
            using var document = Ut.PdfDocument.Open(path);       
            Page page = document.GetPage(page_number);  
            var letters = page.Letters; 
            var options = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions{Filter = (pivot, candidate) => 
                {
                // Ignore candidate letters that have the exact same bounding box / location 
                if (pivot.BoundingBox == candidate.BoundingBox && pivot.Value == candidate.Value)
                {
                    return false; // Blocks it from merging into the word
                }
                return true; 
                }};
            var extraction = new NearestNeighbourWordExtractor(options);
            var ex  = page.GetWords(extraction).Where(w => !string.IsNullOrWhiteSpace(w.Text));

            return ex;
            
        }  

        // REPLACE SPECIFIC WORD ON PDF
        public static void Replacer(string current_path, string output_path, int page_number, List<Word_Model> words)
        {  

            using var reader = new PdfReader(current_path);
            using var writer = new PdfWriter(output_path);
            using var pdf_doc = new PdfDocument(reader, writer);
            var page = pdf_doc.GetPage(page_number);
            var canvas = new PdfCanvas(page);


            foreach (var w in words)
            {
                var pos = w.Position;
                var font = PdfFontFactory.CreateFont(pos.Font ?? StandardFonts.HELVETICA);
                var x = pos.X;
                var y = pos.Base_y;
                var width = pos.Width;
                var height = pos.Height;
                var font_size = pos.Font_size;

                canvas.SetFillColor(ColorConstants.WHITE).Rectangle(x, y - 3, width, height + 4).Fill();

                canvas.SetFillColor(ColorConstants.BLACK).BeginText().SetFontAndSize(font, font_size).MoveText(x, y).ShowText(w.New_Text).EndText();
            }
        

            canvas.Release();
        } 

    }
}