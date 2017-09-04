using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core.Implementations
{
    /// <summary>
    /// Provides the base functionaltiy for receiving data from TCP connection in the separate thread.
    /// </summary>
    public abstract class TcpReceiverBase 
        : TcpHandlerBase, ITcpReceiver
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpReceiverBase"/>.
        /// </summary>
        /// <param name="buffer">The buffer, to which receiver will push received values.</param>
        /// <param name="outputFileBuffer">The buffer, into which we put received values to be written into the output file.</param>
        public TcpReceiverBase(
            IBuffer<long> buffer, 
            IOutputFileBuffer<long> outputFileBuffer)
            : base(buffer, outputFileBuffer)
        {
        }
        #endregion


        #region protected methods
        /// <summary>
        /// Main cycle of handling TCP client on RECEIVE mode. Should be executed in the separate thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task in the separate thread.</returns>
        protected override async Task ExecuteBackgroundAsync(CancellationToken cancellationToken)
        {
            _ended = false;

            using (var reader = new System.IO.BinaryReader(TcpClient.GetStream(), System.Text.Encoding.UTF8, true))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var number = reader.ReadInt64();
                        AfterReceive(number);
                    }
                    catch (Exception)
                    {
                        //connection was lost
                    }

                    //handle value after receiving (primary for pushing into the output file and over the data flow pipeline)
                    
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
            //if ouput file buffer is specifed we push this number into it.
            if (OutputFileBuffer != null)
            {
                OutputFileBuffer.Push(number);
            }

            //if buffer is specified we push received number into it to pass the number over the pipeline
            if (Buffer != null)
            {
                Buffer.Push(number);
            }
        }
        #endregion
    }
}
