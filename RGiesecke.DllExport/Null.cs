// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Null
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;
using System.Collections.Generic;

namespace RGiesecke.DllExport
{
  internal static class Null
  {
    public static int NullSafeCount<T>(this ICollection<T> items)
    {
      if (items != null)
        return items.Count;
      return 0;
    }

    public static string NullSafeTrim(this string input, params char[] trimChars)
    {
      return input?.Trim(trimChars);
    }

    public static string NullSafeToString(this object input)
    {
      return input?.ToString();
    }

    public static string NullSafeToLowerInvariant(this string input)
    {
      return input?.ToLowerInvariant();
    }

    public static string NullSafeToUpperInvariant(this string input)
    {
      return input?.ToUpperInvariant();
    }

    public static string NullSafeTrimStart(this string input, params char[] trimChars)
    {
      return input?.TrimStart(trimChars);
    }

    public static string NullIfEmpty(this string input)
    {
      if (!string.IsNullOrEmpty(input))
        return input;
      return (string) null;
    }

    public static string NullSafeTrimEnd(this string input, params char[] trimChars)
    {
      return input?.TrimEnd(trimChars);
    }

    public static T IfNull<T>(this T input, T replacement)
    {
      if ((object) input != null)
        return input;
      return replacement;
    }

    public static string IfEmpty(this string input, string replacement)
    {
      if (!string.IsNullOrEmpty(input))
        return input;
      return replacement;
    }

    public static TValue NullSafeCall<T, TValue>(this T input, Func<T, TValue> method)
    {
      if ((object) input != null)
        return method(input);
      return default (TValue);
    }

    public static TValue NullSafeCall<T, TValue>(this T input, Func<TValue> method)
    {
      if ((object) input != null)
        return method();
      return default (TValue);
    }
  }
}
