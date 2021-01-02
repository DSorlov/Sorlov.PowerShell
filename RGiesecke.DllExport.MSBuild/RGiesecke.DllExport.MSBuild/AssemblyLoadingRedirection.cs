// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.MSBuild.AssemblyLoadingRedirection
// Assembly: RGiesecke.DllExport.MSBuild, Version=1.2.7.38851, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: 94CA1E2E-92CF-42B1-82E4-1A993050CA42
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.MSBuild.dll

using RGiesecke.DllExport.MSBuild.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RGiesecke.DllExport.MSBuild
{
  internal static class AssemblyLoadingRedirection
  {
    public static readonly bool IsSetup;

    public static void EnsureSetup()
    {
      if (!AssemblyLoadingRedirection.IsSetup)
        throw new InvalidOperationException(string.Format(Resources.AssemblyRedirection_for_0_has_not_been_setup_, (object) typeof (AssemblyLoadingRedirection).FullName));
    }

    static AssemblyLoadingRedirection()
    {
      AppDomain.CurrentDomain.AssemblyResolve += (ResolveEventHandler) ((sender, args) =>
      {
        AssemblyName assemblyName = new AssemblyName(args.Name);
        if (((IEnumerable<string>) new string[2]
        {
          "Mono.Cecil",
          "RGiesecke.DllExport"
        }).Contains<string>(assemblyName.Name))
        {
          string str = Path.Combine(Path.GetDirectoryName(new Uri(typeof (AssemblyLoadingRedirection).Assembly.EscapedCodeBase).AbsolutePath), assemblyName.Name + ".dll");
          if (File.Exists(str))
            return Assembly.LoadFrom(str);
        }
        return (Assembly) null;
      });
      AssemblyLoadingRedirection.IsSetup = true;
    }
  }
}
