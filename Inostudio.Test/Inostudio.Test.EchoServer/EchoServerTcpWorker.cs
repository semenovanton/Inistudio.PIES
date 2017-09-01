using Inostudio.Test.Core;
using System;

namespace Inostudio.Test.EchoServer
{
    /// <summary>
    /// Server TCP worker implementation for the ECHO server. 
    /// </summary>
    class EchoServerTcpWorker 
        : ServerTcpWorkerBase<EchoTcpReceiver, EchoTcpSender>
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoServerTcpWorker"/>
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="proxyPort">The port of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="buffer">The buffer, to which receiver will push received from proxy numbers and from which sender will take these numbers to send them back.</param>
        /// <param name="echoOutBuffer">The buffer into which we should push receivede numbers to be written into the output file.</param>
        /// <param name="threadsCount">The number of threads, which should be created in the ECHO server.</param>
        public EchoServerTcpWorker(
            string proxyHostName, 
            int proxyPort, 
            IBuffer<long> buffer, 
            IFlushingBuffer<long> echoOutBuffer, 
            int threadNumber) 
            : base (proxyHostName, proxyPort)
        {
            Receiver = new EchoTcpReceiver(buffer, echoOutBuffer, threadNumber);
            Sender = new EchoTcpSender(buffer, threadNumber);
        }
        #endregion


        #region public properties
        public override ServerType ServerType => ServerType.Echo;
        #endregion

        private IFlushingBuffer<long> _echoOutBuffer;
    }
}
