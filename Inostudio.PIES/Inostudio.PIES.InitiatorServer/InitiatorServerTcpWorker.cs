using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.InitiatorServer
{
    /// <summary>
    /// Server TCP worker implementation for the INITIATOR server. 
    /// </summary>
    public class InitiatorServerTcpWorker 
        : ServerTcpWorkerBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitiatorServerTcpWorker"/>
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="proxyPort">The port of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="receiver">TCP handler for receiving.</param>
        /// /// <param name="sender">TCP handler for sending.</param>
        public InitiatorServerTcpWorker(
            string proxyHostName, 
            int proxyPort,
            ITcpReceiver receiver,
            ITcpSender sender)
            : base(proxyHostName, proxyPort, receiver, sender)
        {
        }
        #endregion


        #region public properties
        public override ServerType ServerType
        {
            get
            {
                return ServerType.Initiator;
            }
            set
            {
                throw new System.Exception("Forbidden action");
            }
        }
        #endregion
    }
}
