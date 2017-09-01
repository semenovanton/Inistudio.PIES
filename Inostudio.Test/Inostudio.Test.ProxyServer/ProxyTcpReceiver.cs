using System.Threading;
using System.Threading.Tasks;
using Inostudio.Test.Core;
using System;
using System.Net.Sockets;

namespace Inostudio.Test.ProxyServer
{
    /// <summary>
    /// TCP receiver implementatio for the proxy server. 
    /// </summary>
    class ProxyTcpReceiver 
        : TcpReceiverBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyTcpReceiver"/>
        /// </summary>
        /// <param name="buffer">The buffer, to which receiver should place received numbers.</param>
        public ProxyTcpReceiver(IBuffer<long> buffer)
            : base(buffer, 0) //we don't care which number of this thread is
        {
        }
        #endregion

        #region protected methods
        protected override bool CheckConnection()
        {
            if (TcpClient.Client.Poll(0, SelectMode.SelectRead))
            {
                byte[] checkConn = new byte[1];
                if (TcpClient.Client.Receive(checkConn, SocketFlags.Peek) == 0)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }
}
