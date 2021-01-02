// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.DllExportServiceProviderExtensions
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;
using System.ComponentModel.Design;

namespace RGiesecke.DllExport
{
  internal static class DllExportServiceProviderExtensions
  {
    public static TService GetService<TService>(this IServiceProvider serviceProvider)
    {
      return (TService) serviceProvider.GetService(typeof (TService));
    }

    public static TServiceProvider AddService<TServiceProvider, TService>(
      this TServiceProvider serviceProvider,
      TService service)
      where TServiceProvider : IServiceContainer
    {
      serviceProvider.AddService(typeof (TService), (object) service);
      return serviceProvider;
    }

    public static TServiceProvider AddService<TServiceProvider, TService>(
      this TServiceProvider serviceProvider,
      TService service,
      bool promote)
      where TServiceProvider : IServiceContainer
    {
      serviceProvider.AddService(typeof (TService), (object) service, promote);
      return serviceProvider;
    }

    public static TServiceProvider AddServiceFactory<TServiceProvider, TService>(
      this TServiceProvider serviceProvider,
      Func<IServiceProvider, TService> serviceFactory)
      where TServiceProvider : IServiceContainer
    {
      serviceProvider.AddService(typeof (TService), (ServiceCreatorCallback) ((sp, t) => (object) serviceFactory((IServiceProvider) sp)));
      return serviceProvider;
    }

    public static TServiceProvider AddServiceFactory<TServiceProvider, TService>(
      this TServiceProvider serviceProvider,
      Func<IServiceProvider, TService> serviceFactory,
      bool promote)
      where TServiceProvider : IServiceContainer
    {
      serviceProvider.AddService(typeof (TService), (ServiceCreatorCallback) ((sp, t) => (object) serviceFactory((IServiceProvider) sp)), promote);
      return serviceProvider;
    }
  }
}
