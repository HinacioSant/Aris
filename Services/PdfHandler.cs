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
using System.IO;
using System.Diagnostics;


namespace Aris.Services
{
    public class PdfHandler

    {
        // EXTRACTOR OF EVERY WORD ON PDF 
        public static IEnumerable<Word> Extractor(string path, int page_number) {
            using var document = Ut.PdfDocument.Open(path);       
            Page page = document.GetPage(page_number); 
           
            var options = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions
            {
                Filter = (pivot, candidate) => // Letter merge prevention that may cause for word merge.
                {
                    if (string.IsNullOrWhiteSpace(candidate.Value))
                        return false;

                    // Don't merge letters from different rendering passes
                    // A large gap in TextSequence means they were drawn separately
                    if (Math.Abs(pivot.TextSequence - candidate.TextSequence) > 6) return false;                        

                    // Don't merge letters with very different font sizes
                    var maxSize = Math.Max(pivot.PointSize, candidate.PointSize);
                    var minSize = Math.Min(pivot.PointSize, candidate.PointSize);
                    if (minSize != 0 && maxSize / minSize > 2.0) return false;

                    return true;
                }
            };

            var extraction = new NearestNeighbourWordExtractor(options);
            var ex  = page.GetWords(extraction).Where(w => !string.IsNullOrWhiteSpace(w.Text));
            var dupliEx = DuplicationHandler([.. ex]);

            return dupliEx;
            
        }

        private static List<Word> DuplicationHandler(List<Word> words)
        {
            var result = new List<Word>();

            foreach (var w in words)
            {
                int overlapIndex = result.FindIndex(accepted => CheckBoxOverlap(accepted.BoundingBox, w.BoundingBox)); 
                
                if (overlapIndex >= 0) 
                {
                    result[overlapIndex] = w;
                }
                else result.Add(w);
            }

            return result;
        }  

        private static bool CheckBoxOverlap(Ut.Core.PdfRectangle rect_a, Ut.Core.PdfRectangle rect_b, double threshold = 2.0)
        {            
            // Calculate the internal rectangle 
            double internalBottom = Math.Max(rect_a.Bottom, rect_b.Bottom);
            double internalTop = Math.Min(rect_a.Top, rect_b.Top);

            if (internalTop > internalBottom)
                return Math.Abs(rect_a.Left - rect_b.Left) <= threshold;
            else return false;
        }

        // REPLACE EVERY WORD ON CHANGE LIST ON PDF
        public static void Replacer(string current_path, string output_path, Dictionary<int, List<Word_Model>> words)
        {  
            var temp_path = Path.Combine(Path.GetDirectoryName(current_path), "temp_{Guid.NewGuid()}.pdf");

            using var reader = new PdfReader(current_path);
            using var writer = new PdfWriter(temp_path);

            using (var pdf_doc = new PdfDocument(reader, writer))
            {
                foreach (var (page_number, changes) in words)
                {
                    var page = pdf_doc.GetPage(page_number);
                    var canvas = new PdfCanvas(page);                    
                    foreach (var w in changes)
                    {                        
                        var pos = w.Position;
                        var font = PdfChanges.GetFont(pos.Font);
                        var x = pos.X;
                        var y = pos.Base_y;
                        var width = pos.Width;
                        var height = pos.Height;
                        var font_size = pos.Font_size;
                        Debug.WriteLine(font_size);

                        canvas.SetFillColor(ColorConstants.WHITE).Rectangle(x, y - 3, width, height + 4).Fill();

                        canvas.SetFillColor(ColorConstants.BLACK).BeginText().SetFontAndSize(font, font_size).MoveText(x, y).ShowText(w.New_Text).EndText();
                    }
                    canvas.Release();
                }
            }
            try
            {
                if (File.Exists(output_path)) File.Delete(output_path);

                  File.Move(temp_path, output_path);
            }

            finally
            {
                if (File.Exists(temp_path)) File.Delete(temp_path);
            }
            
        } 
    }
}