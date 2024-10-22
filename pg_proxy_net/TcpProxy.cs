#nullable enable
using pg_proxy_net;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetProxy
{
    internal class TcpProxy : IProxy
    {


        /// <summary>
        /// Milliseconds
        /// </summary>
        public int ConnectionTimeout { get; set; } = (4 * 60 * 1000);

        public async Task Start(string remoteServerHostNameOrAddress, ushort remoteServerPort, ushort localPort, string? localIp)
        {
            ConcurrentBag<TcpConnection>? connections = new ConcurrentBag<TcpConnection>();

            IPAddress localIpAddress = string.IsNullOrEmpty(localIp) ? IPAddress.IPv6Any : IPAddress.Parse(localIp);
            TcpListener? localServer = new TcpListener(new IPEndPoint(localIpAddress, localPort));
            localServer.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            localServer.Start();

            Console.WriteLine($"TCP proxy started [{localIpAddress}]:{localPort} -> [{remoteServerHostNameOrAddress}]:{remoteServerPort}");

            Task? _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10)).ConfigureAwait(false);

                    List<TcpConnection>? tempConnections = new List<TcpConnection>(connections.Count);
                    while (connections.TryTake(out TcpConnection? connection))
                    {
                        tempConnections.Add(connection);
                    }

                    foreach (TcpConnection? tcpConnection in tempConnections)
                    {
                        if (tcpConnection.LastActivity + ConnectionTimeout < Environment.TickCount64)
                        {
                            tcpConnection.Stop();
                        }
                        else
                        {
                            connections.Add(tcpConnection);
                        }
                    }
                }
            });

            while (true)
            {
                try
                {
                    IPAddress[]? ips = await Dns.GetHostAddressesAsync(remoteServerHostNameOrAddress).ConfigureAwait(false);

                    TcpConnection? tcpConnection = await TcpConnection.AcceptTcpClientAsync(localServer,
                            new IPEndPoint(ips[0], remoteServerPort))
                        .ConfigureAwait(false);
                    tcpConnection.Run();
                    connections.Add(tcpConnection);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex);
                    Console.ResetColor();
                }
            }
        }
    }

    internal class TcpConnection
    {
        private readonly TcpClient _localServerConnection;
        private readonly EndPoint? _sourceEndpoint;
        private readonly IPEndPoint _remoteEndpoint;
        private readonly TcpClient _forwardClient;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly EndPoint? _serverLocalEndpoint;
        private EndPoint? _forwardLocalEndpoint;
        private long _totalBytesForwarded;
        private long _totalBytesResponded;
        public long LastActivity { get; private set; } = Environment.TickCount64;


        private static readonly string[] s_ignoreList;

        private static List<string> ignoreList_new;


        static TcpConnection()
        {
            s_ignoreList = new string[] {
                 "SET DateStyle=ISO"
                ,"SET client_min_messages=notice"
                ,"SET bytea_output=escape"
                ,"SELECT oid, pg_encoding_to_char(encoding) AS encoding, datlastsysoid"
                ,"set client_encoding to 'UNICODE'"
                 // Show results in pgadmin3 
                ,"as typname FROM pg_type"
                ,"SELECT defaclacl FROM pg_catalog.pg_default_acl dacl WHERE dacl.defaclnamespace"
                ,"SELECT proname, pronargs, proargtypes[0] AS arg0, proargtypes[1] AS arg1, proargtypes[2] AS arg2"
                ,"SELECT count(*) FROM pg_attribute WHERE attrelid = 'pg_catalog.pg_proc'::regclass AND attname = 'proargdefaults'"
                ,"FROM 'autovacuum_"
                ,"CASE WHEN typbasetype=0 THEN oid else typbasetype END AS basetype"
            };

            ignoreList_new = new List<string>() {
                 "SET DateStyle=ISO"
                ,"SET client_min_messages=notice"
                ,"SET bytea_output=escape"
                ,"SELECT oid, pg_encoding_to_char(encoding) AS encoding, datlastsysoid"
                ,"set client_encoding to 'UNICODE'"
                 // Show results in pgadmin3 
                ,"as typname FROM pg_type"
                ,"SELECT defaclacl FROM pg_catalog.pg_default_acl dacl WHERE dacl.defaclnamespace"
                ,"SELECT proname, pronargs, proargtypes[0] AS arg0, proargtypes[1] AS arg1, proargtypes[2] AS arg2"
                ,"SELECT count(*) FROM pg_attribute WHERE attrelid = 'pg_catalog.pg_proc'::regclass AND attname = 'proargdefaults'"
                ,"FROM 'autovacuum_"
                ,"CASE WHEN typbasetype=0 THEN oid else typbasetype END AS basetype"
                ,"SELECT NOW()"
                ,"SELECT VERSION()"
                ,"SET statement_timeout TO 30000"
                ,"SELECT EXTRACT(EPOCH FROM CURRENT_TIMESTAMP - pg_postmaster_start_time())::INTEGER"
                ,"SHOW ssl"
                ,"SELECT table_name FROM information_schema.tables WHERE table_schema='information_schema'"
                ,"DISCARD ALL"
                ,"BEGIN"
                ,"COMMIT"
            };
        }


        public static bool CanIgnore(string query)
        {
            for (int i = 0; i < s_ignoreList.Length; ++i)
            {
                if (query.IndexOf(s_ignoreList[i]) == -1)
                    return true;
            }

            return false;
        }

        //TODO: Поправить
        public static bool IgnoreQuery(string query)
        {
            bool ignore = ignoreList_new.Contains(query);
            
            if(query.Contains("BEGIN"))
            {
                ignore = true;
            }
            if(query.Contains("COMMIT"))
            {
                ignore = true;
            }

            return ignore;
        }


        public static async Task<TcpConnection> AcceptTcpClientAsync(TcpListener tcpListener, IPEndPoint remoteEndpoint)
        {
            TcpClient? localServerConnection = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
            localServerConnection.NoDelay = true;
            return new TcpConnection(localServerConnection, remoteEndpoint);
        }

        private TcpConnection(TcpClient localServerConnection, IPEndPoint remoteEndpoint)
        {
            _localServerConnection = localServerConnection;
            _remoteEndpoint = remoteEndpoint;

            _forwardClient = new TcpClient { NoDelay = true };

            _sourceEndpoint = _localServerConnection.Client.RemoteEndPoint;
            _serverLocalEndpoint = _localServerConnection.Client.LocalEndPoint;
        }

        public void Run()
        {
            RunInternal(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occurred while closing TcpConnection : {ex}");
            }
        }

        private void RunInternal(CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (_localServerConnection)
                    using (_forwardClient)
                    {
                        await _forwardClient.ConnectAsync(_remoteEndpoint.Address, _remoteEndpoint.Port, cancellationToken).ConfigureAwait(false);
                        _forwardLocalEndpoint = _forwardClient.Client.LocalEndPoint;

                        Console.WriteLine($"Established TCP {_sourceEndpoint} => {_serverLocalEndpoint} => {_forwardLocalEndpoint} => {_remoteEndpoint}");

                        using (NetworkStream? serverStream = _forwardClient.GetStream())
                        using (NetworkStream? clientStream = _localServerConnection.GetStream())
                        using (cancellationToken.Register(() =>
                        {
                            serverStream.Close();
                            clientStream.Close();
                        }, true))
                        {
                            await Task.WhenAny(
                                CopyToAsync(clientStream, serverStream, true, 81920, Direction.Forward, cancellationToken),
                                CopyToAsync(serverStream, clientStream, false, 81920, Direction.Responding, cancellationToken)
                            ).ConfigureAwait(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An exception occurred during TCP stream : {ex}");
                }
                finally
                {
                    Console.WriteLine($"Closed TCP {_sourceEndpoint} => {_serverLocalEndpoint} => {_forwardLocalEndpoint} => {_remoteEndpoint}. {_totalBytesForwarded} bytes forwarded, {_totalBytesResponded} bytes responded.");
                }
            });
        }


        protected ExpressProfiler.YukonLexer m_Lex = new ExpressProfiler.YukonLexer();

        private static bool s_isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
                  System.Runtime.InteropServices.OSPlatform.Windows
              );

        private static System.Collections.Generic.Dictionary<char, string> s_frontendMessageDictionary = new System.Collections.Generic.Dictionary<char, string>()
        {
            { 'D', "Describe" },
            { 'S', "Sync" },
            { 'E', "Execute" },
            { 'P', "Parse" },
            { 'B', "Bind" },
            { 'C', "Close" },
            { 'Q', "Query" },
            { 'd', "CopyData" },
            { 'c', "CopyDone" },
            { 'f', "CopyFail" },
            { 'X', "Terminate" },
            { 'p', "Password" }
        };


        private async Task CopyToAsync(Stream source, Stream destination, bool log, int bufferSize = 181920, Direction direction = Direction.Unknown, CancellationToken cancellationToken = default)
        {
            
            
            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {

                while (true)
                {
                    int bytesRead = await source.ReadAsync(new Memory<byte>(buffer), cancellationToken).ConfigureAwait(false);
                    if (bytesRead == 0)
                        break;

                    if (log)
                    {
                        Netproxy.NpgsqlReadBuffer foo = new Netproxy.NpgsqlReadBuffer(buffer, bytesRead);

                        byte messageCode = foo.ReadByte();

                        
                        char messageCodeChar = (char)messageCode;

                        string eventCaption = "Unknown";
                        if (s_frontendMessageDictionary.ContainsKey(messageCodeChar))
                            eventCaption = s_frontendMessageDictionary[messageCodeChar];


                        if (messageCode == Netproxy.FrontendMessageCode.Query)
                        {
                            // WriteBuffer.WriteByte(FrontendMessageCode.Query);
                            // WriteBuffer.WriteInt32(
                            // sizeof(int)  +        // Message length (including self excluding code)
                            // queryByteLen +        // Query byte length
                            // sizeof(byte));        // Null terminator
                            // )

                            // await WriteBuffer.WriteString(sql, queryByteLen, async, cancellationToken);
                            // WriteBuffer.WriteByte(0);  // Null terminator

                            int stringLength = foo.ReadInt32();
                            stringLength -= sizeof(int);
                            stringLength -= sizeof(byte);
                            string query = foo.ReadString(stringLength);

                            if (!IgnoreQuery(query))
                            {
                                //System.Console.ResetColor();
                                //System.Console.Write(eventCaption + ":");
                                //System.Console.Write(new string(' ', System.Console.BufferWidth - System.Console.CursorLeft));
                                //
                                //if (!s_isWindows)
                                //    System.Console.Write(System.Environment.NewLine);
                                //
                                //ExpressProfiler.ConsoleOutputWriter cw = new ExpressProfiler.ConsoleOutputWriter()
                                //{
                                //    BackColor = System.Drawing.Color.White
                                //};
                                //
                                //if (!string.IsNullOrEmpty(query))
                                //{
                                //    // var lex = new YukonLexer(); lex.SyntaxHighlight(cw, td);
                                //    this.m_Lex.SyntaxHighlight(cw, query);
                                //    // rich.Rtf = this.m_Lex.SyntaxHighlight(rb, td);
                                //}



                                Queries.GetInstance.AddQuery(query);

                                //System.Console.WriteLine(query);
                            }

                        }
                        else if (messageCode == Netproxy.FrontendMessageCode.Terminate)
                        {
                            //System.Console.ResetColor();
                            //System.Console.Write(eventCaption + ":");
                            //System.Console.Write(new string(' ', System.Console.BufferWidth - System.Console.CursorLeft));

                            //if (!s_isWindows)
                            //    System.Console.Write(System.Environment.NewLine);
                        }
                        else
                        {
                            if (messageCode != '\0')
                            {
                                System.Console.WriteLine("Unhandled messageCode: '" + messageCodeChar.ToString(System.Globalization.CultureInfo.InvariantCulture).Replace("\0", "NULL") + "'.");
                            }
                        }


                        /*
                        string? message = System.Text.Encoding.UTF8.GetString(buffer);
                        message = message.Replace("��4", "");
                        string[] statements = message.Split('\0', System.StringSplitOptions.RemoveEmptyEntries);

             
                        } // End if (statements.Length > 1 && "Q".Equals(statements[0])) 

                        // message = message.Replace("\0", "!ARGH!");
                        // System.Console.WriteLine(foo);
                        */
                    } // End if (log) 

                    LastActivity = Environment.TickCount64;
                    await destination.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, bytesRead), cancellationToken).ConfigureAwait(false);

                    switch (direction)
                    {
                        case Direction.Forward:
                            Interlocked.Add(ref _totalBytesForwarded, bytesRead);
                            break;
                        case Direction.Responding:
                            Interlocked.Add(ref _totalBytesResponded, bytesRead);
                            break;
                    }
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }

    internal enum Direction
    {
        Unknown = 0,
        Forward,
        Responding,
    }
}
