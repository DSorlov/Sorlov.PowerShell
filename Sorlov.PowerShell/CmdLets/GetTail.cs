using System;
using System.Collections.Generic;
using System.Text;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.Management.Automation;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace Sorlov.PowerShell.Cmdlets
{

    [Cmdlet(VerbsCommon.Get, "Tail")]
    [CmdletDescription("Reads the last lines of any file",
        "This command reads the last lines of any files and optionally tracks it for changes.")]
    [Example(Code="Get-Tail c:\\log.txt",Remarks="Gets 10 last lines of the c:\\log.txt file")]
    [Example(Code = "Get-Tail c:\\log.txt -f", Remarks = "Gets 10 last lines of the c:\\log.txt file and then keeps following the files for any changes")]
    [OutputType(typeof(string))]
    public class GetTail : PSCmdlet
    {
        #region "Private Parameters"
        public static bool doCheck = false;
        string parFile = string.Empty;
        string parFilter = string.Empty;
        bool parFollow = false;
        int parLines = 5;

        private Regex lineFilterRegex;
        private long prevLen = 0;
        private FileSystemWatcher watcher;
        private string previous = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The file to read")]
        [ValidateNotNullOrEmpty()]
        public string Path
        {
            get { return parFile; }
            set { parFile = value; }
        }

        [Parameter(Position = 1, HelpMessage = "Number of lines to read")]
        public int Lines
        {
            get { return parLines; }
            set { parLines = value; }
        }

        [Parameter(Position = 2, HelpMessage = "Follow file and do not return until break")]
        public SwitchParameter Follow
        {
            get { return parFollow; }
            set { parFollow = value; }
        }

        [Parameter(Position = 3, HelpMessage = "Follow file and do not return until break")]
        public String Regexp
        {
            get { return parFilter; }
            set { parFilter = value; }
        }

        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose(string.Format("Begin processing, will print {0} lines from file {1}, follow={2}",parLines,parFile,parFollow));
        }
        #endregion      

        private string[] MakeTail(int nLines)
        {
            
            if (!string.IsNullOrEmpty(parFilter))
                lineFilterRegex = new Regex(parFilter);

            List<string> returndata = new List<string>();
            using (StreamReader sr = new StreamReader(new FileStream(parFile, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite)))
            {
                string line;
                while (null != (line = sr.ReadLine()))
                {
                    if (!string.IsNullOrEmpty(parFilter))
                    {
                        if (lineFilterRegex.IsMatch(line))
                        {
                            EnqueueLine(nLines, returndata, line);
                        }
                    }
                    else
                    {
                        EnqueueLine(nLines, returndata, line);
                    }
                }
            }
            return returndata.ToArray();
        }

        private void EnqueueLine(int nLines, List<string> returnlist, string line)
        {
            if (returnlist.Count >= nLines)
            {
                returnlist.RemoveAt(0);
            }
            returnlist.Add(line);
        }

        public static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            doCheck = true;
        }

        private void UpdateTail()
        {
            doCheck = false;
            WriteVerbose("Found change to the file, evaluating size");
            FileInfo fi = new FileInfo(parFile);
            if (fi.Length != prevLen)
            {
                WriteVerbose("Reading changes from file");
                if (fi.Length < prevLen)
                {
                    //assume truncated!
                    prevLen = 0;
                }
                using (FileStream stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
                {

                    stream.Seek(prevLen, SeekOrigin.Begin);
                    if (string.IsNullOrEmpty(parFilter))
                    {
                        string line;
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            line = sr.ReadToEnd();
                            if (!string.IsNullOrEmpty(line))
                                WriteObject(line.Replace("\r", "").Replace("\n", "").TrimStart());
                        }
                    }
                    else
                    {
                        char[] buffer = new char[4096];
                        StringBuilder current = new StringBuilder();
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            int nRead;
                            do
                            {
                                nRead = sr.ReadBlock(buffer, 0, 4096);
                                for (int i = 0; i < nRead; ++i)
                                {
                                    if (buffer[i] == '\n' || buffer[i] == '\r')
                                    {
                                        if (current.Length > 0)
                                        {
                                            string line = string.Concat(previous, current);
                                            if (lineFilterRegex.IsMatch(line))
                                            {
                                                WriteObject(line.Replace("\r", "").Replace("\n", "").TrimStart());
                                            }
                                        }
                                        current = new StringBuilder();
                                    }
                                    else
                                    {
                                        current.Append(buffer[i]);
                                    }
                                }
                            } while (nRead > 0);
                            if (current.Length > 0)
                            {
                                previous = current.ToString();
                            }
                        }
                    }
                }
                prevLen = fi.Length;
            }
            else
            {
                WriteVerbose("No change in size detected, quitting update");
            }
        }

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            
            if (!File.Exists(parFile))
                ThrowTerminatingError(new ErrorRecord(new FileNotFoundException("File does not exist:" + parFile),"200",ErrorCategory.OpenError,parFile));

            WriteVerbose("Reading initial tail");
            foreach (string data in MakeTail(parLines))
                WriteObject(data);

            if (parFollow)
            {
                WriteVerbose("Will follow this file now");

                watcher = new FileSystemWatcher(System.IO.Path.GetDirectoryName(parFile), System.IO.Path.GetFileName(parFile));
                watcher.NotifyFilter = NotifyFilters.Size;
                watcher.EnableRaisingEvents = true;
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);

                FileInfo fi = new FileInfo(parFile);
                prevLen = fi.Length;

                while (watcher.EnableRaisingEvents)
                    {
                        if (doCheck) UpdateTail();
                        Thread.Sleep(25);
                    }

                WriteVerbose("Stopping to follow file");
                watcher.EnableRaisingEvents = false;
            }
        }
        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            watcher.EnableRaisingEvents = false;
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            watcher.EnableRaisingEvents = false;
            WriteVerbose("Stop processing inited");
        }
        #endregion

    }
}
