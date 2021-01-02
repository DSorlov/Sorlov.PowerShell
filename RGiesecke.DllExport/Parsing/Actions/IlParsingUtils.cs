// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.Actions.IlParsingUtils
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport.Parsing.Actions
{
  internal static class IlParsingUtils
  {
    public static void ParseIlSnippet(
      string inputText,
      ParsingDirection direction,
      Func<IlParsingUtils.IlSnippetLocation, bool> predicate,
      Action<IlParsingUtils.IlSnippetFinalizaton> finalization = null)
    {
      bool withinString = false;
      bool withinScope = false;
      bool atOuterBracket = false;
      int nestedBrackets = 0;
      int endIndex = inputText.Length - 1;
      bool wasInterupted = false;
      int val2 = -1;
      int num1 = direction == ParsingDirection.Forward ? 1 : -1;
      int num2 = direction == ParsingDirection.Forward ? 0 : endIndex;
      Func<int, bool> func = direction == ParsingDirection.Forward ? (Func<int, bool>) (i => i <= endIndex) : (Func<int, bool>) (i => i > -1);
      int lastPosition = -1;
      char ch = char.MinValue;
      string lastIdentifier = (string) null;
      for (int index = num2; func(index); index += num1)
      {
        char currentChar = inputText[index];
        atOuterBracket = false;
        if (currentChar == '\'')
        {
          withinString = !withinString;
          if (!withinString && val2 > -1)
          {
            int val1 = index - num1;
            int startIndex = Math.Min(val1, val2);
            int num3 = Math.Max(val1, val2);
            lastIdentifier = val2 != val1 ? inputText.Substring(0, num3 + 1).Substring(startIndex) : "";
          }
          else
            lastIdentifier = (string) null;
          if (withinString && ch == '[')
            withinScope = true;
        }
        else
        {
          if (withinString && ch == '\'')
            val2 = index;
          if (!withinString)
          {
            val2 = -1;
            if (withinScope && currentChar == ']')
              withinScope = false;
            switch (currentChar)
            {
              case '(':
                --nestedBrackets;
                atOuterBracket = nestedBrackets == 0;
                break;
              case ')':
                atOuterBracket = nestedBrackets == 0;
                ++nestedBrackets;
                break;
            }
          }
        }
        if (!predicate(new IlParsingUtils.IlSnippetLocation(inputText, index, currentChar, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket)))
        {
          wasInterupted = true;
          break;
        }
        lastPosition = index;
        ch = currentChar;
      }
      if (finalization == null)
        return;
      finalization(new IlParsingUtils.IlSnippetFinalizaton(inputText, lastPosition, wasInterupted, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket));
    }

    public class IlSnippetLocationBase
    {
      public string InputText { get; private set; }

      public string LastIdentifier { get; set; }

      public bool WithinString { get; private set; }

      public bool WithinScope { get; set; }

      public int NestedBrackets { get; private set; }

      public bool AtOuterBracket { get; private set; }

      protected IlSnippetLocationBase(
        string inputText,
        string lastIdentifier,
        bool withinString,
        bool withinScope,
        int nestedBrackets,
        bool atOuterBracket)
      {
        if (inputText == null)
          throw new ArgumentNullException(nameof (inputText));
        this.InputText = inputText;
        this.LastIdentifier = lastIdentifier;
        this.WithinString = withinString;
        this.WithinScope = withinScope;
        this.NestedBrackets = nestedBrackets;
        this.AtOuterBracket = atOuterBracket;
      }
    }

    public sealed class IlSnippetFinalizaton : IlParsingUtils.IlSnippetLocationBase
    {
      public int LastPosition { get; private set; }

      public bool WasInterupted { get; private set; }

      public IlSnippetFinalizaton(
        string inputText,
        int lastPosition,
        bool wasInterupted,
        string lastIdentifier,
        bool withinString,
        bool withinScope,
        int nestedBrackets,
        bool atOuterBracket)
        : base(inputText, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket)
      {
        this.LastPosition = lastPosition;
        this.WasInterupted = wasInterupted;
      }
    }

    public class IlSnippetLocation : IlParsingUtils.IlSnippetLocationBase
    {
      public int Index { get; private set; }

      public char CurrentChar { get; private set; }

      public IlSnippetLocation(
        string inputText,
        int index,
        char currentChar,
        string lastIdentifier,
        bool withinString,
        bool withinScope,
        int nestedBrackets,
        bool atOuterBracket)
        : base(inputText, lastIdentifier, withinString, withinScope, nestedBrackets, atOuterBracket)
      {
        this.Index = index;
        this.CurrentChar = currentChar;
      }
    }
  }
}
