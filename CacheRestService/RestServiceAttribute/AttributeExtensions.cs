using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Modified from https://stackoverflow.com/questions/2656189/how-do-i-read-an-attribute-on-a-class-at-runtime#answer-2656211
/// </summary>
namespace Ib.Xamarin.CacheUtils.CacheRestService.RestServiceAttribute
{
    internal static class AttributeExtensions
    {
        internal static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        {
            Type gta = type.GenericTypeArguments.FirstOrDefault();

            TAttribute att;
            if (gta != null)
                att = gta.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            else
                att = type.GetTypeInfo().GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
            if (att != null)
                return valueSelector(att);

            return default(TValue);
        }
    }
}
