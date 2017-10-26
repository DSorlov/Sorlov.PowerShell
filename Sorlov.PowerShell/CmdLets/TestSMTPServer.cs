using System;
using System.Management.Automation;
using System.Net;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using Sorlov.PowerShell.Dto.Network;

namespace Sorlov.PowerShell.Cmdlets
{
    [Cmdlet(VerbsDiagnostic.Test, "SMTPServer",SupportsShouldProcess=true)]
    [CmdletDescription("Checks if a smtpserer transaction is successfull",
        "This commands connects to a SMTP server and then creates a session and returns the status of that test.")]
    [Example(Code = "Test-SMTPSever mail.remotesite.com test@test.com test@domain2.com", Remarks = "Tries to validate test@test.com sending a email to test@domain2.com via mail.remotesite.com via port 25")]
    [Example(Code = "Test-SMTPSever mail.remotesite.com test@test.com test@domain2.com 587", Remarks = "Tries to validate test@test.com sending a email to test@domain2.com via mail.remotesite.com via port 587")]
    public class TestSMTPServer : PSCmdlet
    {
        #region "Private Parameters"
        int serverPort = 25;
        string serverName = string.Empty;
        string senderName = string.Empty;
        string recipientName = string.Empty;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The host to test", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string ComputerName
        {
            get { return serverName; }
            set { serverName = value; }
        }
        [Parameter(Position = 1, Mandatory = true, HelpMessage = "The host to test", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string From
        {
            get { return senderName; }
            set { senderName = value; }
        }
        [Parameter(Position = 2, Mandatory = true, HelpMessage = "The host to test", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string To
        {
            get { return recipientName; }
            set { recipientName = value; }
        }
        [Parameter(Position = 3, HelpMessage = "Which port to connect to", ValueFromPipelineByPropertyName = true)]
        [ValidateRange(1,65534)]
        public int Port
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");
        }
        #endregion

        private SMTPConversationResult SMTPTalker(DateTime startTime, StreamReader reader, StreamWriter writer, string currentHost)
        {
            string inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));
            TimeSpan duration = DateTime.Now - startTime;

            if (inData.Substring(0, 3) != "220")
                return new SMTPConversationResult("No welcome banner: " + inData.Substring(4), false, duration, senderName, recipientName, currentHost);

            WriteVerbose("SEND> HELO " + Environment.MachineName);
            writer.WriteLine("HELO " + Environment.MachineName);
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));
            duration = DateTime.Now - startTime;

            if (inData.Substring(0, 3) != "250")
                return new SMTPConversationResult("Hostname not accepted: " + inData.Substring(4), false, duration, senderName, recipientName, currentHost);

            WriteVerbose("SEND> VRFY " + recipientName);
            writer.WriteLine("VRFY " + recipientName);
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));

            WriteVerbose("SEND> RSET");
            writer.WriteLine("RSET");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));

            WriteVerbose("SEND> EXPN " + recipientName);
            writer.WriteLine("EXPN " + recipientName);
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));

            WriteVerbose("SEND> RSET");
            writer.WriteLine("RSET");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));

            WriteVerbose("SEND> MAIL FROM: <" + senderName + ">");
            writer.WriteLine("MAIL FROM: <" + senderName + ">");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));
            duration = DateTime.Now - startTime;

            if (inData.Substring(0, 3) != "250")
                return new SMTPConversationResult("Sender not accepted: " + inData.Substring(4), false, duration, senderName, recipientName, currentHost);

            WriteVerbose("SEND> RCPT TO: <" + recipientName + ">");
            writer.WriteLine("RCPT TO: <" + recipientName + ">");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));
            duration = DateTime.Now - startTime;

            if (inData.Substring(0, 3) != "250")
                return new SMTPConversationResult("Recipient not accepted: " + inData.Substring(4), false, duration, senderName, recipientName, currentHost);

            WriteVerbose("SEND> RSET");
            writer.WriteLine("RSET");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));

            WriteVerbose("SEND> QUIT");
            writer.WriteLine("QUIT");
            writer.Flush();
            inData = reader.ReadLine();
            WriteVerbose(string.Format("RECV: {0}", inData));
            duration = DateTime.Now - startTime;

            if (inData.Substring(0, 3) != "220")
                return new SMTPConversationResult("Transaction failed to complete: " + inData.Substring(4), false, duration, senderName, recipientName, currentHost);
            else
                return new SMTPConversationResult("Transaction completed", true, duration, senderName, recipientName, currentHost);


        }

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            if (ShouldProcess(serverName, "Check mailserver"))
            {
                IPAddress address;
                IPAddress[] addresses = new IPAddress[] { };
                if (IPAddress.TryParse(serverName,out address))
                {
                    WriteVerbose(string.Format("{0} is a IP, will not look up. Proceeding to connection test..",serverName));
                    addresses = new IPAddress[] { address };
                }
                else
                {
                    WriteVerbose(string.Format("{0} is not a IP, will now look it up..",serverName));
                    try { addresses = System.Net.Dns.GetHostAddresses(serverName); }
                    catch { }
                    WriteVerbose(string.Format("Found {0} matches to that hostname..",addresses.Length));
                }

                if (addresses.Length < 1)
                {
                    WriteObject(new SMTPConversationResult("Mailserver hostname could not be resolved", false, TimeSpan.Zero,senderName,recipientName,serverName));
                }
                else
                {
                    foreach(IPAddress host in addresses)
                    {
                            TcpClient client = new TcpClient(host.AddressFamily);
                            DateTime startTime = DateTime.Now;
                            IAsyncResult clientConnect = client.BeginConnect(host, serverPort, null, null);
                            clientConnect.AsyncWaitHandle.WaitOne(1000, false);

                            if (client.Connected)
                            {
                                NetworkStream stream = client.GetStream();
                                StreamReader reader = new StreamReader(stream);
                                StreamWriter writer = new StreamWriter(stream);

                                Thread.Sleep(500);
                                SMTPConversationResult result = SMTPTalker(startTime,reader, writer, host.ToString());
                                WriteObject(result);

                                writer.Close();
                                reader.Close();
                                client.Close();
                            }
                            else
                                WriteObject(new SMTPConversationResult("Connection timed out", false, TimeSpan.Zero,senderName,recipientName,host.ToString()));
                        
                        
                        


                    }
                }
            }



        }

        #endregion

        #region "EndProcessing"
        protected override void EndProcessing()
        {
            base.EndProcessing();
            WriteVerbose("End processing");
        }
        #endregion

        #region "StopProcessing"
        protected override void StopProcessing()
        {
            base.StopProcessing();
            WriteVerbose("Stop processing");
        }
        #endregion

    }
}
