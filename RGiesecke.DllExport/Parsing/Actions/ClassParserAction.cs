// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.Actions.ClassParserAction
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport.Parsing.Actions
{
  [ParserStateAction(ParserState.Class)]
  public sealed class ClassParserAction : IlParser.ParserStateAction
  {
    public override void Execute(ParserStateValues state, string trimmedLine)
    {
      if (trimmedLine.StartsWith(".class", StringComparison.Ordinal))
      {
        state.State = ParserState.ClassDeclaration;
        state.AddLine = true;
        state.ClassDeclaration = trimmedLine;
      }
      else if (trimmedLine.StartsWith(".method", StringComparison.Ordinal))
      {
        ExportedClass exportedClass;
        if (state.ClassNames.Count == 0 || !this.Parser.Exports.ClassesByName.TryGetValue(state.ClassNames.Peek(), out exportedClass))
          return;
        state.Method.Reset();
        state.Method.Declaration = trimmedLine;
        state.AddLine = false;
        state.State = ParserState.MethodDeclaration;
      }
      else
      {
        if (!trimmedLine.StartsWith("} // end of class", StringComparison.Ordinal))
          return;
        state.ClassNames.Pop();
        state.State = state.ClassNames.Count > 0 ? ParserState.Class : ParserState.Normal;
      }
    }
  }
}
