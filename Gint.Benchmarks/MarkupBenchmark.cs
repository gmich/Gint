using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using Gint.Markup;

namespace Gint.Benchmarks
{
    [SimpleJob(RunStrategy.Throughput)]
    //[MonoJob, SimpleJob(RuntimeMoniker.Net461), SimpleJob(RuntimeMoniker.NetCoreApp31)]
    //[MinColumn, MaxColumn, MeanColumn, MedianColumn]
    [MemoryDiagnoser]
    public class MarkupBenchmark
    {
        //private readonly ConsoleMarkupWriter consoleMarkupWriter = new ConsoleMarkupWriter();
        
        private string text =
            @"[fg:red,bg:white]hello world[-bg:white]!!!!!![-fg:red]
                [~br,~br]
                [fg:green]hello world[-fg:green]!!!!!!";

        private string bigText =
    @"[fg:red,bg:white]hello world[-bg:white]!!!!!![-fg:red]
                [~br,~br]
                [fg:green]hello world[-fg:green]!!!!!!
[fg:red,bg:white]hello world[-bg:white]!!!!!![-fg:red]
                [~br,~br]
                [fg:green]hello world[-fg:green]!!!!!!
[fg:red,bg:white]hello world[-bg:white]!!!!!![-fg:red]
                [~br,~br]
                [fg:green]hello world[-fg:green]!!!!!!";


        public MarkupBenchmark() { }

        //[Benchmark]
        //public void ConsoleMarkupWriter() => consoleMarkupWriter.Print(text);

        [Benchmark]
        public MarkupSyntaxToken[] LinterWithFormat() => MarkupLinter.Lint(text, out var diagnostics);

        [Benchmark]
        public MarkupSyntaxToken[] LinterWithALotOfFormat() => MarkupLinter.Lint(bigText, out var diagnostics);

        [Benchmark]
        public MarkupSyntaxToken[] LinterNoFormat() => MarkupLinter.Lint("hello world!", out var diagnostics);
    }
}
