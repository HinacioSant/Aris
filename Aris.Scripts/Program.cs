using Aris.Services;
using Ut = UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using Aris.Models;


const string path = "corpus/small.pdf";
string otp = "corpus/dedup_stress.pdf";
int pageNumber = 1; 
int NumberOfWords = 300; // Words to duplicate

var result = new List<Word_Model>();          
using var pigDoc = UglyToad.PdfPig.PdfDocument.Open(path);
var page = pigDoc.GetPage(pageNumber);            
        
    foreach (var (pos, index) in PdfHandler.Extractor(path, pageNumber).Take(NumberOfWords).Select((pos, i) => (pos, i)))
    {
        var cFont = pos.FontName;
        result.Add( new Word_Model
        {
            ID = index,
            Position  = new Word_data(pos.Text, (float)pos.BoundingBox.Left , (float)(page.Height - pos.BoundingBox.Bottom) , 
                (float)pos.BoundingBox.Width, (float)pos.BoundingBox.Height , cFont ?? "Helvetica", (float)pos.Letters[0].PointSize , (float)pos.BoundingBox.Bottom),                   
            New_Text = pos.Text,
            Og_Text = pos.Text
        });
    }   
    var dict = new Dictionary<int, List<Word_Model>>
    {
        { 1, result }
    };
PdfHandler.Replacer(path, otp, dict);


