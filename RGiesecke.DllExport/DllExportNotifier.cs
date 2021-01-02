// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.DllExportNotifier
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using RGiesecke.DllExport.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace RGiesecke.DllExport
{
  public class DllExportNotifier : IDllExportNotifier, IDisposable
  {
    private readonly Stack<NotificationContext> _ContextScopes = new Stack<NotificationContext>();

    public void Dispose()
    {
      this.Notification = (EventHandler<DllExportNotificationEventArgs>) null;
    }

    public string ContextName
    {
      get
      {
        NotificationContext context = this.Context;
        if (!(context != (NotificationContext) null))
          return (string) null;
        return context.Name;
      }
    }

    public object ContextObject
    {
      get
      {
        NotificationContext context = this.Context;
        if (!(context != (NotificationContext) null))
          return (object) null;
        return context.Context;
      }
    }

    public NotificationContext Context
    {
      get
      {
        try
        {
          return this._ContextScopes.Peek();
        }
        catch (Exception ex)
        {
          throw ex;
        }
      }
    }

    public IDisposable CreateContextName(object context, string name)
    {
      return (IDisposable) new DllExportNotifier.ContextScope(this, new NotificationContext(name, context));
    }

    public event EventHandler<DllExportNotificationEventArgs> Notification;

    public void Notify(DllExportNotificationEventArgs e)
    {
      NotificationContext notificationContext = this.Context;
      if ((object) notificationContext == null)
        notificationContext = new NotificationContext((string) null, (object) this);
      this.OnNotification((object) notificationContext, e);
    }

    private void OnNotification(object sender, DllExportNotificationEventArgs e)
    {
      EventHandler<DllExportNotificationEventArgs> notification = this.Notification;
      if (notification == null)
        return;
      notification(sender, e);
    }

    public void Notify(int severity, string code, string message, params object[] values)
    {
      this.Notify(severity, code, (string) null, new SourceCodePosition?(), new SourceCodePosition?(), message, values);
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
      DllExportNotificationEventArgs e = new DllExportNotificationEventArgs()
      {
        Severity = severity,
        Code = code,
        Context = this.Context,
        FileName = fileName,
        StartPosition = startPosition,
        EndPosition = endPosition
      };
      e.Message = values.NullSafeCall<object[], int>((Func<int>) (() => values.Length)) == 0 ? message : string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, values);
      if (string.IsNullOrEmpty(e.Message))
        return;
      this.Notify(e);
    }

    private sealed class ContextScope : IDisposable
    {
      private readonly DllExportNotifier _Notifier;

      public NotificationContext Context { get; private set; }

      public ContextScope(DllExportNotifier notifier, NotificationContext context)
      {
        this.Context = context;
        this._Notifier = notifier;
        Stack<NotificationContext> contextScopes = this._Notifier._ContextScopes;
        lock (contextScopes)
          contextScopes.Push(context);
      }

      public void Dispose()
      {
        Stack<NotificationContext> contextScopes = this._Notifier._ContextScopes;
        lock (contextScopes)
        {
          if (contextScopes.Peek() != this.Context)
            throw new InvalidOperationException(string.Format(Resources.Current_Notifier_Context_is___0____it_should_have_been___1___, (object) contextScopes.Peek(), (object) this.Context.Name));
          contextScopes.Pop();
        }
      }
    }
  }
}
