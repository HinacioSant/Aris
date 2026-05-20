using Ut = UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using iText.Kernel.Pdf; 
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using Aris.Pages;

public class PdfHandler

{
    // EXTRACTOR OF EVERY WORD ON PDF 
    public static (IEnumerable<Word>, Page) Extractor(string path, int page_number) {
        using var document = Ut.PdfDocument.Open(path);       
        Page page = document.GetPage(page_number);  
        var letters = page.Letters; 
        var extraction = NearestNeighbourWordExtractor.Instance;
        var ex  = extraction.GetWords(letters).Where(w => !string.IsNullOrWhiteSpace(w.Text));

        return (ex, page);
        
    }

    // SPECIFIC WORD FINDER FOR REPLACMENT 
    public static Finder_Data Word_Finder(IEnumerable<Word> extraction, string word_to_find){    
   
    var word = extraction.FirstOrDefault(w => w.Text == word_to_find);

    if (word == null)
    {
        return null;        
    }

    return new Finder_Data(
        Word:word.Text,
        X:(float)word.BoundingBox.Left,
        Y:(float)word.BoundingBox.Bottom,
        Width:(float)word.BoundingBox.Width,
        Height:(float)word.BoundingBox.Height
        );
    
    }

    // REPLACE SPECIFIC WORD ON PDF
    public static void Replacer(string current_path, string output_path, int page_number, string new_word, Viewer.Word_data word_Data)
    {  

        using var reader = new PdfReader(current_path);
        using var writer = new PdfWriter(output_path);
        using var pdf_doc = new PdfDocument(reader, writer);



        var page = pdf_doc.GetPage(page_number);
        var canvas = new PdfCanvas(page);
        var font = PdfFontFactory.CreateFont(word_Data.Font ?? StandardFonts.HELVETICA);
        var x = word_Data.X;
        var y = word_Data.Base_y;
        var width = word_Data.Width;
        var height = word_Data.Height;
        var font_size = word_Data.Font_size;

        canvas.SetFillColor(ColorConstants.WHITE).Rectangle(x, y, width, height).Fill();

        canvas.SetFillColor(ColorConstants.BLACK).BeginText().SetFontAndSize(font, font_size).MoveText(x, y).ShowText(new_word).EndText();

        canvas.Release();
    }
    



public record Finder_Data(
    string Word,
    float X,
    float Y,
    float Width,
    float Height
    );

}