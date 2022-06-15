#nullable enable

namespace NetProxy
{


    internal static class Program
    {
        // https://stackoverflow.com/a/48274520
        private static System.Timers.Timer ﾠ300;
        private static bool s_showSparta = true;

        private static void Sparta_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (s_showSparta)
                System.Console.Title = "THIS IS MADNESS!!!    Madness huh?    THIS IS SPARTA!!!!!!! ";
            else
                System.Console.Title = "https://github.com/ststeiger/pg_proxy_net";

            s_showSparta ^= true;
        }


        static Program()
        {
            ﾠ300 = new System.Timers.Timer(4000);
            ﾠ300.AutoReset = true;
            ﾠ300.Elapsed += new System.Timers.ElapsedEventHandler(Sparta_Elapsed!);
            ﾠ300.Start();
        }


        // https://stackoverflow.com/questions/49290683/how-to-redirect-root-to-swagger-in-asp-net-core-2-x
        private static void Main(string[] args)
        {
            StartProxy();

            // ExpressProfiler.TestLexer inst = new ExpressProfiler.TestLexer();
            // inst.LexMe("SELECT 123 AS 'abc', LEFT(x, 3) FROM mytable LEFT JOIN foobar ON foobar.id = 123");


            // pg_proxy_net.SyntaxHighlighting.Lexer.SqlStringReader.Test();
            // pg_proxy_net.SyntaxHighlighting.Lexer.SqlStringReader.Lexme("");
            // pg_proxy_net.SyntaxHighlighting.Lexer.SqlStringReader.LexToHtml("");



            // string x = TestLucene.CrapLord.LinuxNativeMethods.ReadLink("path");

            // https://www.kernel.org/doc/Documentation/networking/proc_net_tcp.txt


            // System.Collections.Generic.List<System.Collections.Generic.List<string>> lines = ProcFsReader.ReadProcNetTcp();
            // ProcFsReader.Test();

            // NetStat.Test();
            // pg_proxy_net.network.LoseHelper.ListUsedTcpPorts();
            // pg_proxy_net.network.LoseHelper.ListTcp();
        }






        private static void StartProxy()
        {
            try
            {
                string? configJson = System.IO.File.ReadAllText("config.json");
                System.Collections.Generic.Dictionary<string, ProxyConfig>? configs = System.Text.Json.JsonSerializer
                    .Deserialize<System.Collections.Generic.Dictionary<string, ProxyConfig>>(configJson);

                if (configs == null)
                {
                    throw new System.Exception("configs is null");
                }


                System.Collections.Generic.List<System.Threading.Tasks.Task> tasks = 
                    new System.Collections.Generic.List<System.Threading.Tasks.Task>();

                foreach (System.Collections.Generic.KeyValuePair<string, ProxyConfig> kvp in configs)
                {
                    foreach (System.Threading.Tasks.Task? x in ProxyFromConfig(kvp.Key, kvp.Value))
                    {
                        tasks.Add(x);
                    }
                }

                System.Threading.Tasks.Task.WhenAll(tasks); //.Wait();





                System.Console.ResetColor();

                // System.Console.WriteLine("--- Press ENTER to stop profiling --- ");
                // System.Console.ReadLine();

                // System.Console.WriteLine("--- Press any key to stop profiling --- ");
                // System.Console.ReadKey();

                System.Console.WriteLine("--- Press BACKSPACE to stop profiling --- ");

                System.ConsoleKey cc = default(System.ConsoleKey);

                do
                {
                    // THIS IS MADNESS!!!   Madness huh?   THIS IS SPARTA!!!!!!! 
                    while (!System.Console.KeyAvailable)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    cc = System.Console.ReadKey().Key;

                    if (cc == System.ConsoleKey.C)
                        System.Console.Clear();

                    // } while (cc != System.ConsoleKey.Enter);
                } while (cc != System.ConsoleKey.Backspace);

            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"An error occurred : {ex}");
            }
        }


        private static System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> 
            ProxyFromConfig(string proxyName, ProxyConfig proxyConfig)
        {
            ushort? forwardPort = proxyConfig.forwardPort;
            ushort? localPort = proxyConfig.localPort;
            string? forwardIp = proxyConfig.forwardIp;
            string? localIp = proxyConfig.localIp;
            string? protocol = proxyConfig.protocol;
            try
            {
                if (forwardIp == null)
                {
                    throw new System.Exception("forwardIp is null");
                }
                if (!forwardPort.HasValue)
                {
                    throw new System.Exception("forwardPort is null");
                }
                if (!localPort.HasValue)
                {
                    throw new System.Exception("localPort is null");
                }
                if (protocol != "udp" && protocol != "tcp" && protocol != "any")
                {
                    throw new System.Exception($"protocol is not supported {protocol}");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Failed to start {proxyName} : {ex.Message}");
                throw;
            }

            bool protocolHandled = false;
            if (protocol == "udp" || protocol == "any")
            {
                protocolHandled = true;
                System.Threading.Tasks.Task task;
                try
                {
                    UdpProxy proxy = new UdpProxy();
                    task = proxy.Start(forwardIp, forwardPort.Value, localPort.Value, localIp);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Failed to start {proxyName} : {ex.Message}");
                    throw;
                }

                yield return task;
            }

            if (protocol == "tcp" || protocol == "any")
            {
                protocolHandled = true;
                System.Threading.Tasks.Task task;
                try
                {
                    TcpProxy? proxy = new TcpProxy();
                    task = proxy.Start(forwardIp, forwardPort.Value, localPort.Value, localIp);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"Failed to start {proxyName} : {ex.Message}");
                    throw;
                }

                yield return task;
            }

            if (!protocolHandled)
            {
                throw new System.InvalidOperationException($"protocol not supported {protocol}");
            }
        }
    }


    public class ProxyConfig
    {
        public string? protocol { get; set; }
        public ushort? localPort { get; set; }
        public string? localIp { get; set; }
        public string? forwardIp { get; set; }
        public ushort? forwardPort { get; set; }
    }


    internal interface IProxy
    {
        System.Threading.Tasks.Task Start(string remoteServerHostNameOrAddress, ushort remoteServerPort, ushort localPort, string? localIp = null);
    }


}