// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.IInputValues
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

namespace RGiesecke.DllExport
{
  public interface IInputValues
  {
    CpuPlatform Cpu { get; set; }

    bool EmitDebugSymbols { get; set; }

    string LeaveIntermediateFiles { get; set; }

    string FileName { get; set; }

    string FrameworkPath { get; set; }

    string InputFileName { get; set; }

    string KeyContainer { get; set; }

    string KeyFile { get; set; }

    string OutputFileName { get; set; }

    string RootDirectory { get; set; }

    string SdkPath { get; set; }

    string MethodAttributes { get; set; }

    string LibToolPath { get; set; }

    string DllExportAttributeFullName { get; set; }

    string DllExportAttributeAssemblyName { get; set; }

    string LibToolDllPath { get; set; }

    AssemblyBinaryProperties InferAssemblyBinaryProperties();

    void InferOutputFile();
  }
}
