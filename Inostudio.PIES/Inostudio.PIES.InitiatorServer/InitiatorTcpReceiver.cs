using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.InitiatorServer
{
    /// <summary>
    /// TCP receiver for the INITIATOR server.
    /// </summary>
    public class InitiatorTcpReceiver 
        : TcpReceiverBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitiatorTcpReceiver"/>.
        /// </summary>
        /// <param name="buffer">The buffer, to which receiver will push received values.</param>
        /// <param name="outputFileBuffer">The buffer, into which we put received values to be written into the output file.</param>
        public InitiatorTcpReceiver(
            IBuffer<long> buffer,
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion
    }
}
