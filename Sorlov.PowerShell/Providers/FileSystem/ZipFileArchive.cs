using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sorlov.PowerShell.Providers.FileSystem
{
   public class ZipFileArchive : IDisposable {
      private object external;
 
      public enum CompressionMethodEnum { 
         Stored, 
         Deflated 
      };
      public enum DeflateOptionEnum { 
         Normal, 
         Maximum, 
         Fast, 
         SuperFast 
      };
 
      private ZipFileArchive() {
      }
 
      private ZipFileArchive(object external) {
         this.external = external;
      }
 
      public static ZipFileArchive OpenFromFile(string path, FileMode mode, FileAccess access, FileShare share, bool streaming) {
         Type packageType = typeof(System.IO.Packaging.Package);

         Type zipArchiveType = packageType.Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
         MethodInfo methodInfo = zipArchiveType.GetMethod("OpenOnFile", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

         object zipObject = methodInfo.Invoke(null, new object[] { path, mode, access, share, streaming });

         Type blockManagerType = packageType.Assembly.GetType("MS.Internal.IO.Zip.ZipIOBlockManager");
         FieldInfo blockManagerField = zipArchiveType.GetField("_blockManager", BindingFlags.Instance | BindingFlags.NonPublic);

          object blockManager = blockManagerField.GetValue(zipObject);
         FieldInfo encodingField = blockManagerType.GetField("_encoding", BindingFlags.Instance | BindingFlags.NonPublic);
         encodingField.SetValue(blockManager, new ZipFileASCIIEncoding());

         return new ZipFileArchive(zipObject);
      }
 
      public static ZipFileArchive OpenFromFile(string path) {
         return ZipFileArchive.OpenFromFile(path, FileMode.Open, FileAccess.Read, FileShare.Read, false);
      }
 
      public static ZipFileArchive OpenFromStream(Stream stream) {
         return ZipFileArchive.OpenFromStream(stream, FileMode.OpenOrCreate, FileAccess.ReadWrite, false);
      }
 
      public static ZipFileArchive OpenFromStream(Stream stream, FileMode mode, FileAccess access, bool streaming) {
          Type zipArchiveType = typeof(System.IO.Packaging.Package).Assembly.GetType("MS.Internal.IO.Zip.ZipArchive");
          MethodInfo methodInfo = zipArchiveType.GetMethod("OpenOnStream", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
          
          object zipObject = methodInfo.Invoke(null, new object[] { stream, mode, access, streaming });

          Type blockManagerType = zipArchiveType.Assembly.GetType("MS.Internal.IO.Zip.ZipIOBlockManager");
          FieldInfo blockManagerField = zipArchiveType.GetField("_blockManager", BindingFlags.Instance | BindingFlags.NonPublic);

          object blockManager = blockManagerField.GetValue(zipObject);
          FieldInfo encodingField = blockManagerType.GetField("_encoding", BindingFlags.Instance | BindingFlags.NonPublic);
          encodingField.SetValue(blockManager, new ZipFileASCIIEncoding());

          return new ZipFileArchive(zipObject);
      }
 
      public ZipFileInfo AddFile(string path) {
         return this.AddFile(path, CompressionMethodEnum.Deflated, DeflateOptionEnum.Normal);
      }
 
      public ZipFileInfo AddFile(string path, CompressionMethodEnum compmeth, DeflateOptionEnum option) {
         var type = external.GetType();
         var meth = type.GetMethod("AddFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         var comp = type.Assembly.GetType("MS.Internal.IO.Zip.CompressionMethodEnum").GetField(compmeth.ToString()).GetValue(null);
         var opti = type.Assembly.GetType("MS.Internal.IO.Zip.DeflateOptionEnum").GetField(option.ToString()).GetValue(null);
         return new ZipFileInfo(meth.Invoke(external, new object[] { path, comp, opti }));
      }
 
      public void DeleteFile(string name) {
         var meth = external.GetType().GetMethod("DeleteFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         meth.Invoke(external, new object[] { name });
      }
 
      public void Dispose() {
         ((IDisposable)external).Dispose();
      }
      public ZipFileInfo GetFile(string name) {
         var meth = external.GetType().GetMethod("GetFile", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
         return new ZipFileInfo(meth.Invoke(external, new object[] { name }));
      }

      public bool FileExists(string name)
      {
          var meth = external.GetType().GetMethod("FileExists", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          return (bool)meth.Invoke(external, new object[] { name });
      }

       public void Flush()
      {
          var meth = external.GetType().GetMethod("Flush", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
          meth.Invoke(external, null);
      }

       public IEnumerable<ZipFileInfo> Files
      {
         get {
            var meth = external.GetType().GetMethod("GetFiles", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var coll = meth.Invoke(external, null) as System.Collections.IEnumerable; //ZipFileInfoCollection
            foreach (var p in coll) yield return new ZipFileInfo(p);
         }
      }
 
      public IEnumerable<string> FileNames {
         get {
            return Files.Select(p => p.Name).OrderBy(p => p);
         }
      }
 
      public static void CopyTo(Stream input, Stream output) {
         byte[] buffer = new byte[16 * 1024]; 
         int bytesRead;
 
         while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0) {
            output.Write(buffer, 0, bytesRead);
         }

      }

      public sealed class ZipFileASCIIEncoding : ASCIIEncoding
      {
          private readonly Encoding encoding = GetEncoding(858);

          public override byte[] GetBytes(string s)
          {
              return this.encoding.GetBytes(s);
          }

          public override string GetString(byte[] bytes)
          {
              return this.encoding.GetString(bytes);
          }
      }

      public struct ZipFileInfo {
         internal object external;
 
         internal ZipFileInfo(object external) {
            this.external = external;
         }
 
         private object GetProperty(string name) {
            return external.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(external, null);
         }
 
         public override string ToString() {
            return Name;
         }
 
         public string Name {
            get { return (string)GetProperty("Name"); }
         }
 
         public DateTime LastModFileDateTime {
            get { return (DateTime)GetProperty("LastModFileDateTime"); }
         }
 
         public bool FolderFlag {
            get { return (bool)GetProperty("FolderFlag"); }
         }
 
         public bool VolumeLabelFlag {
            get { return (bool)GetProperty("VolumeLabelFlag"); }
         }
 
         public object CompressionMethod {
            get { return GetProperty("CompressionMethod"); }
         }
 
         public object DeflateOption {
            get { return GetProperty("DeflateOption"); }
         }
 
         public Stream GetStream() {
            return this.GetStream(FileMode.Open, FileAccess.Read);
         }
 
         public Stream GetStream(FileMode mode, FileAccess access) {
            var meth = external.GetType().GetMethod("GetStream", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return (Stream)meth.Invoke(external, new object[] { mode, access });
         }
 
      } 
   }
} 
