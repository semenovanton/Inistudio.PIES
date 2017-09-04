using System.Net.Sockets;

using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.ProxyServer
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
        /// <para>Use when we can't say onj creation, which buffers we can use.</para>
        /// </summary>
        public ProxyTcpReceiver()
            : base(null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyTcpReceiver"/>
        /// </summary>
        /// <param name="buffer">Buffer, from which sender can take values for sending into connection</param>
        /// <param name="outputFileBuffer">The buffer, into which we put sending values to be written into the output file.</param>
        public ProxyTcpReceiver(
            IBuffer<long> buffer,
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion
    }
}
