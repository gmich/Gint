using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.CsProj;
using System;

namespace Gint.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //var markupSummary = BenchmarkRunner.Run<MarkupBenchmark>();
            var pipesSummary = BenchmarkRunner.Run<PipesBenchmark>();
            Console.ReadLine();
        }
    }

}
