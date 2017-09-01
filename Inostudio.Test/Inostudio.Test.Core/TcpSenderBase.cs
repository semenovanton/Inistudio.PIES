using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Provides the base functionaltiy for sending data to TCP connection in the separate thread.
    /// </summary>
    public abstract class TcpSenderBase 
        : TcpHandlerBase, ITcpSender
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpSenderBase"/> class.
        /// </summary>
        /// <param name="buffer">Buffer, from which sender can take values for sending into connection</param>
        /// <param name="threadNumber">The number of this thread.</param>
        public TcpSenderBase(IBuffer<long> buffer, int threadNumber)
            : base(buffer, threadNumber)
        {
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Main cycle of handling TCP client on SEND mode. Should be executed in the separate thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task in the separate thread.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var stream = TcpClient.GetStream();

            //we can send values only from specified buffer
            if (Buffer == null)
            {
                throw new Exception("Buffer should be specified");
            }

            while (!cancellationToken.IsCancellationRequested && CheckConnection())
            {
                if (Buffer.TryPull(out long number))
                {
                    ///handle value before sending (primary for pushing into the output file)
                    BeforeSend(number);

                    var bytes = BitConverter.GetBytes(number);

                    await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                    await stream.FlushAsync();
                }
                else
                {
                    //if buffer is empty now we sleep for a while
                    Thread.Sleep(50);
                }
                
            }

            OnEndOfWork();
        }

        /// <summary>
        /// Handles value from buffer before it would be sent into TCP connection.
        /// </summary>
        /// <param name="number">The number value.</param>
        protected virtual void BeforeSend(long number)
        {
#if DEBUG
            //Console.WriteLine($"sending: {number} by {ThreadNumber}");
#endif
        }
        #endregion
    }
}
