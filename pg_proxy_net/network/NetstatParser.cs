
namespace NetProxy
{


    /// <summary>
    /// Static class that returns the list of processes and the ports those processes use.
    /// </summary>
    public static class ProcessPorts
    {
        /// <summary>
        /// A list of ProcesesPorts that contain the mapping of processes and the ports that the process uses.
        /// </summary>
        public static System.Collections.Generic.List<ProcessPort> ProcessPortMap
        {
            get
            {
                return GetNetStatPorts();
            }
        }


        /// <summary>
        /// This method distills the output from netstat -a -n -o into a list of ProcessPorts that provide a mapping between
        /// the process (name and id) and the ports that the process is using.
        /// </summary>
        /// <returns></returns>
        private static System.Collections.Generic.List<ProcessPort> GetNetStatPorts()
        {
            System.Collections.Generic.List<ProcessPort> ProcessPorts = new System.Collections.Generic.List<ProcessPort>();

            try
            {
                using (System.Diagnostics.Process Proc = new System.Diagnostics.Process())
                {

                    System.Diagnostics.ProcessStartInfo StartInfo = new System.Diagnostics.ProcessStartInfo();
                    StartInfo.FileName = "netstat.exe";
                    StartInfo.Arguments = "-a -n -o";
                    StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    StartInfo.UseShellExecute = false;
                    StartInfo.RedirectStandardInput = true;
                    StartInfo.RedirectStandardOutput = true;
                    StartInfo.RedirectStandardError = true;

                    Proc.StartInfo = StartInfo;
                    Proc.Start();

                    System.IO.StreamReader StandardOutput = Proc.StandardOutput;
                    System.IO.StreamReader StandardError = Proc.StandardError;

                    string NetStatContent = StandardOutput.ReadToEnd() + StandardError.ReadToEnd();
                    string NetStatExitStatus = Proc.ExitCode.ToString();

                    if (NetStatExitStatus != "0")
                    {
                        System.Console.WriteLine("NetStat command failed.   This may require elevated permissions.");
                    }

                    string[] NetStatRows = System.Text.RegularExpressions.Regex.Split(NetStatContent, "\r\n");

                    foreach (string NetStatRow in NetStatRows)
                    {
                        string[] Tokens = System.Text.RegularExpressions.Regex.Split(NetStatRow, "\\s+");
                        if (Tokens.Length > 4 && (Tokens[1].Equals("UDP") || Tokens[1].Equals("TCP")))
                        {
                            string IpAddress = System.Text.RegularExpressions.Regex.Replace(Tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            try
                            {
                                ProcessPorts.Add(new ProcessPort(
                                    Tokens[1] == "UDP" ? GetProcessName(System.Convert.ToInt16(Tokens[4])) : GetProcessName(System.Convert.ToInt16(Tokens[5])),
                                    Tokens[1] == "UDP" ? System.Convert.ToInt16(Tokens[4]) : System.Convert.ToInt16(Tokens[5]),
                                    IpAddress.Contains("1.1.1.1") ? string.Format("{0}v6", Tokens[1]) : string.Format("{0}v4", Tokens[1]),
                                    System.Convert.ToInt32(IpAddress.Split(':')[1])
                                ));
                            }
                            catch
                            {
                                System.Console.WriteLine("Could not convert the following NetStat row to a Process to Port mapping.");
                                System.Console.WriteLine(NetStatRow);
                            }
                        }
                        else
                        {
                            if (!NetStatRow.Trim().StartsWith("Proto") && !NetStatRow.Trim().StartsWith("Active") && !string.IsNullOrWhiteSpace(NetStatRow))
                            {
                                System.Console.WriteLine("Unrecognized NetStat row to a Process to Port mapping.");
                                System.Console.WriteLine(NetStatRow);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return ProcessPorts;
        }

        /// <summary>
        /// Private method that handles pulling the process name (if one exists) from the process id.
        /// </summary>
        /// <param name="ProcessId"></param>
        /// <returns></returns>
        private static string GetProcessName(int ProcessId)
        {
            string procName = "UNKNOWN";

            try
            {
                procName = System.Diagnostics.Process.GetProcessById(ProcessId).ProcessName;
            }
            catch { }

            return procName;
        }
    }

    /// <summary>
    /// A mapping for processes to ports and ports to processes that are being used in the system.
    /// </summary>
    public class ProcessPort
    {
        private string _ProcessName = string.Empty;
        private int _ProcessId = 0;
        private string _Protocol = string.Empty;
        private int _PortNumber = 0;

        /// <summary>
        /// Internal constructor to initialize the mapping of process to port.
        /// </summary>
        /// <param name="ProcessName">Name of process to be </param>
        /// <param name="ProcessId"></param>
        /// <param name="Protocol"></param>
        /// <param name="PortNumber"></param>
        internal ProcessPort(string ProcessName, int ProcessId, string Protocol, int PortNumber)
        {
            _ProcessName = ProcessName;
            _ProcessId = ProcessId;
            _Protocol = Protocol;
            _PortNumber = PortNumber;
        }

        public string ProcessPortDescription
        {
            get
            {
                return string.Format("{0} ({1} port {2} pid {3})", _ProcessName, _Protocol, _PortNumber, _ProcessId);
            }
        }
        public string ProcessName
        {
            get { return _ProcessName; }
        }
        public int ProcessId
        {
            get { return _ProcessId; }
        }
        public string Protocol
        {
            get { return _Protocol; }
        }
        public int PortNumber
        {
            get { return _PortNumber; }
        }
    }


    internal class NetStat 
    {
        internal static void Test()
        {
            foreach (ProcessPort p in ProcessPorts.ProcessPortMap.FindAll(
                    x => x.ProcessName.ToLower().Contains("skype")
                )
            )
            {
                System.Console.WriteLine(p.ProcessPortDescription);
            }

            foreach (ProcessPort p in ProcessPorts.ProcessPortMap.FindAll(x => x.PortNumber == 4444))
            {
                System.Console.WriteLine(p.ProcessPortDescription);
            }

            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadLine();
        }
    }


}
