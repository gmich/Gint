using System;

namespace Gint
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FormatMaskAttribute : Attribute
    {
        public FormatMaskAttribute(string mask)
        {
            Mask = mask;
        }

        public string Mask { get; }
    }


}
