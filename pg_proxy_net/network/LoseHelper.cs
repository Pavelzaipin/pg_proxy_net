
namespace pg_proxy_net.network
{


    public class LoseHelper
    {


        public static void ListUsedTcpPorts()
        {
            System.Collections.ArrayList usedPort = new System.Collections.ArrayList();

            System.Net.NetworkInformation.IPGlobalProperties ipGlobalProperties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            System.Net.NetworkInformation.TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (System.Net.NetworkInformation.TcpConnectionInformation tcpInfo in tcpConnInfoArray)
            {
                System.Console.WriteLine("Port {0} {1} {2} ", tcpInfo.LocalEndPoint, tcpInfo.RemoteEndPoint, tcpInfo.State);
                usedPort.Add(tcpInfo.LocalEndPoint.Port);
            }

            // System.Net.NetworkInformation.UdpStatistics? a = ipGlobalProperties.GetUdpIPv4Statistics();

            // https://github.com/DamonMohammadbagher/TCPMon
            // Note: with/without "rus as Admin", you can use this code ;)

            // System.Collections.IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();

            // while (myEnum.MoveNext())
            // {
            //    System.Net.NetworkInformation.TcpConnectionInformation TCPInfo = (System.Net.NetworkInformation.TcpConnectionInformation)myEnum.Current;
            //    System.Console.WriteLine("Port {0} {1} {2} ", TCPInfo.LocalEndPoint, TCPInfo.RemoteEndPoint, TCPInfo.State);
            //    usedPort.Add(TCPInfo.LocalEndPoint.Port);
            // }
        } // End Sub ListUsedTcpPorts 


        public static void ListTcp()
        {
            // https://github.com/DamonMohammadbagher/TCPMon

            foreach (pg_proxy_net.network.windows.TcpRow row in pg_proxy_net.network.windows.ManagedIpHelper.GetExtendedTcpTable(true))
            {
                string name = "Unknown";
                try
                {
                    using (var p = System.Diagnostics.Process.GetProcessById(row.ProcessId))
                    {
                        name = p.ProcessName;
                        // name = p.MainModule.FileName; 
                    }
                }
                catch (System.Exception ex)
                {
                    name = ex.GetType().FullName + ": " + ex.Message;
                }

                System.Console.Write(name);
                System.Console.Write(": ");
                System.Console.WriteLine(row.LocalEndPoint.Port);
            } // Next row 


            //  I can read the /proc/$PID/net/tcp file for example
            //  and get information about TCP ports opened by the process.

            //  No. That file is not a list of tcp ports opened by the process. 
            //  It is a list of all open tcp ports in the current network namespace, 
            //  and for processes running in the same network namespace is identical to the contents of /proc/net/tcp.

            //  To find ports opened by your process,
            //  you would need to get a list of socket descriptors from 
            //  /proc/<pid>/fd
            //  , and then match those descriptors to the inode field of
            //  /proc/net/tcp.


            // root@prodesk:~# cat /proc/net/tcp
            //sl  local_address rem_address   st tx_queue rx_queue tr tm->when retrnsmt   uid  timeout inode
            // 0: 0100007F:0BEA 00000000:0000 0A 00000000:00000000 00:00000000 00000000   130        0 30439 1 ffff8ff112da71c0 100 0 0 10 0
            // 1: 00000000:024B 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 35440 1 ffff8ff10dde6900 100 0 0 10 0
            // 2: 00000000:008B 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 33468 1 ffff8ff104856900 100 0 0 10 0
            // 3: 0100007F:810D 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 28299 1 ffff8ff1089b1a40 100 0 0 10 0
            // 4: 00000000:0050 00000000:0000 0A 00000000:00000000 00:00000000 00000000     0        0 27926 1 ffff8ff1143c9a40 100 0 1 10 0


            // root @prodesk:/proc/self/fd# ls
            // 0  1  2  255

            // readlink /proc/self/exe
            // /usr/bin/bash


        }


        public static void ReadLineByLine()
        {
            string path = "/home/janbodnar/Documents/thermopylae.txt";
            string? line = string.Empty;

            using (System.IO.Stream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.UTF8))
                {

                    while ((line = sr.ReadLine()) != null)
                    {
                        System.Console.WriteLine(line);
                    } // Whend 

                } // End Using sr

            } // End Using fs 

        } // End Sub Foo 



        public static async System.Threading.Tasks.Task ReadLineByLineAsync()
        {
            string path = "/home/janbodnar/Documents/thermopylae.txt";
            string? line = string.Empty;

            await using (System.IO.Stream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
            {
                using (System.IO.TextReader sr = new System.IO.StreamReader(fs, System.Text.Encoding.UTF8))
                {

                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        System.Console.WriteLine(line);
                    } // Whend 

                } // End Using sr

            } // End Using fs 

        } // End Sub Foo 


    } // End Class 


}
