using System;
using System.Net.Sockets;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Incapsulates one opened TCP connection, receiver and sender handlers working in separate threads.
    /// </summary>
    /// <typeparam name="TTcpReceiver"></typeparam>
    /// <typeparam name="TTcpSender"></typeparam>
    public interface IServerTcpWorker<TTcpReceiver, TTcpSender> 
        : IBackgroundWorker
        where TTcpReceiver : ITcpReceiver
        where TTcpSender : ITcpSender
    {
        /// <summary>
        /// Gets the TCP client, which wrap TCP connection.
        /// </summary>
        TcpClient TcpClient { get; }
        /// <summary>
        /// Gets the TCP handler, that receives data from the TCP connection.
        /// </summary>
        TTcpReceiver Receiver { get; }
        /// <summary>
        /// Gets the TCP handler, that sends data from the TCP connection.
        /// </summary>
        TTcpSender Sender { get; }
        /// <summary>
        /// Gets the type of the server.
        /// <para>Can be used for service needs.</para>
        /// </summary>
        ServerType ServerType { get; }
        event EventHandler EndOfWork;
    }
}
