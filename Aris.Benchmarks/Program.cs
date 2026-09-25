using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Aris.Services;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 5, iterationCount: 15)]
[Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
public class PdfExtractionBenchmarks
{
    private const string SmallPdf = "corpus/small.pdf";
    private const string LargePdf = "corpus/large.pdf";  
    private const string OverlappingTextPdf = "corpus/dedup_stress.pdf";

    private void Extract(string path, int pn)
    {        
        PdfHandler.Extractor(path, pn);
    }

    [Benchmark (Baseline = true)]
    public void ExtractSmall() => Extract(SmallPdf,1);

    [Benchmark]
    public void ExtractLarge() => Extract(LargePdf, 12);

    [Benchmark]
    public void ExtractWithDeduplication() => Extract(OverlappingTextPdf, 1);    

    
    
}

public class Program
{
    public static void Main(string[] args) => BenchmarkRunner.Run<PdfExtractionBenchmarks>();
}