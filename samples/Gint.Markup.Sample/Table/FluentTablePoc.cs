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
                .AddColumn($"content3")
                    .NewRow()
                    .WithoutRowDivider()
                    .WithRowAlignment(Alignment.Center)
                .AddColumn("hello world1")
                .NewRow()
                    .WithRowAlignment(Alignment.Center)
                .AddColumn("hello world2")
                .Build();

            renderer.Render(Console.Out);
        }
    }

}
