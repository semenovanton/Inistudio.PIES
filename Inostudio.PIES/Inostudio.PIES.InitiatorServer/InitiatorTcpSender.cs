using System;
using System.Threading;

using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;
using System.Threading.Tasks;

namespace Inostudio.PIES.InitiatorServer
{
    /// <summary>
    /// TCP sender implementation for the INITIATOR server.
    /// </summary>
    public class InitiatorTcpSender
        : TcpSenderBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitiatorTcpSender"/>.
        /// </summary>
        /// <param name="buffer">The buffer, from which sender will take numbers to be sent.</param>
        /// <param name="outputFileBuffer">The buffer, into which we put sending values to be written into the output file.</param>
        public InitiatorTcpSender(
            IBuffer<long> buffer,
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion


        #region protected methods
        /// <summary>
        /// Handles value from buffer before it would be sent into TCP connection.
        /// </summary>
        /// <param name="number">The number value.</param>
        protected override void BeforeSend(long number)
        {
            //this sleep is used for delay number generation 
            //TODO: maybe move to another method, maybe to InitiatorSequenceBuffer
            Thread.Sleep(100);
            base.BeforeSend(number);
        }
        #endregion
    }
}
