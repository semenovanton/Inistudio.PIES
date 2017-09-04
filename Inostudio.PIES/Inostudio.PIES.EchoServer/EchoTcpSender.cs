using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.EchoServer
{
    /// <summary>
    /// TCP sender implementation for ECHO server.
    /// </summary>
    class EchoTcpSender 
        : TcpSenderBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoTcpSender"/>.
        /// </summary>
        /// <param name="buffer">Buffer, from which sender can take values for sending into connection</param>
        /// <param name="outputFileBuffer">The buffer, into which we push received values to be written into the output file.</param>
        public EchoTcpSender(
            IBuffer<long> buffer,
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion
    }
}
