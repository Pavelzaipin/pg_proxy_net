using NetProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetProxy
{
    public static class Profile
    {
        public static void StartProxy()
        {
            try
            {
                string? configJson = System.IO.File.ReadAllText("config.json");
                Dictionary<string, ProxyConfig>? configs = System.Text.Json.JsonSerializer
                    .Deserialize<Dictionary<string, ProxyConfig>>(configJson);

                if (configs == null)
                {
                    throw new Exception("configs is null");
                }


                List<Task> tasks = new List<Task>();

                foreach (KeyValuePair<string, ProxyConfig> kvp in configs)
                {
                    foreach (Task? x in ProxyFromConfig(kvp.Key, kvp.Value))
                    {
                        tasks.Add(x);
                    }
                }

                Task.WhenAll(tasks); //.Wait();

                Console.ResetColor();

                Console.WriteLine("--- Press BACKSPACE to stop profiling --- ");

                ConsoleKey cc = default(ConsoleKey);

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    cc = Console.ReadKey().Key;

                    if (cc == ConsoleKey.C)
                        Console.Clear();

                } while (cc != ConsoleKey.Backspace);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred : {ex}");
            }
        }


        private static IEnumerable<System.Threading.Tasks.Task>
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


    public interface IProxy
    {
        System.Threading.Tasks.Task Start(string remoteServerHostNameOrAddress, ushort remoteServerPort, ushort localPort, string? localIp = null);
    }
}
