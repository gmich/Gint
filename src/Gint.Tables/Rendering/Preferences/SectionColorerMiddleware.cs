using System;

namespace Gint.Tables
{
    public class SectionColorerMiddleware : ITableRenderMiddleware
    {
        private readonly TableRenderPreferences preferences;

        public SectionColorerMiddleware(TableRenderPreferences preferences)
        {
            this.preferences = preferences;
        }

        public void PostWrite(string text, TableSection section)
        {
            Console.ResetColor();
        }

        public string PreWrite(string text, TableSection section)
        {
            switch (section)
            {
                case TableSection.HeaderColumn:
                    Console.ForegroundColor = preferences.HeaderColor;
                    break;
                case TableSection.ContentColumn:
                    Console.ForegroundColor = preferences.ContentColor;
                    break;
                default:
                    Console.ForegroundColor = preferences.BorderColor; 
                    break;
            }
            return text;
        }
    }

}
