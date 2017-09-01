using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Provides the base functionaltiy for receiving data from TCP connection in the separate thread.
    /// </summary>
    public abstract class TcpReceiverBase : TcpHandlerBase, ITcpReceiver
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpReceiverBase"/>.
        /// </summary>
        /// <param name="buffer">The buffer, to which receiver will push received values.</param>
        /// <param name="threadNumber">The thread number.</param>
        public TcpReceiverBase(IBuffer<long> buffer, int threadNumber)
            : base(buffer, threadNumber)
        {
        }
        #endregion


        #region protected methods
        /// <summary>
        /// Main cycle of handling TCP client on RECEIVE mode. Should be executed in the separate thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task in the separate thread.</returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var stream = TcpClient.GetStream();

            while (!cancellationToken.IsCancellationRequested && CheckConnection())
            {
                if (TcpClient.Available >= 8)
                {
                    //if we have 8 bytes in the TCP connection buffer we can read them as sended value...
                    var bytes = new byte[8];

                    await stream.ReadAsync(bytes, 0, bytes.Length);
                    var number = BitConverter.ToInt64(bytes, 0);

                    AfterReceive(number);

                    if (Buffer != null)
                    {
                        Buffer.Push(number);
                    }
                }
                else
                {
                    //...if not, we should sleep
                    Thread.Sleep(50);
                }
            }

            OnEndOfWork();
        }

        /// <summary>
        /// Handles the received value.
        /// </summary>
        /// <param name="number">The received value.</param>
        protected virtual void AfterReceive(long number)
        {
#if DEBUG
            //Console.WriteLine($"received: {number} by {ThreadNumber}");
#endif
        }
        #endregion
    }
}
