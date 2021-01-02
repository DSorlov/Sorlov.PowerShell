// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.DuplicateExports
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System.Collections.Generic;

namespace RGiesecke.DllExport
{
  public sealed class DuplicateExports
  {
    private readonly List<ExportedMethod> _Duplicates = new List<ExportedMethod>();
    private readonly ExportedMethod _UsedExport;

    internal DuplicateExports(ExportedMethod usedExport)
    {
      this._UsedExport = usedExport;
    }

    public ExportedMethod UsedExport
    {
      get
      {
        return this._UsedExport;
      }
    }

    public ICollection<ExportedMethod> Duplicates
    {
      get
      {
        return (ICollection<ExportedMethod>) this._Duplicates;
      }
    }
  }
}
