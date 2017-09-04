using System;
using System.Net.Sockets;

namespace Inostudio.PIES.Core
{
    /// <summary>
    /// Incapsulates one opened TCP connection, receiver and sender handlers working in separate threads.
    /// </summary>
    public interface IServerTcpWorker
        : IBackgroundWorker
    {
        /// <summary>
        /// Gets the TCP client, which wrap TCP connection.
        /// </summary>
        TcpClient TcpClient { get; set; }
        /// <summary>
        /// Gets the TCP handler, that receives data from the TCP connection.
        /// </summary>
        ITcpReceiver Receiver { get; }
        /// <summary>
        /// Gets the TCP handler, that sends data from the TCP connection.
        /// </summary>
        ITcpSender Sender { get; }
        /// <summary>
        /// Gets the type of the server.
        /// <para>Can be used for service needs.</para>
        /// </summary>
        ServerType ServerType { get; }
        event EventHandler EndOfWork;
        /// <summary>
        /// Sends the server type over the TCP connection.
        /// <para>Use when this worker initializes the connection and its server type is known.</para>
        /// </summary>
        void SendServerType();
        /// <summary>
        /// Sends the server type over the TCP connection.
        /// <para>Use when this worker handle the connection with another server and its type is unknown.</para>
        /// </summary>
        void ReceiveServerType();
    }
}
