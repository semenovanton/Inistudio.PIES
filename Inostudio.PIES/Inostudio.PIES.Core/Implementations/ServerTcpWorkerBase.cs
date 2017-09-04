using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core.Implementations
{
    /// <summary>
    /// Incapsulates one opened TCP connection, receiver and sender handlers working in separate threads.
    /// </summary>
    /// <typeparam name="TTcpReceiver"></typeparam>
    /// <typeparam name="TTcpSender"></typeparam>
    public abstract class ServerTcpWorkerBase
        : BackgroundWorkerBase, IServerTcpWorker
    {
        #region public constructors
        /// <summary>
        /// Initializes a nes instance of the <see cref="ServerWorkerBase"/>
        /// <para>Use for creation a TCP clint inside of the worker.</para>
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <param name="port">The port number.</param>
        /// <param name="receiver">TCP handler for receiving.</param>
        /// /// <param name="sender">TCP handler for sending.</param>
        public ServerTcpWorkerBase(
            string hostName, 
            int port, 
            ITcpReceiver receiver, 
            ITcpSender sender) 
            : this(receiver, sender)
        {
            HostName = hostName;
            Port = port;
        }

        /// <summary>
        /// Initializes a nes instance of the <see cref="ServerWorkerBase"/>
        /// <para>Use, when TCP client was created outside.</para>
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        public ServerTcpWorkerBase(TcpClient tcpClient, ITcpReceiver receiver, ITcpSender sender)
            : this(receiver, sender)
        {
            TcpClient = tcpClient;
        }

        protected ServerTcpWorkerBase(ITcpReceiver receiver, ITcpSender sender)
        {
            Receiver = receiver;
            Sender = sender;
        }
        #endregion


        #region public properties
        /// <summary>
        /// Gets or sets the TCP client, which wrap TCP connection.
        /// </summary>
        public TcpClient TcpClient { get; set; }
        /// <summary>
        /// Gets the TCP handler, that receives data from the TCP connection.
        /// </summary>
        public ITcpReceiver Receiver { get; protected set; }
        /// <summary>
        /// Gets the TCP handler, that sends data from the TCP connection.
        /// </summary>
        public ITcpSender Sender { get; protected set; }
        /// <summary>
        /// Gets the type of the server.
        /// <para>Can be used for service needs.</para>
        /// </summary>
        public virtual ServerType ServerType { get; set; }

        /// <summary>
        /// Gets or sets the host name.
        /// </summary>
        public string HostName { get; protected set; }
        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        public int Port { get; protected set; }

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


        #region public methods
        /// <summary>
        /// Stops the server worker.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            //stops the TCP connection
            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }
        }
        /// <summary>
        /// Sends the server type over the TCP connection.
        /// <para>Use when this worker initializes the connection and its server type is known.</para>
        /// </summary>
        public virtual void SendServerType()
        {
            using (var writer = new System.IO.BinaryWriter(TcpClient.GetStream(), System.Text.Encoding.UTF8, true))
            {
                writer.Write((byte)ServerType);
            }
        }

        /// <summary>
        /// Sends the server type over the TCP connection.
        /// <para>Use when this worker handle the connection with another server and its type is unknown.</para>
        /// </summary>
        public virtual void ReceiveServerType()
        {
            using (var reader = new System.IO.BinaryReader(TcpClient.GetStream(), System.Text.Encoding.UTF8, true))
            {
                ServerType = (ServerType)reader.ReadByte();
            }
        }
        #endregion


        #region protected methods
        protected override Task ExecuteBackgroundAsync(CancellationToken cancellationToken)
        {
            _ended = false;

            //create a new TCP connection based on passed HOSTNAME and PORT
            TcpClient = new TcpClient(HostName, Port);

            if (!TcpClient.Connected)
            {
                TcpClient.Connect(HostName, Port);
            }

            //send server type to identify on other side what type of server current instance is
            SendServerType();

            //set this tcp client for receiver and sender.
            Receiver.TcpClient = TcpClient;
            Sender.TcpClient = TcpClient;

            //sign on EndOfWork TCP handler events
            Receiver.EndOfWork += OnTcpHandlerEndHandler;
            Sender.EndOfWork += OnTcpHandlerEndHandler;

            //start receiver and sender handlers
            var receiverTask = Receiver.StartAsync(cancellationToken);
            var senderTAsk = Sender.StartAsync(cancellationToken);

            return Task.WhenAll(receiverTask, senderTAsk);
        }

        /// <summary>
        /// Raices when all work is over.
        /// </summary>
        protected virtual void OnEndOfWork()
        {
            if (TcpClient != null)
            {
                TcpClient.Close();
                TcpClient = null;
            }
            _endOfWork?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Handles EndOfWork event of TCP handlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnTcpHandlerEndHandler(object sender, EventArgs e)
        {
            if (!_ended)
            {
                OnEndOfWork();
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
