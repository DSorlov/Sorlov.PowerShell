using System.Management.Automation;
using System.Text;

namespace Sorlov.PowerShell.Providers.FileSystem
{
    public class ZipFileContentDynamicParameters
    {
        private FileSystemCmdletProviderEncoding streamType; 
        private System.IO.FileMode fileMode;
        private System.IO.FileAccess fileAccess;

        public ZipFileContentDynamicParameters(FileSystemCmdletProviderEncoding streamType, System.IO.FileMode fileMode, System.IO.FileAccess fileAccess)
        {
            this.streamType = streamType;
            this.fileMode = fileMode;
            this.fileAccess = fileAccess;
        }

        public enum FileSystemCmdletProviderEncoding
        {
            Unknown,
            String,
            Unicode,
            Byte,
            BigEndianUnicode,
            UTF8,
            UTF7,
            Ascii,
        }

        [Parameter]
        public System.IO.FileAccess FileAccess
        {
            get
            {
                return fileAccess;
            }
            set
            {
                fileAccess = value;
            }
        }

        [Parameter]
        public System.IO.FileMode FileMode
        {
            get
            {
                return fileMode;
            }
            set
            {
                fileMode = value;
            }
        }
       

        [Parameter]
        public FileSystemCmdletProviderEncoding Encoding
        {
            get
            {
                return this.streamType;
            }
            set
            {
                this.streamType = value;
            }
        }

        public Encoding EncodingType
        {
            get
            {
                return GetEncodingFromEnum(this.streamType);
            }
        }

        public bool UsingByteEncoding
        {
            get
            {
                return this.streamType == FileSystemCmdletProviderEncoding.Byte;
            }
        }

        private static System.Text.Encoding GetEncodingFromEnum(FileSystemCmdletProviderEncoding type)
        {
            System.Text.Encoding unicode = System.Text.Encoding.Unicode;
            System.Text.Encoding encoding;
            switch (type)
            {
                case FileSystemCmdletProviderEncoding.String:
                    encoding = System.Text.Encoding.Unicode;
                    break;
                case FileSystemCmdletProviderEncoding.Unicode:
                    encoding = System.Text.Encoding.Unicode;
                    break;
                case FileSystemCmdletProviderEncoding.BigEndianUnicode:
                    encoding = System.Text.Encoding.BigEndianUnicode;
                    break;
                case FileSystemCmdletProviderEncoding.UTF8:
                    encoding = System.Text.Encoding.UTF8;
                    break;
                case FileSystemCmdletProviderEncoding.UTF7:
                    encoding = System.Text.Encoding.UTF7;
                    break;
                case FileSystemCmdletProviderEncoding.Ascii:
                    encoding = System.Text.Encoding.ASCII;
                    break;
                default:
                    encoding = System.Text.Encoding.Unicode;
                    break;
            }
            return encoding;
        }

    }
}
