using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
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
            var resultSummary =  BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args,
                DefaultConfig.Instance
                    .AddDiagnoser(new EtwProfiler()));

            Console.ReadLine();
        }
    }

}
