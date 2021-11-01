using System;

namespace Gint.Tables
{
    public class NoopRenderMiddleware : ITableRenderMiddleware
    {
        public void PostWrite(string text, TableSection section)
        {
        }

        public string PreWrite(string text, TableSection section)
        {
            return text;
        }
    }

}
