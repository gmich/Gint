using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gint
{
    internal static class FormatRegistry
    {
        private static Dictionary<FormatType, string> FormatEnumToMask { get; set; }
        private static Dictionary<string, FormatType> MaskToFormatType { get; set; }

        private static FormatMaskAttribute GetAttribute(FormatType formatType)
        {
            var type = formatType.GetType();
            var memInfo = type.GetMember(formatType.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(FormatMaskAttribute), false);
            return (attributes.Length > 0) ? ((FormatMaskAttribute)attributes[0]) : null;
        }

        static FormatRegistry()
        {
            var formatAttributes =
            Enum.GetValues(typeof(FormatType)).Cast<FormatType>()
                .Select(c => new
                {
                    Type = c,
                    Attribute = GetAttribute(c)
                })
                .Where(c => c.Attribute != null)
                .ToArray();

            FormatEnumToMask = formatAttributes
                .ToDictionary(
                    c => c.Type,
                    c => c.Attribute.Mask
                );

            MaskToFormatType = formatAttributes
                .ToDictionary(
                    c => c.Attribute.Mask,
                    c => c.Type
                );
        }


        public static string GetFormatMaskWithPrefix(FormatType format)
        {
            return $"{FormatFacts.MaskPrefix}{FormatEnumToMask[format]}";
        }

        public static FormatType GetFormatType(string maskWithoutPrefix)
        {
            if(MaskToFormatType.ContainsKey(maskWithoutPrefix))
            {
                return MaskToFormatType[maskWithoutPrefix];
            }
            return FormatType.Custom;
        }

    }


}
