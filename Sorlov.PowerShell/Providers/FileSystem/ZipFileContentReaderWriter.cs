using System;
using System.Management.Automation.Provider;
using System.Text;
using System.IO;
using System.Collections;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    public class ZipFileContentReaderWriter : IContentReader, IContentWriter, IDisposable
    {
        private ZipFileProvider provider;
        private Stream stream;
        private bool usingByteEncoding;
        private Encoding contentEncoding;
        private StreamReader streamReader;
        private StreamWriter streamWriter;

        private string path;

        public ZipFileContentReaderWriter(ZipFileProvider provider, string path, FileMode mode, FileAccess access, Encoding contentEncoding,  bool usingByteEncoding)
        {
            this.path = path;
            this.provider = provider;
            this.contentEncoding = contentEncoding;
            this.usingByteEncoding = usingByteEncoding;

            stream = provider.ZipDrive.Archive.GetFile(provider.GetPathCaseInsensitivePath(provider.ParsePath(path,false))).GetStream(mode, access);
        }

        public void Close()
        {
            if (streamWriter != null) streamWriter.Flush();
            if (streamReader != null) streamReader.DiscardBufferedData();

            if (streamWriter != null) streamWriter.Close();
            if (streamReader != null) streamReader.Close();
            stream.Close();
        }

        public System.Collections.IList Read(long readCount)
        {
            if (streamReader == null) streamReader = new StreamReader(stream, contentEncoding);

            ArrayList blocks = new ArrayList();
            for (long index = 0L; index < readCount; index++)
            {
                if (!usingByteEncoding)
                {
                    if (!ReadByLine(blocks))
                        break;
                }
                else
                {
                    if (!ReadByte(blocks))
                        break;
                }   
            }
            return (IList)blocks.ToArray();

        }

        public bool ReadByLine(ArrayList blocks)
        {
            object line = (object)streamReader.ReadLine();
            if (line!=null) blocks.Add(line);
            return streamReader.Peek() != -1;
        }

        public bool ReadByte(ArrayList blocks)
        {
            int num = stream.ReadByte();

            if (num == -1)
                return false;
            else
                blocks.Add((object)(byte)num);

            return true;
        }

        public void Seek(long offset, System.IO.SeekOrigin origin)
        {
            if (streamReader == null) streamReader = new StreamReader(stream, contentEncoding);
            if (streamWriter == null) streamWriter = new StreamWriter(stream, contentEncoding);

            streamWriter.Flush();

            stream.Seek(offset, origin);

            streamWriter.Flush();
            this.streamReader.DiscardBufferedData();

        }

        public void Dispose()
        {
            if (streamReader != null) streamReader.Close();
            if (streamWriter != null) streamWriter.Close();
            stream.Close();
        }

        private void WriteObject(object content)
        {
            if (content == null)
                return;

            if (this.usingByteEncoding)
                    this.stream.WriteByte((byte)content);
            else
                streamWriter.WriteLine(content.ToString());
            streamWriter.Flush();
        }

        public System.Collections.IList Write(System.Collections.IList content)
        {
            if (streamWriter == null) streamWriter = new StreamWriter(stream, contentEncoding);

            foreach (object content1 in (IEnumerable)content)
            {
                object[] objArray = content1 as object[];
                if (objArray != null)
                {
                    foreach (object content2 in objArray)
                        this.WriteObject(content2);
                }
                else
                    this.WriteObject(content1);
            }
            return content;
        }
    }
}
