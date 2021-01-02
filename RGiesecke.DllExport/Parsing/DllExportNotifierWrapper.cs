// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.Parsing.DllExportNotifierWrapper
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport.Parsing
{
  public abstract class DllExportNotifierWrapper : IDllExportNotifier, IDisposable
  {
    protected DllExportNotifierWrapper(IDllExportNotifier notifier)
    {
      this.Notifier = notifier;
    }

    public IDisposable CreateContextName(object context, string name)
    {
      return this.Notifier.CreateContextName(context, name);
    }

    protected virtual IDllExportNotifier Notifier { get; private set; }

    protected virtual bool OwnsNotifier
    {
      get
      {
        return false;
      }
    }

    public void Notify(DllExportNotificationEventArgs e)
    {
      this.Notifier.Notify(e);
    }

    public void Notify(int severity, string code, string message, params object[] values)
    {
      this.Notifier.Notify(severity, code, message, values);
    }

    public void Notify(
      int severity,
      string code,
      string fileName,
      SourceCodePosition? startPosition,
      SourceCodePosition? endPosition,
      string message,
      params object[] values)
    {
      this.Notifier.Notify(severity, code, fileName, startPosition, endPosition, message, values);
    }

    public void Dispose()
    {
      if (!this.OwnsNotifier)
        return;
      (this.Notifier as IDisposable)?.Dispose();
    }

    event EventHandler<DllExportNotificationEventArgs> IDllExportNotifier.Notification
    {
      add
      {
        this.Notifier.Notification += value;
      }
      remove
      {
        this.Notifier.Notification -= value;
      }
    }
  }
}
