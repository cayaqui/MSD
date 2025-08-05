using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Helpers;

public static class EnumHelper
{
    public static T GetEnumFromString<T>(string value)
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out T result))
        {
            return result;
        }
        throw new ArgumentException($"'{value}' no es un valor válido para {typeof(T).Name}");
    }

    public static IEnumerable<T> GetValues<T>()
        where T : Enum
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    public static IEnumerable<string> GetNames<T>()
        where T : Enum
    {
        return Enum.GetNames(typeof(T));
    }

    public static bool IsValidValue<T>(string value)
        where T : struct, Enum
    {
        return Enum.TryParse<T>(value, true, out _);
    }

    public static int GetValueFromName<T>(string value)
        where T : struct, Enum
    {
        if (Enum.TryParse<T>(value, true, out T result))
        {
            return Convert.ToInt32(result);
        }
        throw new ArgumentException($"'{value}' no es un valor válido para {typeof(T).Name}");
    }

    public static string GetNameFromValue<T>(int value)
        where T : Enum
    {
        if (Enum.IsDefined(typeof(T), value))
        {
            return Enum.GetName(typeof(T), value);
        }
        throw new ArgumentException($"'{value}' no es un valor válido para {typeof(T).Name}");
    }

    public static Dictionary<string, int> GetNameValuePairs<T>()
        where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .ToDictionary(e => e.ToString(), e => Convert.ToInt32(e));
    }
}
