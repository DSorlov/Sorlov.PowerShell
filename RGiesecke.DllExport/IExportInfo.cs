// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.IExportInfo
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System.Runtime.InteropServices;

namespace RGiesecke.DllExport
{
  public interface IExportInfo
  {
    CallingConvention CallingConvention { get; set; }

    string ExportName { get; set; }

    bool IsStatic { get; }

    bool IsGeneric { get; }

    void AssignFrom(IExportInfo info);
  }
}
