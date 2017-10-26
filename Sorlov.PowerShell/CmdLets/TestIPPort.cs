using System;
using System.Text;
using System.Management.Automation;
using System.Net;
using Sorlov.PowerShell.Lib.Core.Attributes;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Sorlov.PowerShell.Dto.Network;

namespace Sorlov.PowerShell.Cmdlets
{

    [Cmdlet(VerbsDiagnostic.Test, "IPPort",SupportsShouldProcess=true)]
    [CmdletDescription("Checks if a TCP/UDP port is open on a system",
        "This command connects to specified port and returns the status of that port. UDP is stateless and port open prediction on those should be considered low-fidelity.")]
    [Example(Code = "Test-IPPort host.remotesite.com", Remarks = "Checks if port tcp 443 on host.remotesite.com is open")]
    [Example(Code = "Test-IPPort host.remotesite.com 80", Remarks = "Checks if port tcp 80 on host.remotesite.com is open")]
    public class TestIPPort: PSCmdlet
    {
        #region "Private Parameters"
        int ipPort = 443;
        int ipTimeout = 1000;
        string ipHost = string.Empty;
        ProtocolType ipProto = ProtocolType.Tcp;
        #endregion

        #region "Public Parameters"
        [Parameter(Position = 0, Mandatory = true, HelpMessage = "The host to test", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        [ValidateNotNullOrEmpty()]
        public string ComputerName
        {
            get { return ipHost; }
            set { ipHost = value; }
        }
        [Parameter(Position = 1, HelpMessage = "Which port to connect to", ValueFromPipelineByPropertyName = true)]
        [ValidateRange(1,65534)]
        public int Port
        {
            get { return ipPort; }
            set { ipPort = value; }
        }
        [Parameter(Position = 2, HelpMessage = "Which protocol to test", ValueFromPipelineByPropertyName = true)]
        [ValidateSet("Tcp","Udp")]
        public string Protocol
        {
            get { return ipProto.ToString(); }
            set { if (value == "Tcp") { ipProto = ProtocolType.Tcp; } else { ipProto = ProtocolType.Udp; } }
        }
        [Parameter(Position = 3, HelpMessage = "How long to wait", ValueFromPipelineByPropertyName = true)]
        [ValidateRange(1, 10)]
        public int Timeout
        {
            get { return ipTimeout/1000; }
            set { ipTimeout = value*1000; }
        }
        #endregion

        #region "BeginProcessing"
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            WriteVerbose("Begin processing");
        }
        #endregion

        #region "ProcessRecord"
        protected override void ProcessRecord()
        {
            if (ShouldProcess(string.Format("{0}:{1}[{2}]",ipHost,ipPort,ipProto), "Check port"))
            {
                IPAddress address;
                IPAddress[] addresses = new IPAddress[] { };
                if (IPAddress.TryParse(ipHost,out address))
                {
                    WriteVerbose(string.Format("{0} is a IP, will not look up. Proceeding to connection test..",ipHost));
                    addresses = new IPAddress[] { address };
                }
                else
                {
                    WriteVerbose(string.Format("{0} is not a IP, will now look it up..",ipHost));
                    try { addresses = System.Net.Dns.GetHostAddresses(ipHost); }
                    catch { }
                    WriteVerbose(string.Format("Found {0} matches to that hostname..",addresses.Length));
                }

                if (addresses.Length < 1)
                {
                    WriteError(new ErrorRecord(new InvalidDataException("No such host found"),"1",ErrorCategory.InvalidData,ipHost));
                    WriteObject(new IPTestPortResult(ipHost, ipPort, IPAddress.None, false, new System.IO.IOException("Host not found"), null, ipProto, TimeSpan.Zero, IPStatus.Unknown));
                }
                else
                {
                    foreach(IPAddress host in addresses)
                    {
                        Ping ping = new Ping();
                        PingReply pingReply = ping.Send(host, ipTimeout);

                        if (ipProto == ProtocolType.Tcp)
                        {
                            TcpClient client = new TcpClient(host.AddressFamily);
                            DateTime startTime = DateTime.Now;
                            IAsyncResult clientConnect = client.BeginConnect(host, ipPort, null, null);
                            clientConnect.AsyncWaitHandle.WaitOne(ipTimeout, false);
                            TimeSpan duration = DateTime.Now - startTime;

                            if (client.Connected)
                            {
                                string networkData = null;
                                NetworkStream stream = client.GetStream();
                                StreamReader reader = new StreamReader(stream);
                                Thread.Sleep(500);
                                if (stream.DataAvailable) networkData = reader.ReadLine();
                                reader.Close();
                                stream.Close();
                                WriteObject(new IPTestPortResult(ipHost, ipPort, host, true, null, networkData, ipProto, duration,pingReply.Status));
                            }
                            else
                            {
                                WriteObject(new IPTestPortResult(ipHost, ipPort, host, false, new TimeoutException("Connect timed out"), null, ipProto, duration,pingReply.Status));
                            }

                        }
                        else
                        {
                            ASCIIEncoding encoder = new ASCIIEncoding();
                            byte[] data = encoder.GetBytes(DateTime.UtcNow.ToFileTimeUtc().ToString());
                            UdpClient udpClient = new UdpClient(host.AddressFamily);
                            udpClient.Client.ReceiveTimeout = ipTimeout;
                            udpClient.Connect(host, ipPort);
                            DateTime startTime = DateTime.Now;
                            udpClient.Send(data, data.Length);
                            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);

                            try
                            {
                                byte[] remoteData = udpClient.Receive(ref endpoint);
                                TimeSpan duration = DateTime.Now - startTime;
                                WriteObject(new IPTestPortResult(ipHost, ipPort, host, true, null, encoder.GetString(remoteData), ipProto, duration,pingReply.Status));
                            }
                            catch(SocketException e)
                            {
                                TimeSpan duration = DateTime.Now - startTime;
                                WriteObject(new IPTestPortResult(ipHost, ipPort, host, false, e, null, ipProto, duration,pingReply.Status));
                            }
                            finally
                            {
                                udpClient.Close();
                            }

                            

                        }   
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
