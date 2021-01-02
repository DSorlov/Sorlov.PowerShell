// Decompiled with JetBrains decompiler
// Type: RGiesecke.DllExport.MSBuild.DllExportTask
// Assembly: RGiesecke.DllExport.MSBuild, Version=1.2.7.38851, Culture=neutral, PublicKeyToken=8f52d83c1a22df51
// MVID: 94CA1E2E-92CF-42B1-82E4-1A993050CA42
// Assembly location: C:\Users\k.gosse\.nuget\packages\unmanagedexports.repack\1.0.0\tasks\RGiesecke.DllExport.MSBuild.dll

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Security.Permissions;

namespace RGiesecke.DllExport.MSBuild
{
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class DllExportTask : Task, IDllExportTask, IInputValues, IServiceProvider
    {
        private readonly ExportTaskImplementation<DllExportTask> _ExportTaskImplementation;

        private IServiceProvider _ServiceProvider
        {
            get
            {
                return (IServiceProvider)this._ExportTaskImplementation;
            }
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return this._ServiceProvider.GetService(serviceType);
        }

        public string MethodAttributes
        {
            get
            {
                return this._ExportTaskImplementation.MethodAttributes;
            }
            set
            {
                this._ExportTaskImplementation.MethodAttributes = value;
            }
        }

        public IDllExportNotifier GetNotifier()
        {
            return this._ExportTaskImplementation.GetNotifier();
        }

        public void Notify(int severity, string code, string message, params object[] values)
        {
            this._ExportTaskImplementation.Notify(severity, code, message, values);
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
            this._ExportTaskImplementation.Notify(severity, code, fileName, startPosition, endPosition, message, values);
        }

        bool? IDllExportTask.SkipOnAnyCpu
        {
            get
            {
                return this._ExportTaskImplementation.SkipOnAnyCpu;
            }
            set
            {
                this._ExportTaskImplementation.SkipOnAnyCpu = value;
            }
        }

        public string SkipOnAnyCpu
        {
            get
            {
                return Convert.ToString((object)this._ExportTaskImplementation.SkipOnAnyCpu);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    this._ExportTaskImplementation.SkipOnAnyCpu = new bool?();
                else
                    this._ExportTaskImplementation.SkipOnAnyCpu = new bool?(Convert.ToBoolean(value));
            }
        }

        public string TargetFrameworkVersion
        {
            get
            {
                return this._ExportTaskImplementation.TargetFrameworkVersion;
            }
            set
            {
                this._ExportTaskImplementation.TargetFrameworkVersion = value;
            }
        }

        public string Platform
        {
            get
            {
                return this._ExportTaskImplementation.Platform;
            }
            set
            {
                this._ExportTaskImplementation.Platform = value;
            }
        }

        public string PlatformTarget
        {
            get
            {
                return this._ExportTaskImplementation.PlatformTarget;
            }
            set
            {
                this._ExportTaskImplementation.PlatformTarget = value;
            }
        }

        public string CpuType
        {
            get
            {
                return this._ExportTaskImplementation.CpuType;
            }
            set
            {
                this._ExportTaskImplementation.CpuType = value;
            }
        }

        public string ProjectDirectory
        {
            get
            {
                return this._ExportTaskImplementation.ProjectDirectory;
            }
            set
            {
                this._ExportTaskImplementation.ProjectDirectory = value;
            }
        }

        public string AssemblyKeyContainerName
        {
            get
            {
                return this._ExportTaskImplementation.AssemblyKeyContainerName;
            }
            set
            {
                this._ExportTaskImplementation.AssemblyKeyContainerName = value;
            }
        }

        public int Timeout
        {
            get
            {
                return this._ExportTaskImplementation.Timeout;
            }
            set
            {
                this._ExportTaskImplementation.Timeout = value;
            }
        }

        static DllExportTask()
        {
            AssemblyLoadingRedirection.EnsureSetup();
        }

        public DllExportTask()
        {
            this._ExportTaskImplementation = new ExportTaskImplementation<DllExportTask>(this);
        }

        public DllExportTask(ResourceManager taskResources)
          : base(taskResources)
        {
            this._ExportTaskImplementation = new ExportTaskImplementation<DllExportTask>(this);
        }

        public DllExportTask(ResourceManager taskResources, string helpKeywordPrefix)
          : base(taskResources, helpKeywordPrefix)
        {
            this._ExportTaskImplementation = new ExportTaskImplementation<DllExportTask>(this);
        }

        public CpuPlatform Cpu
        {
            set
            {
                this._ExportTaskImplementation.Cpu = value;
            }
            get
            {
                return this._ExportTaskImplementation.Cpu;
            }
        }

        public bool EmitDebugSymbols
        {
            set
            {
                this._ExportTaskImplementation.EmitDebugSymbols = value;
            }
            get
            {
                return this._ExportTaskImplementation.EmitDebugSymbols;
            }
        }

        public string LeaveIntermediateFiles
        {
            get
            {
                return this._ExportTaskImplementation.LeaveIntermediateFiles;
            }
            set
            {
                this._ExportTaskImplementation.LeaveIntermediateFiles = value;
            }
        }

        public string FileName
        {
            set
            {
                this._ExportTaskImplementation.FileName = value;
            }
            get
            {
                return this._ExportTaskImplementation.FileName;
            }
        }

        [Required]
        public string FrameworkPath
        {
            set
            {
                this._ExportTaskImplementation.FrameworkPath = value;
            }
            get
            {
                return this._ExportTaskImplementation.FrameworkPath;
            }
        }

        public string LibToolPath
        {
            set
            {
                this._ExportTaskImplementation.LibToolPath = value;
            }
            get
            {
                return this._ExportTaskImplementation.LibToolPath;
            }
        }

        public string LibToolDllPath
        {
            set
            {
                this._ExportTaskImplementation.LibToolDllPath = value;
            }
            get
            {
                return this._ExportTaskImplementation.LibToolDllPath;
            }
        }

        [Required]
        public string InputFileName
        {
            set
            {
                this._ExportTaskImplementation.InputFileName = value;
            }
            get
            {
                return this._ExportTaskImplementation.InputFileName;
            }
        }

        public string KeyContainer
        {
            set
            {
                this._ExportTaskImplementation.KeyContainer = value;
            }
            get
            {
                return this._ExportTaskImplementation.KeyContainer;
            }
        }

        public string KeyFile
        {
            set
            {
                this._ExportTaskImplementation.KeyFile = value;
            }
            get
            {
                return this._ExportTaskImplementation.KeyFile;
            }
        }

        public string OutputFileName
        {
            set
            {
                this._ExportTaskImplementation.OutputFileName = value;
            }
            get
            {
                return this._ExportTaskImplementation.OutputFileName;
            }
        }

        public string RootDirectory
        {
            set
            {
                this._ExportTaskImplementation.RootDirectory = value;
            }
            get
            {
                return this._ExportTaskImplementation.RootDirectory;
            }
        }

        [Required]
        public string SdkPath
        {
            set
            {
                this._ExportTaskImplementation.SdkPath = value;
            }
            get
            {
                return this._ExportTaskImplementation.SdkPath;
            }
        }

        public string DllExportAttributeFullName
        {
            set
            {
                this._ExportTaskImplementation.DllExportAttributeFullName = value;
            }
            get
            {
                return this._ExportTaskImplementation.DllExportAttributeFullName;
            }
        }

        public string DllExportAttributeAssemblyName
        {
            set
            {
                this._ExportTaskImplementation.DllExportAttributeAssemblyName = value;
            }
            get
            {
                return this._ExportTaskImplementation.DllExportAttributeAssemblyName;
            }
        }

        [CLSCompliant(false)]
        public AssemblyBinaryProperties InferAssemblyBinaryProperties()
        {
            return this._ExportTaskImplementation.InferAssemblyBinaryProperties();
        }

        public void InferOutputFile()
        {
            this._ExportTaskImplementation.InferOutputFile();
        }

        public override bool Execute()
        {
            return this._ExportTaskImplementation.Execute();
        }
    }
}
