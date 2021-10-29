namespace Gint.Markup.Sample
{
    public class ContinousTableDividerStyle : ITableDividerStyle
    {
        public char Get(TableDividerPart divider)
        {
            return divider switch
            {
                TableDividerPart.HeaderColumn => '│',
                TableDividerPart.HeaderRow => '─',
                TableDividerPart.ContentColumn => '│',
                TableDividerPart.ContentRow => '─',
                _ => throw new System.ArgumentException($"Unknown divider style {divider}"),
            };
        }
    }
}