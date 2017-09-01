using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inostudio.Test.Core;
using System;

namespace Inostudio.Test.InitiatorServer
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
        /// <param name="senderOutBuffer">The buffer, into which we will push sending numbers to be written into the log file.</param>
        /// <param name="threadNumber">The number of the thread</param>
        public InitiatorTcpSender(
            IBuffer<long> buffer, 
            IFlushingBuffer<long> senderOutBuffer, 
            int threadNumber = 0)
            : base(buffer, threadNumber)
        {
            _senderOutBuffer = senderOutBuffer;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Handles value from buffer before it would be sent into TCP connection.
        /// </summary>
        /// <param name="number">The number value.</param>
        protected override void BeforeSend(long number)
        {
            base.BeforeSend(number);
            //this sleep is used for delay number generation 
            //TODO: maybe move to another method, maybe to InitiatorSequenceBuffer
            Thread.Sleep(100);

            _senderOutBuffer.Push(number);
        }
        #endregion

        #region private fields
        /// <summary>
        /// The buffer, into which we will push sending numbers to be written into the log file.
        /// </summary>
        private IFlushingBuffer<long> _senderOutBuffer;
        #endregion
    }
}
