using System;
using System.ComponentModel;
using System.Reflection;

namespace Sorlov.PowerShell
{
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static T FromString<T>(string value)
          {
             return (T) Enum.Parse(typeof(T),value);
          }

    }
}
