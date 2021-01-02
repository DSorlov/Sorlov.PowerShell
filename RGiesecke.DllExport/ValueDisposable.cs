// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.ValueDisposable`1
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport
{
  public class ValueDisposable<T> : GenericDisposable
  {
    public T Value { get; private set; }

    public ValueDisposable(T value, Action<T> action)
      : base((Action) (() => action(value)))
    {
      this.Value = value;
    }
  }
}
