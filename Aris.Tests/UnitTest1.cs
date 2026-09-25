namespace Aris.Tests;
using Aris.Services;

public class UnitTest1
{
   private static readonly string SmallPath = 
    Path.Combine(AppContext.BaseDirectory, "corpus", "small.pdf");
private static readonly string DedupStressPath = 
    Path.Combine(AppContext.BaseDirectory, "corpus", "dedup_stress.pdf");

[Fact]
public void Dedup_RemovesAllDuplicates_MatchingBaselineWordCount()
{
    var baseline = PdfHandler.Extractor(SmallPath, 1);
    var stressed = PdfHandler.Extractor(DedupStressPath, 1);

    Assert.Equal(baseline.Count(), stressed.Count());
}
}
