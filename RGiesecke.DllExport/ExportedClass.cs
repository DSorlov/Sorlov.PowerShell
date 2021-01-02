// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.ExportedClass
// Assembly: RGiesecke.DllExport, Version=1.2.7.38850, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: C9BFF197-CAB3-40A6-BA31-9260266DE1B7
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.dll

using System.Collections.Generic;

namespace RGiesecke.DllExport
{
  public class ExportedClass
  {
    private readonly List<ExportedMethod> _Methods = new List<ExportedMethod>();
    private readonly Dictionary<string, List<ExportedMethod>> _MethodsByName = new Dictionary<string, List<ExportedMethod>>();

    public ExportedClass(string fullTypeName, bool hasGenericContext)
    {
      this.FullTypeName = fullTypeName;
      this.HasGenericContext = hasGenericContext;
    }

    public string FullTypeName { get; private set; }

    public bool HasGenericContext { get; private set; }

    internal Dictionary<string, List<ExportedMethod>> MethodsByName
    {
      get
      {
        return this._MethodsByName;
      }
    }

    internal List<ExportedMethod> Methods
    {
      get
      {
        return this._Methods;
      }
    }

    internal void Refresh()
    {
      lock (this)
      {
        this.MethodsByName.Clear();
        foreach (ExportedMethod method in this.Methods)
        {
          List<ExportedMethod> exportedMethodList;
          if (!this.MethodsByName.TryGetValue(method.Name, out exportedMethodList))
            this.MethodsByName.Add(method.Name, exportedMethodList = new List<ExportedMethod>());
          exportedMethodList.Add(method);
        }
      }
    }
  }
}
