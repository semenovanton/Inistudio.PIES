using Inostudio.Test.Core;

namespace Inostudio.Test.InitiatorServer
{
    /// <summary>
    /// Server TCP worker implementation for the INITIATOR server. 
    /// </summary>
    public class InitiatorServerTcpWorker 
        : ServerTcpWorkerBase<InitiatorTcpReceiver, InitiatorTcpSender>
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitiatorServerTcpWorker"/>
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="proxyPort">The port of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="receiverOutBuffer">The buffer, which used to write numbers into the output file which are receioved from PROXY server.</param>
        /// <param name="senderBuffer">The buffer, from which numbers are got to be sent to the PROXY server.</param>
        /// <param name="senderOutBuffer">The buffer, which used to write numbers into the output file which are sending to PROXY server.</param>
        /// <param name="threadNumber">The number of the current server tcp worker.</param>
        public InitiatorServerTcpWorker(
            string proxyHostName, 
            int proxyPort,
            IFlushingBuffer<long> receiverOutBuffer, 
            IBuffer<long> senderBuffer,
            IFlushingBuffer<long> senderOutBuffer, 
            int threadNumber) 
            : base (proxyHostName, proxyPort)
        {
            Receiver = new InitiatorTcpReceiver(receiverOutBuffer, threadNumber);
            Sender = new InitiatorTcpSender(senderBuffer, senderOutBuffer, threadNumber);
        }
        #endregion

        #region public properties
        public override ServerType ServerType => ServerType.Initiator;
        #endregion
    }
}
