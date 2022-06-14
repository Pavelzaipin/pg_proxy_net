
namespace pg_proxy_net.network.windows
{


    public static class ManagedIpHelper
    {
        #region Public Methods

        public static TcpTable GetExtendedTcpTable(bool sorted)
        {
            System.Collections.Generic.List<TcpRow> tcpRows = new System.Collections.Generic.List<TcpRow>();

            System.IntPtr tcpTable = System.IntPtr.Zero;
            int tcpTableLength = 0;

            if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, sorted, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) != 0)
            {
                try
                {
                    tcpTable = System.Runtime.InteropServices.Marshal.AllocHGlobal(tcpTableLength);
                    if (IpHelper.GetExtendedTcpTable(tcpTable, ref tcpTableLength, true, IpHelper.AfInet, IpHelper.TcpTableType.OwnerPidAll, 0) == 0)
                    {
                        IpHelper.TcpTable table = (IpHelper.TcpTable)System.Runtime.InteropServices.Marshal.PtrToStructure(tcpTable, typeof(IpHelper.TcpTable));

                        System.IntPtr rowPtr = (System.IntPtr)((long)tcpTable + System.Runtime.InteropServices.Marshal.SizeOf(table.length));
                        for (int i = 0; i < table.length; ++i)
                        {
                            tcpRows.Add(new TcpRow((IpHelper.TcpRow)System.Runtime.InteropServices.Marshal.PtrToStructure(rowPtr, typeof(IpHelper.TcpRow))));
                            rowPtr = (System.IntPtr)((long)rowPtr + System.Runtime.InteropServices.Marshal.SizeOf(typeof(IpHelper.TcpRow)));
                        }
                    }
                }
                finally
                {
                    if (tcpTable != System.IntPtr.Zero)
                    {
                        System.Runtime.InteropServices.Marshal.FreeHGlobal(tcpTable);
                    }
                }
            }

            return new TcpTable(tcpRows);
        }

        #endregion
    }


}
