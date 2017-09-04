using System.Net.Sockets;

using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.ProxyServer
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
        /// <para>Use when we can't say onj creation, which buffers we can use.</para>
        /// </summary>
        public ProxyTcpSender()
            : base(null, null)
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyTcpSender"/>.
        /// </summary>
        /// <param name="buffer">Buffer, from which sender can take values for sending into connection</param>
        /// <param name="outputFileBuffer">The buffer, into which we put sending values to be written into the output file.</param>
        public ProxyTcpSender(IBuffer<long> buffer,
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion
    }
}
