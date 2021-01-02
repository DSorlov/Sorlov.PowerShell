// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.Actions.ExternalAssemlyDeclaration
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

namespace RGiesecke.DllExport.Parsing.Actions
{
  public sealed class ExternalAssemlyDeclaration
  {
    public int InputLineIndex { get; private set; }

    public string AssemblyName { get; private set; }

    public string AliasName { get; private set; }

    public ExternalAssemlyDeclaration(int inputLineIndex, string assemblyName, string aliasName)
    {
      this.InputLineIndex = inputLineIndex;
      this.AssemblyName = assemblyName;
      this.AliasName = aliasName;
    }
  }
}
