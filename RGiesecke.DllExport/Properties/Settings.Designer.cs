// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Properties.Settings
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace RGiesecke.DllExport.Properties
{
  [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
  [CompilerGenerated]
  public sealed class Settings : ApplicationSettingsBase
  {
    private static Settings defaultInstance = (Settings) SettingsBase.Synchronized((SettingsBase) new Settings());

    public static Settings Default
    {
      get
      {
        return Settings.defaultInstance;
      }
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    [ApplicationScopedSetting]
    public string ILDasmPath
    {
      get
      {
        return (string) this[nameof (ILDasmPath)];
      }
    }

    [DebuggerNonUserCode]
    [DefaultSettingValue("")]
    [ApplicationScopedSetting]
    public string ILAsmPath
    {
      get
      {
        return (string) this[nameof (ILAsmPath)];
      }
    }
  }
}
