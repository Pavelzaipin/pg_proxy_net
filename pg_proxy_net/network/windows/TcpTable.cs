
namespace pg_proxy_net.network.windows
{


    public class TcpTable 
        : System.Collections.Generic.IEnumerable<TcpRow>
    {
        #region Private Fields

        private System.Collections.Generic.IEnumerable<TcpRow> tcpRows;

        #endregion

        #region Constructors

        public TcpTable(System.Collections.Generic.IEnumerable<TcpRow> tcpRows)
        {
            this.tcpRows = tcpRows;
        }

        #endregion

        #region Public Properties

        public System.Collections.Generic.IEnumerable<TcpRow> Rows
        {
            get { return this.tcpRows; }
        }

        #endregion

        #region IEnumerable<TcpRow> Members

        public System.Collections.Generic.IEnumerator<TcpRow> GetEnumerator()
        {
            return this.tcpRows.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.Generic.IEnumerator<TcpRow> System.Collections.Generic.IEnumerable<TcpRow>.GetEnumerator()
        {
            return this.tcpRows.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.tcpRows.GetEnumerator();
        }

        #endregion
    }
}
