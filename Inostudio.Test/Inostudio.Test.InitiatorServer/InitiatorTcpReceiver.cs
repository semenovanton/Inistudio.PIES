using Inostudio.Test.Core;

namespace Inostudio.Test.InitiatorServer
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
        /// <param name="receiverOutBuffer"></param>
        /// <param name="threadNumber"></param>
        public InitiatorTcpReceiver(
            IFlushingBuffer<long> receiverOutBuffer, 
            int threadNumber = 0)
            : base(null, threadNumber) //this receiver wan't use receiver buffer at all, only flushing buffer.
        {
            _receiverOutBuffer = receiverOutBuffer;
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Handles the received value.
        /// </summary>
        /// <param name="number">The received value.</param>
        protected override void AfterReceive(long number)
        {
            base.AfterReceive(number);

            _receiverOutBuffer.Push(number);
        }
        #endregion

        #region private fields
        /// <summary>
        /// this buffer is used to write received from ECHO server numbers into file.
        /// </summary>
        private IFlushingBuffer<long> _receiverOutBuffer;
        #endregion
    }
}
