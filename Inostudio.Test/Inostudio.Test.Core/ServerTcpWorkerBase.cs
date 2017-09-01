using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.Test.Core
{
    /// <summary>
    /// Incapsulates one opened TCP connection, receiver and sender handlers working in separate threads.
    /// </summary>
    /// <typeparam name="TTcpReceiver"></typeparam>
    /// <typeparam name="TTcpSender"></typeparam>
    public abstract class ServerTcpWorkerBase<TReceiver, TSender>
        : BackgroundWorkerBase
        where TReceiver : ITcpReceiver
        where TSender : ITcpSender
    {
        #region public constructors
        /// <summary>
        /// Initializes a nes instance of the <see cref="ServerWorkerBase"/>
        /// <para>Use for creation a TCP clint inside of the worker.</para>
        /// </summary>
        /// <param name="hostName">The host name.</param>
        /// <param name="port">The port number.</param>
        public ServerTcpWorkerBase(string hostName, int port)
        {
            HostName = hostName;
            Port = port;
        }

        /// <summary>
        /// Initializes a nes instance of the <see cref="ServerWorkerBase"/>
        /// <para>Use, when TCP client was created outside.</para>
        /// </summary>
        /// <param name="tcpClient">The TCP client.</param>
        public ServerTcpWorkerBase(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets the TCP client, which wrap TCP connection.
        /// </summary>
        public TcpClient TcpClient { get; protected set; }
        /// <summary>
        /// Gets the TCP handler, that receives data from the TCP connection.
        /// </summary>
        public TReceiver Receiver { get; protected set; }
        /// <summary>
        /// Gets the TCP handler, that sends data from the TCP connection.
        /// </summary>
        public TSender Sender { get; protected set; }
        /// <summary>
        /// Gets the type of the server.
        /// <para>Can be used for service needs.</para>
        /// </summary>
        public abstract ServerType ServerType { get; }

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
        #endregion

        #region protected methods
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //create a new TCP connection based on passed HOSTNAME and PORT
            TcpClient = new TcpClient(HostName, Port);

            if (!TcpClient.Connected)
            {
                TcpClient.Connect(HostName, Port);
            }

            var stream = TcpClient.GetStream();

            //get the server type and send to this connection to identify, which type of server it is (spoiler: Initiator or Proxy in our project).
            var initMessage = BitConverter.GetBytes((long)ServerType);

            stream.Write(initMessage, 0, initMessage.Length);

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

        protected virtual void OnEndOfWork()
        {
            TcpClient.Close();
            TcpClient = null;
            _endOfWork?.Invoke(this, new EventArgs());
        }

        protected virtual void OnTcpHandlerEndHandler(object sender, EventArgs e)
        {
            if (sender.Equals(Receiver))
            {
                _isReceiverEnd = true;
            }
            if (sender.Equals(Receiver))
            {
                _isSenderEnd = true;
            }

            if (_isReceiverEnd && _isSenderEnd)
            {
                OnEndOfWork();
            }
        }
        #endregion


        #region private fields
        /// <summary>
        /// raises when handling is over.
        /// </summary>
        private EventHandler _endOfWork;
        protected bool _isReceiverEnd = false;
        protected bool _isSenderEnd = false;
        #endregion
    }
}
