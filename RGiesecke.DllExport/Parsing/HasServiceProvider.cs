// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.HasServiceProvider
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport.Parsing
{
  public abstract class HasServiceProvider : IDisposable, IServiceProvider
  {
    private readonly IServiceProvider _ServiceProvider;

    protected HasServiceProvider(IServiceProvider serviceProvider)
    {
      this._ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider
    {
      get
      {
        return this._ServiceProvider;
      }
    }

    public void Dispose()
    {
      (this._ServiceProvider as IDisposable)?.Dispose();
    }

    object IServiceProvider.GetService(Type serviceType)
    {
      return this._ServiceProvider.GetService(serviceType);
    }
  }
}
