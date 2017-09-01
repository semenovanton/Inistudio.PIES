using Inostudio.Test.Core;

namespace Inostudio.Test.EchoServer
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
        /// <param name="threadNumber">The number of this thread.</param>
        public EchoTcpSender(
            IBuffer<long> buffer, 
            int threadNumber = 0)
            : base(buffer, threadNumber)
        {
        }
        #endregion
    }
}
