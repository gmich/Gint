using System;

namespace Gint.Markup.Sample
{
    public class TablePoc
    {
        public TablePoc()
        {
            new FluentTablePoc();
            return;

            var table = new Table
            {
                Header = new Header
                {
                    Row = new Row
                    {
                        Columns = new[] { new Column { Content = "header1", SpansOverColumns = 2, Alignment = Alignment.Center }, new Column { Content = "header2..", SkipRowDivider = true }, new Column { Content = "header3.." } }
                    }
                },
                Content = new Content
                {
                    Rows = new[]
                    {
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" ,SpansOverColumns  = 2 },  new Column { Content = "content13.xx." }, new Column { SkipRowDivider=true } }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" ,SpansOverColumns  =4, SkipRowDivider = true} }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" ,SpansOverColumns  =4 } }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" }, new Column { SkipRowDivider=true }, new Column {  }, new Column {} }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content11" ,SkipRowDivider=true}, new Column { Content = "content12.xxxx." }, new Column { Content = "content13.xx." }, new Column {  } }
                        },
                         new Row
                        {
                            Columns = new[] { new Column { Content = "content21.." , SpansOverColumns = 2 }, new Column { Content = "header23.." }, new Column {  } }
                        },
                        new Row
                        {
                            Columns = new[] { new Column { Content = "content21.." , SpansOverColumns = 3 , Alignment = Alignment.End}, new Column { Content = "header23..", SkipRowDivider = true } }
                        },
                    }
                }
            };

            new TableRenderer(table, new TableRenderPreferences { TableRenderMiddleware = new SectionColorerMiddleware() }).Render(Console.Out);
        }
    }

}
