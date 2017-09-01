using Inostudio.Test.Core;

namespace Inostudio.Test.EchoServer
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
        /// /// <param name="echoOutBuffer">The buffer, into which we push received values to be written into the output file.</param>
        /// <param name="threadNumber">The thread number.</param>
        public EchoTcpReceiver(
            IBuffer<long> buffer, 
            IFlushingBuffer<long> echoOutBuffer
            , int threadNumber = 0)
            : base(buffer, threadNumber)
        {
            _echoOutBuffer = echoOutBuffer;
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

            _echoOutBuffer.Push(number);
        }
        #endregion


        #region private fields
        /// <summary>
        /// The buffer, into which we push received values to be written into the output file.
        /// </summary>
        private IFlushingBuffer<long> _echoOutBuffer;
        #endregion
    }
}
