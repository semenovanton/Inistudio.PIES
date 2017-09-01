using Inostudio.Test.Core;
using System.Net.Sockets;

namespace Inostudio.Test.ProxyServer
{
    /// <summary>
    /// TCP sender implemenattion for the PROXY server.
    /// </summary>
    class ProxyTcpSender 
        : TcpSenderBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyTcpSender"/>.
        /// </summary>
        /// <param name="buffer">The buffer, from which receiver will take numbers to send them.</param>
        public ProxyTcpSender(IBuffer<long> buffer)
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
