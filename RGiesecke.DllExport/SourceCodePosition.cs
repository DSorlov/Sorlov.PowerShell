// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.SourceCodePosition
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System;

namespace RGiesecke.DllExport
{
  public struct SourceCodePosition : IEquatable<SourceCodePosition>
  {
    private readonly int _Character;
    private readonly int _Line;

    public static SourceCodePosition? FromText(string lineText, string columnText)
    {
      int? nullable1 = new int?();
      int? nullable2 = new int?();
      int result;
      if (int.TryParse(lineText, out result))
        nullable1 = new int?(result);
      if (int.TryParse(columnText, out result))
        nullable2 = new int?(result);
      if (nullable1.HasValue || nullable2.HasValue)
        return new SourceCodePosition?(new SourceCodePosition(nullable1 ?? -1, nullable2 ?? -1));
      return new SourceCodePosition?();
    }

    public SourceCodePosition(int line, int character)
    {
      this._Line = line;
      this._Character = character;
    }

    public int Line
    {
      get
      {
        return this._Line;
      }
    }

    public int Character
    {
      get
      {
        return this._Character;
      }
    }

    public bool Equals(SourceCodePosition other)
    {
      if (other._Line == this._Line)
        return other._Character == this._Character;
      return false;
    }

    public override bool Equals(object obj)
    {
      if (!object.ReferenceEquals((object) null, obj) && obj.GetType() == typeof (SourceCodePosition))
        return this.Equals((SourceCodePosition) obj);
      return false;
    }

    public override int GetHashCode()
    {
      return this._Line * 397 ^ this._Character;
    }

    public static bool operator ==(SourceCodePosition left, SourceCodePosition right)
    {
      return left.Equals(right);
    }

    public static bool operator !=(SourceCodePosition left, SourceCodePosition right)
    {
      return !left.Equals(right);
    }
  }
}
