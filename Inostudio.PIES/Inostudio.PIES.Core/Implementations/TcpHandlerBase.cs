using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core.Implementations
{
    /// <summary>
    /// Provides the base functionality for threads, which will handle TCP connection.
    /// </summary>
    public abstract class TcpHandlerBase
        : BackgroundWorkerBase, ITcpHandler 
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TcpHandlerBase"/>.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="outputFileBuffer"></param>
        public TcpHandlerBase(
            IBuffer<long> buffer, 
            IOutputFileBuffer<long> outputFileBuffer)
        {
            Buffer = buffer;
            OutputFileBuffer = outputFileBuffer;
        }
        #endregion


        #region public properties
        /// <summary>
        /// Gets or sets the TCP client, which wraps the handling TCP connection.
        /// </summary>
        public TcpClient TcpClient { get; set; }
        /// <summary>
        /// Buffer, which can be used in different scenarious, depend on which type of handler current handler is. 
        /// </summary>
        public IBuffer<long> Buffer { get; set; }
        /// <summary>
        /// Buffer, into which we put values to be written into the output file. 
        /// </summary>
        public IOutputFileBuffer<long> OutputFileBuffer { get; set; }

        /// <summary>
        /// Raises when handling is over.
        /// </summary>
        public event EventHandler EndOfWork
        {
            add
            {
                _endOfWork += value;
            }
            remove
            {
                _endOfWork -= value;
            }
        }
        #endregion


        #region protected method
        /// <summary>
        /// Fire when handling is over.
        /// </summary>
        protected virtual void OnEndOfWork()
        {
            if (!_ended)
            {
                _endOfWork?.Invoke(this, new EventArgs());
            }
            _ended = true;
        }
        #endregion


        #region private fields
        /// <summary>
        /// raises when handling is over.
        /// </summary>
        private EventHandler _endOfWork;
        protected bool _ended;
        #endregion
    }
}
