// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.Actions.ClassDeclarationParserAction
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;
using System.Text;

namespace RGiesecke.DllExport.Parsing.Actions
{
  [ParserStateAction(ParserState.ClassDeclaration)]
  public sealed class ClassDeclarationParserAction : IlParser.ParserStateAction
  {
    public override void Execute(ParserStateValues state, string trimmedLine)
    {
      if (trimmedLine.StartsWith("{"))
      {
        state.State = ParserState.Class;
        string str = ClassDeclarationParserAction.GetClassName(state);
        if (state.ClassNames.Count > 0)
          str = state.ClassNames.Peek() + "/" + str;
        state.ClassNames.Push(str);
      }
      else
      {
        state.ClassDeclaration = state.ClassDeclaration + " " + trimmedLine;
        state.AddLine = true;
      }
    }

    private static string GetClassName(ParserStateValues state)
    {
      bool hadClassName = false;
      StringBuilder classNameBuilder = new StringBuilder(state.ClassDeclaration.Length);
      IlParsingUtils.ParseIlSnippet(state.ClassDeclaration, ParsingDirection.Forward, (Func<IlParsingUtils.IlSnippetLocation, bool>) (s =>
      {
        if (s.WithinString)
        {
          hadClassName = true;
          if (s.CurrentChar != '\'')
            classNameBuilder.Append(s.CurrentChar);
        }
        else if (hadClassName)
        {
          if (s.CurrentChar == '.' || s.CurrentChar == '/')
            classNameBuilder.Append(s.CurrentChar);
          else if (s.CurrentChar != '\'')
            return false;
        }
        return true;
      }), (Action<IlParsingUtils.IlSnippetFinalizaton>) null);
      return classNameBuilder.ToString();
    }
  }
}
