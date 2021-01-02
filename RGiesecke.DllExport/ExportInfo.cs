// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.ExportInfo
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;
using System.Runtime.InteropServices;

namespace RGiesecke.DllExport
{
  [Serializable]
  public class ExportInfo : IExportInfo
  {
    public virtual string ExportName { get; set; }

    public CallingConvention CallingConvention { get; set; }

    public bool IsStatic { get; set; }

    public bool IsGeneric { get; set; }

    public void AssignFrom(IExportInfo info)
    {
      if (info == null)
        return;
      this.CallingConvention = info.CallingConvention != (CallingConvention) 0 ? info.CallingConvention : CallingConvention.StdCall;
      this.ExportName = info.ExportName;
    }
  }
}
