using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Arkivverket.Arkade.Core.Util
{
    public static class EnumExtensions
    {
        public static string[] GetDescriptions(this Type enumType)
        {
            return (from object value in Enum.GetValues(enumType) select value.GetDescription()).ToArray();
        }

        public static string GetDescription<T>(this T enumValue)
        {
            string description = ((DescriptionAttribute)enumValue.GetType().GetField(enumValue.ToString()!)?
                .GetCustomAttributes(typeof(DescriptionAttribute), true)[0])?.Description;

            return description ?? throw new ArgumentException(@"Description not found", nameof(enumValue));
        }

        public static T GetValueByDescription<T>(this string description)
        {
            foreach (FieldInfo field in typeof(T).GetFields())
                if (FieldHasDescription(description, field))
                    return (T)field.GetValue(null);

            throw new ArgumentException(@"Description not found", nameof(description));
        }

        public static bool TryParseFromDescription<TEnum>(this string description, out TEnum enumValue)
        {
            try
            {
                enumValue = description.GetValueByDescription<TEnum>();
                return true;
            }
            catch
            {
                enumValue = default;
                return false;
            }
        }

        public static bool HasValueForDescription<T>(this string description)
        {
            return typeof(T).GetFields().Any(field => FieldHasDescription(description, field));
        }

        private static bool FieldHasDescription(string description, MemberInfo field)
        {
            var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return customAttribute is DescriptionAttribute attribute && attribute.Description == description;
        }
    }
}
