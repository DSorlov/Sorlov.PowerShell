// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.ExportedMethod
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

namespace RGiesecke.DllExport
{
  public sealed class ExportedMethod : ExportInfo
  {
    private readonly ExportedClass _ExportedClass;

    public ExportedMethod(ExportedClass exportedClass)
    {
      this._ExportedClass = exportedClass;
    }

    public string Name
    {
      get
      {
        return this.MemberName;
      }
    }

    public ExportedClass ExportedClass
    {
      get
      {
        return this._ExportedClass;
      }
    }

    public string MemberName { get; set; }

    public int VTableOffset { get; set; }

    public override string ExportName
    {
      get
      {
        return base.ExportName ?? this.Name;
      }
      set
      {
        base.ExportName = value;
      }
    }
  }
}
