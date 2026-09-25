# Benchmarks

Performance and correctness benchmarks for the PDF extraction pipeline (PdfPig-based), run via [BenchmarkDotNet](https://benchmarkdotnet.org/) against a fixed test corpus. Full harness in `Aris.Benchmarks/`.

## Test corpus

Corpus files are sized to reflect realistic upper bounds rather than arbitrary small/large labels:

| File | Pages | Word count | Description |
|---|---|---|---|
| `small.pdf` | 1 | 731 | Baseline reference file — near the practical maximum word density for a single page at 11pt font |
| `large.pdf` | 700 | — | Book-length document — represents realistic upper-bound page count |
| `dedup_stress.pdf` | 1 | 731 (+300 duplicated) | Same baseline density, with 300 words duplicated at matching spatial coordinates to stress-test deduplication |

Because `small.pdf` is already close to max word density per page, `large.pdf` effectively represents a worst-case scenario for both dimensions combined — 700 pages each near maximum text density.

## Extraction performance

| File | Mean time | vs. baseline | Allocated | vs. baseline |
|---|---|---|---|---|
| `small.pdf` | 43.81 ms | baseline | 9.98 MB | baseline |
| `large.pdf` | 50.68 ms | +15.7% | 24.73 MB | +147.8% |
| `dedup_stress.pdf` | 55.94 ms | +27.7% | 13.02 MB | +30.5% |

**Takeaway:** even at book-length scale (700 near-max-density pages), extraction time increases only 15.7% over a single page — page count is not a significant time bottleneck. Memory scales more proportionally with page count (+147.8%), as expected. Duplicate-word matching, by contrast, drives time cost independent of page count — a single-page file with 300 duplicated words costs more in time than the 700-page book, despite far less memory use.

## Deduplication scaling

| Duplicate words | % of baseline word count | Time cost | Allocation cost |
|---|---|---|---|
| 61 | ~8% | +2.7% | +7.0% |
| 300 | ~41% | +27.7% | +30.5% |

Cost-per-duplicate-word roughly tripled between the two test points, suggesting mildly super-linear scaling. Not yet confirmed as quadratic — a third data point at a higher duplicate ratio would be needed to verify. However a 731 word page with additional 300 edited words is far from the average use of Aris. ~So at this moment it SEEMS fine to maintain as it is. Also the intended deduplication fix should also improve this overall performance.

## Deduplication accuracy

| Condition | Result | Rate |
|---|---|---|
| Default font size | 3 duplicates unmerged out of 300 | 1.0% miss rate |
| Reduced font size (−3pt) | 1 false-positive merge | Over-aggressive at tighter spacing |

**Known limitation:** matching tolerance is currently a fixed value rather than proportional to font size, causing accuracy to shift with text scale. A punctuation-boundary edge case has also been identified as the likely root cause of the default-font misses. Fix planned.

---
*Benchmarks run on [Ryzen 3 2200g - 16gb DDR4 Ram] / .NET [10.0], last updated [24/09/2026].*

## Reproducing these results

All benchmark, test, and test-data-generation code lives in this repository as sibling projects to `Aris.csproj`.
```
You will need to add you own "small.pdf" "large.pdf" files to each corpus. For the "dedup_stress.pdf" you can use Aris.Scripts to make it or change manually using Aris.

File that I used was "The Iliad Homer; Robert Fagles(translation)"
"small.pdf" - Page 12 only
"large.pdf" - The whole 700 pages
"dedup_stress" - Page 12 with 300 duplicated words
```

### Setup

```bash
git clone https://github.com/HinacioSant/Aris.git
cd Aris
dotnet restore
```

### Run performance benchmarks

```bash
cd Aris.Benchmarks
dotnet run -c Release
```

Results are printed to console and saved to `Aris.Benchmarks/BenchmarkDotNet.Artifacts/results/`.

### Run correctness tests (deduplication accuracy, etc.)

```bash
cd Aris.Tests
dotnet test
```

### Regenerate the test corpus

The `dedup_stress.pdf` file is generated programmatically — duplicate words are stamped onto a copy of `small.pdf` at matching coordinates.

```bash
cd Aris.Scripts
dotnet run
```

Edit `DuplicateCount` and `Seed` in `Aris.Scripts/Program.cs` to generate different stress-test variants.

### Requirements

- .NET SDK [10.0.201]
- Windows (project targets `net10.0-windows`)