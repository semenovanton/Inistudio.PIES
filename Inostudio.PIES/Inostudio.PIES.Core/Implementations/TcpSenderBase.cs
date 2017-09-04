using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core.Implementations
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
        /// <param name="outputFileBuffer">The buffer, into which we put sending values to be written into the output file.</param>
        public TcpSenderBase(
            IBuffer<long> buffer, 
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion

        #region protected methods
        /// <summary>
        /// Main cycle of handling TCP client on SEND mode. Should be executed in the separate thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task in the separate thread.</returns>
        protected override async Task ExecuteBackgroundAsync(CancellationToken cancellationToken)
        {
            //we can send values only from specified buffer
            if (Buffer == null)
            {
                throw new Exception("Buffer should be specified");
            }
            _ended = false;

            using (var writer = new System.IO.BinaryWriter(TcpClient.GetStream(), System.Text.Encoding.UTF8, true))
            {

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (Buffer.TryPull(out long number))
                    {
                        //handle value before sending (primary for pushing into the output file)
                        BeforeSend(number);

                        try
                        {
                            writer.Write(number);
                            writer.Flush();
                        }
                        catch (Exception)
                        {
                            //connection was lost
                        }
                    }
                    else
                    {
                        //if buffer is empty now we sleep for a while
                        Thread.Sleep(50);
                    }

                }
            }

            //Raise event about end of work
            OnEndOfWork();
        }

        /// <summary>
        /// Handles value from buffer before it would be sent into TCP connection.
        /// </summary>
        /// <param name="number">The number value.</param>
        protected virtual void BeforeSend(long number)
        {
            if (OutputFileBuffer != null)
            {
                OutputFileBuffer.Push(number);
            }
        }
        #endregion
    }
}
