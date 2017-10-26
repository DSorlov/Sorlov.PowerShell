using System;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.IO;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    [CmdletProvider("ZipFile",ProviderCapabilities.None)]
    public class ZipFileProvider : NavigationCmdletProvider, IContentCmdletProvider
    {
        public const string PATH_SEPERATOR = "/";

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            WriteWarning("");
            WriteWarning("  This provider is still experimental and have not yet been fully tested!");
            WriteWarning("");
            WriteWarning("  Not implemented (will be): COPY, MOVE, RENAME, GETPARENTPATH, EXPANDPATH, CONVERTPATH");
            WriteWarning("  Not implemented (will not be): MAKEPATH");
            WriteWarning("  Limitation: The zip-file you are mounting must be in your current directory");
            WriteWarning("");

            try
            {
                ZipFileDriveInfo zipDriveInfo = new ZipFileDriveInfo(drive);
                zipDriveInfo.Archive = ZipFileArchive.OpenFromFile(drive.Root,System.IO.FileMode.OpenOrCreate,System.IO.FileAccess.ReadWrite,System.IO.FileShare.None,false);
                return zipDriveInfo;
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex.InnerException,"100",ErrorCategory.DeviceError,null));
                return null;
            }
        }


        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            ZipFileDriveInfo driveInfo = drive as ZipFileDriveInfo;

            driveInfo.Archive.Dispose();


            return ZipDrive;
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            base.CopyItem(path, copyPath, recurse);

            try
            {
                if (GetPathCaseInsensitivePath(ParsePath(copyPath,false))!=string.Empty)
                {
                 WriteError(new ErrorRecord(new Exception("path"),"NoPath",ErrorCategory.InvalidArgument, path));
                }
            }
            catch
            {
            }
        }

        protected override void RenameItem(string path, string newName)
        {
            CopyItem(path, newName, false);
            RemoveItem(path, false);
        }

        internal ZipFileArchive.ZipFileInfo GetPathCaseInsensitive(string path)
        {
            string lowerPath = path.ToLower();
            foreach (string casedFiles in ZipDrive.Archive.FileNames)
            {
                string lowerFile = casedFiles.ToLower();

                if (lowerFile==lowerPath)
                    return ZipDrive.Archive.GetFile(casedFiles);
            }

            throw new System.IO.FileNotFoundException("File could not be found");
        }

        internal string GetPathCaseInsensitivePath(string path)
        {
            string lowerPath = path.ToLower();
            foreach (string casedFiles in ZipDrive.Archive.FileNames)
            {
                string lowerFile = casedFiles.ToLower();

                if (lowerFile == lowerPath)
                    return casedFiles;
            }

            throw new System.IO.FileNotFoundException("File could not be found");
        }


        protected override bool IsValidPath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return false;

            foreach (string pathChunk in ParsePath(path,false).Split(PATH_SEPERATOR.ToCharArray()))
                if (pathChunk.Length == 0)
                    return false;

            return true;
        }


        protected override bool ItemExists(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                WriteError(new ErrorRecord(new ArgumentException("path"),"NoPath",ErrorCategory.InvalidArgument, path));
                return false;
            }

            if (PathIsDrive(path))
                return true;

            try
            {
                string fileToFind = ParsePath(path,true);
                ZipFileArchive.ZipFileInfo info = GetPathCaseInsensitive(fileToFind);
                return true;
            }
            catch
            {
            }
            try
            {
                string fileToFind = ParsePath(path, false);
                ZipFileArchive.ZipFileInfo info = GetPathCaseInsensitive(fileToFind);
                return true;
            }
            catch
            {
                return false;
            }

        }

        protected override void GetItem(string path)
        {
            if (PathIsDrive(path))
                WriteItemObject(PSDriveInfo, path, true);

            try
            {
                string parsedPath = ParsePath(path, false);
                ZipFileArchive.ZipFileInfo fileInfo = GetPathCaseInsensitive(parsedPath);
                ZipFileItem zi = new ZipFileItem(GetFileOrDirname(fileInfo.Name), fileInfo.LastModFileDateTime, fileInfo.FolderFlag ? "Directory" : "File");
                WriteItemObject(zi, fixRootPath(fileInfo.Name).Replace("/", "\\"), fileInfo.FolderFlag);
            }
            catch
            {
            }
            try
            {
                string parsedPath = ParsePath(path, true);
                ZipFileArchive.ZipFileInfo fileInfo = GetPathCaseInsensitive(parsedPath);
                ZipFileItem zi = new ZipFileItem(GetFileOrDirname(fileInfo.Name), fileInfo.LastModFileDateTime, fileInfo.FolderFlag ? "Directory" : "File");
                WriteItemObject(zi, fixRootPath(fileInfo.Name).Replace("/", "\\"), fileInfo.FolderFlag);
            }
            catch
            {
            }

        }


        private string GetFileOrDirname(string input)
        {
            string result = System.IO.Path.GetFileName(input);
            if (result == string.Empty)
            {
                string dirname = System.IO.Path.GetDirectoryName(input);
                if (dirname.LastIndexOf("\\") > 0)
                    result = dirname.Substring(dirname.LastIndexOf("\\")+1);
                else
                    result = dirname;
            }
            return result;
        }

        private string fixRootPath(string input)
        {
            string path = input.StartsWith("/") ? input : "/" + input;
            path = ZipDrive.Root + "//" + input;
            return path;
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            path = ParsePath(path,true);

            foreach (ZipFileArchive.ZipFileInfo zipEntry in ZipDrive.Archive.Files)
            {
                if (!recurse && !ItemIsInDirectory(path, zipEntry.Name))
                    continue;

                if (path == "/" || zipEntry.Name.StartsWith(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    string type = "File";
                    if (zipEntry.FolderFlag)
                    {
                        type = "Directory";
                    }

                    ZipFileItem zi = new ZipFileItem(GetFileOrDirname(zipEntry.Name), zipEntry.LastModFileDateTime, type);
                    WriteItemObject(zi, fixRootPath(zipEntry.Name).Replace("/","\\"), zipEntry.FolderFlag);
                }
            }
        }

        internal string ParsePath(string path, bool asDirectory)
        {
            path = path.Replace(PSDriveInfo.Root + "\\", "");
            //path = path.Substring(path.IndexOf("\\") + 1);

            if (path.IndexOf(".zip", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                path = path.Remove(path.ToUpper().IndexOf(".ZIP"), 4);
            }

            path = path.Replace("\\", PATH_SEPERATOR);


            if (asDirectory & !path.EndsWith(PATH_SEPERATOR))
            {
                path = path + PATH_SEPERATOR;
            }

            if (path == string.Empty)
                path = PATH_SEPERATOR;

            return path;
        }

        private static bool ItemIsInDirectory(string path, string item)
        {
            if (path.Length > item.Length)
                return false;

            string remainder = item.Remove(0, path.Length);

            if (remainder.Length == 0)
            {
                return false;
            }

            if (remainder.EndsWith(PATH_SEPERATOR, StringComparison.InvariantCultureIgnoreCase))
            {
                remainder = remainder.Substring(0, remainder.Length - 1);
            }

            if (remainder.IndexOf(PATH_SEPERATOR) == -1)
            {
                return true;
            }

            return false;
        }

        private bool PathIsDrive(string path)
        {
            if (String.IsNullOrEmpty(path.Replace(PSDriveInfo.Root, "")) ||
                  String.IsNullOrEmpty(path.Replace(PSDriveInfo.Root + "\\", "")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            path = ParsePath(path,false);

            foreach (ZipFileArchive.ZipFileInfo zipEntry in ZipDrive.Archive.Files)
            {
                if (!ItemIsInDirectory(path, zipEntry.Name))
                    continue;

                if (zipEntry.Name.StartsWith(path,StringComparison.InvariantCultureIgnoreCase))
                {
                    WriteItemObject(zipEntry.Name, path, zipEntry.FolderFlag);
                }
            }
        }

        protected override bool HasChildItems(string path)
        {
            if (PathIsDrive(path))
                return true;

            return GetPathCaseInsensitive(ParsePath(path, false)).FolderFlag;

        }

        protected override void NewItem(string path, string type, object newItemValue)
        {
            ZipFileArchive.ZipFileInfo fileInfo = ZipDrive.Archive.AddFile(ParsePath(path, false), ZipFileArchive.CompressionMethodEnum.Deflated, ZipFileArchive.DeflateOptionEnum.Normal);
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            ZipDrive.Archive.DeleteFile(GetPathCaseInsensitivePath(ParsePath(path,false)));
        }

        protected override bool IsItemContainer(string path)
        {
            if (PathIsDrive(path))
                return true;

            path = ParsePath(path,true);

            if (GetPathCaseInsensitive(path).FolderFlag)
                return true;
            else
                return false;
        }

        internal ZipFileDriveInfo ZipDrive
        {
            get
            {
                return PSDriveInfo as ZipFileDriveInfo;
            }
        }


        public void ClearContent(string path)
        {
            Stream stream = ZipDrive.Archive.GetFile(GetPathCaseInsensitivePath(ParsePath(path, false))).GetStream(FileMode.Open, FileAccess.ReadWrite);
            stream.SetLength(0);
            stream.Flush();
            stream.Close();
        }

        public object ClearContentDynamicParameters(string path)
        {
            return null;
        }

        protected override void SetItem(string path, object value)
        {
            ZipFileContentDynamicParameters parameters = DynamicParameters as ZipFileContentDynamicParameters;

            Stream stream = ZipDrive.Archive.GetFile(GetPathCaseInsensitivePath(ParsePath(path, false))).GetStream(FileMode.Open, FileAccess.ReadWrite);

            if (parameters.UsingByteEncoding)
            {
                if (value.GetType() == typeof(byte[]))
                {
                    foreach (byte bajt in (byte[])value)
                        stream.WriteByte(bajt);
                }
                else
                {
                    WriteError(new ErrorRecord(new ArgumentException("content"), "ByteDataWasExpected", ErrorCategory.InvalidArgument, path));
                }
            }
            else
            {
                StreamWriter writer = new StreamWriter(stream, parameters.EncodingType);
                writer.Write(value);
                writer.Flush();
                writer.Close();
            }

            stream.Flush();
            stream.Close();
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {
            return new ZipFileContentDynamicParameters(ZipFileContentDynamicParameters.FileSystemCmdletProviderEncoding.UTF8, FileMode.Open, FileAccess.Read);
        }

        public IContentReader GetContentReader(string path)
        {
            ZipFileContentDynamicParameters parameters = DynamicParameters as ZipFileContentDynamicParameters;
            return new ZipFileContentReaderWriter(this, path, parameters.FileMode, parameters.FileAccess, parameters.EncodingType, parameters.UsingByteEncoding);   
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            return new ZipFileContentDynamicParameters(ZipFileContentDynamicParameters.FileSystemCmdletProviderEncoding.UTF8, FileMode.Open, FileAccess.Read);
        }

        public IContentWriter GetContentWriter(string path)
        {
            ZipFileContentDynamicParameters parameters = DynamicParameters as ZipFileContentDynamicParameters;
            return new ZipFileContentReaderWriter(this, path, parameters.FileMode, parameters.FileAccess, parameters.EncodingType, parameters.UsingByteEncoding);
        }

        public object GetContentWriterDynamicParameters(string path)
        {
            return new ZipFileContentDynamicParameters(ZipFileContentDynamicParameters.FileSystemCmdletProviderEncoding.UTF8, FileMode.Open, FileAccess.ReadWrite);
        }

    }
}
