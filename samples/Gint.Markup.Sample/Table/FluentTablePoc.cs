using System;

namespace Gint.Markup.Sample
{
    public class FluentTablePoc
    {
        public FluentTablePoc()
        {
            var renderer = GintTable.WithFirstRowAsHeader()
                    .WithRowAlignment(Alignment.Center)
                .AddColumn("header1")
                .AddColumn("header2")
                .AddColumn("header3")
                    .NewRow()
                    .WithRowAlignment(Alignment.Start)
                .AddColumn("content1")
                .AddColumn("content2")
                .AddColumn($"con{Environment.NewLine}tent3")
                    .NewRow()
                    .WithoutRowDivider()
                    .WithRowAlignment(Alignment.Center)
                .AddColumn($"hello world1 { Environment.NewLine}  New line")
                .NewRow()
                    .WithRowAlignment(Alignment.Center)
                .AddColumn("hello world2")
                .Build();

            renderer.Render(Console.Out);
        }
    }

}
