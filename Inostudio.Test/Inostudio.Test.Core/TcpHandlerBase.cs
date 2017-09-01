using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.Test.Core
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
        /// <param name="threadNumber"></param>
        public TcpHandlerBase(IBuffer<long> buffer, int threadNumber)
        {
            Buffer = buffer;
            ThreadNumber = threadNumber;
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
        /// The number of the current thread
        /// <para>Can be used for service needs.</para>
        /// </summary>
        public int ThreadNumber { get; set; }

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
            _endOfWork?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Checks if connection is alive
        /// </summary>
        /// <returns></returns>
        protected virtual bool CheckConnection()
        {
            return TcpClient.Connected;
        }
        #endregion


        #region private fields
        /// <summary>
        /// raises when handling is over.
        /// </summary>
        private EventHandler _endOfWork;
        #endregion
    }
}
