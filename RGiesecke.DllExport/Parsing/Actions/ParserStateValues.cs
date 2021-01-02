// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.Actions.ParserStateValues
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using RGiesecke.DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RGiesecke.DllExport.Parsing.Actions
{
  public sealed class ParserStateValues
  {
    public readonly Stack<string> ClassNames = new Stack<string>();
    public readonly ParserStateValues.MethodStateValues Method = new ParserStateValues.MethodStateValues();
    private readonly List<string> _Result = new List<string>();
    private readonly List<ExternalAssemlyDeclaration> _ExternalAssemlyDeclarations = new List<ExternalAssemlyDeclaration>();
    private readonly CpuPlatform _Cpu;
    private readonly ReadOnlyCollection<string> _InputLines;
    public bool AddLine;
    public string ClassDeclaration;
    public int MethodPos;
    public ParserState State;
    private readonly IList<ExternalAssemlyDeclaration> _ReadonlyExternalAssemlyDeclarations;

    public SourceCodeRange GetRange()
    {
      for (int inputPosition = this.InputPosition; inputPosition < this.InputLines.Count; ++inputPosition)
      {
        string inputLine = this.InputLines[inputPosition];
        string line;
        if (inputLine != null && (line = inputLine.Trim()).StartsWith(".line", StringComparison.Ordinal))
          return SourceCodeRange.FromMsIlLine(line);
      }
      return (SourceCodeRange) null;
    }

    public ParserStateValues(CpuPlatform cpu, IList<string> inputLines)
    {
      this._Cpu = cpu;
      this._InputLines = new ReadOnlyCollection<string>(inputLines);
      this._ReadonlyExternalAssemlyDeclarations = (IList<ExternalAssemlyDeclaration>) this._ExternalAssemlyDeclarations.AsReadOnly();
    }

    public IList<string> InputLines
    {
      get
      {
        return (IList<string>) this._InputLines;
      }
    }

    public int InputPosition { get; internal set; }

    public CpuPlatform Cpu
    {
      get
      {
        return this._Cpu;
      }
    }

    public List<string> Result
    {
      get
      {
        return this._Result;
      }
    }

    public IList<ExternalAssemlyDeclaration> ExternalAssemlyDeclarations
    {
      get
      {
        return this._ReadonlyExternalAssemlyDeclarations;
      }
    }

    public ExternalAssemlyDeclaration RegisterMsCorelibAlias(
      string assemblyName,
      string alias)
    {
      ExternalAssemlyDeclaration assemlyDeclaration = new ExternalAssemlyDeclaration(this.Result.Count, assemblyName, alias);
      this._ExternalAssemlyDeclarations.Add(assemlyDeclaration);
      return assemlyDeclaration;
    }

    public sealed class MethodStateValues
    {
      public MethodStateValues()
      {
        this.Reset();
      }

      public string Declaration { get; set; }

      public string ResultAttributes { get; set; }

      public string Name { get; set; }

      public string Attributes { get; set; }

      public string Result { get; set; }

      public string After { get; set; }

      public override string ToString()
      {
        return this.Name.IfEmpty(Resources.no_name___) + "; " + this.Declaration.IfEmpty(Resources.no_declaration___);
      }

      public void Reset()
      {
        this.Declaration = "";
        this.Name = "";
        this.Attributes = "";
        this.Result = "";
        this.ResultAttributes = "";
        this.After = "";
      }
    }
  }
}
