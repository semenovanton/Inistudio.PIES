using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.EchoServer
{
    /// <summary>
    /// TCP receiver implementation for the ECHO server.
    /// </summary>
    class EchoTcpReceiver 
        : TcpReceiverBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EchoTcpReceiver"/>.
        /// </summary>
        /// <param name="buffer">The buffer, to which receiver will push received values.</param>
        /// <param name="outputFileBuffer">The buffer, into which we push received values to be written into the output file.</param>
        public EchoTcpReceiver(
            IBuffer<long> buffer, 
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion
    }
}
