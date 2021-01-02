// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Set
// Assembly: RGiesecke.DllExport.MSBuild, Version=1.2.7.38851, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: 94CA1E2E-92CF-42B1-82E4-1A993050CA42
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.MSBuild.dll

using Mono.Cecil;

namespace RGiesecke.DllExport
{
  internal static class Set
  {
    public static bool Contains(this ModuleAttributes input, ModuleAttributes set)
    {
      return (input & set) == input;
    }

    public static bool Contains(this TargetArchitecture input, TargetArchitecture set)
    {
      return (input & set) == input;
    }
  }
}
