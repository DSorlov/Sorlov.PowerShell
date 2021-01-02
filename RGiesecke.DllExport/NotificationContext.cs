// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.NotificationContext
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport
{
  public sealed class NotificationContext : IEquatable<NotificationContext>
  {
    public string Name { get; private set; }

    public object Context { get; private set; }

    public NotificationContext(string name, object context)
    {
      this.Name = name;
      this.Context = context;
    }

    public bool Equals(NotificationContext other)
    {
      if (object.ReferenceEquals((object) null, (object) other))
        return false;
      if (object.ReferenceEquals((object) this, (object) other))
        return true;
      if (object.Equals(this.Context, other.Context))
        return string.Equals(this.Name, other.Name);
      return false;
    }

    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if ((object) (obj as NotificationContext) != null)
        return this.Equals((NotificationContext) obj);
      return false;
    }

    public override int GetHashCode()
    {
      return (this.Context != null ? this.Context.GetHashCode() : 0) * 397 ^ (this.Name != null ? this.Name.GetHashCode() : 0);
    }

    public static bool operator ==(NotificationContext left, NotificationContext right)
    {
      return object.Equals((object) left, (object) right);
    }

    public static bool operator !=(NotificationContext left, NotificationContext right)
    {
      return !object.Equals((object) left, (object) right);
    }
  }
}
