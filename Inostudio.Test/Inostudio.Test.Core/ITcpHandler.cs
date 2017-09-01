using System;
using System.Net.Sockets;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Provides the base functionality for threads, which will handle TCP connection.
    /// </summary>
    public interface ITcpHandler : IBackgroundWorker
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
        /// The number of the current thread
        /// <para>Can be used for service needs.</para>
        /// </summary>
        int ThreadNumber { get; set; }
        event EventHandler EndOfWork;
    }
}
