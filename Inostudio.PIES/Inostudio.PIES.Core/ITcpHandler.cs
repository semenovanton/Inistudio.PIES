using System;
using System.Net.Sockets;

namespace Inostudio.PIES.Core
{
    /// <summary>
    /// Provides the base functionality for threads, which will handle TCP connection.
    /// </summary>
    public interface ITcpHandler 
        : IBackgroundWorker
    {
        /// <summary>
        /// Gets or sets the TCP client, which wraps the handling TCP connection.
        /// </summary>
        TcpClient TcpClient { get; set; }
        /// <summary>
        /// Buffer, which can be used in different scenarious, depend on which type of handler current handler is. 
        /// </summary>
        IBuffer<long> Buffer { get; set; }
        /// <summary>
        /// Raises when work of the handler is over.
        /// </summary>
        event EventHandler EndOfWork;
    }
}
