using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.EchoServer
{
    /// <summary>
    /// Server TCP worker implementation for the ECHO server. 
    /// </summary>
    class EchoServerTcpWorker 
        : ServerTcpWorkerBase 
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoServerTcpWorker"/>
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="proxyPort">The port of the proxy server, to which ECHO server should be connected.</param>
        public EchoServerTcpWorker(
            string proxyHostName, 
            int proxyPort, 
            ITcpReceiver receiver, ITcpSender sender) 
            : base (proxyHostName, proxyPort, receiver, sender)
        {
        }
        #endregion

        #region public properties
        public override ServerType ServerType
        {
            get
            {
                return ServerType.Echo;
            }
            set
            {
                throw new System.Exception("Forbidden action");
            }
        }
        #endregion
    }
}
