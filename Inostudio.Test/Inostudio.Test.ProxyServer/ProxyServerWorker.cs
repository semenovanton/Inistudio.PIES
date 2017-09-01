using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Inostudio.Test.Core;
using System;

namespace Inostudio.Test.ProxyServer
{
    /// <summary>
    /// Server TCP worker implementation for the PROXY server.
    /// <para>This worker wraps TCP connection with one another server, Initiator or Proxy.</para>
    /// </summary>
    class ProxyServerWorker 
        : ServerTcpWorkerBase<ProxyTcpReceiver, ProxyTcpSender>
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyServerWorker"/>.
        /// </summary>
        /// <param name="tcpClient">The already working TCP client.</param>
        /// <param name="receiverBuffer">The buffer, to which receiver will push received numbers.</param>
        /// <param name="senderBuffer">The buffer from which sender will take numbers for sending.</param>
        public ProxyServerWorker(
            TcpClient tcpClient, 
            IBuffer<long> receiverBuffer, 
            IBuffer<long> senderBuffer,
            ServerType serverType) 
            : base (tcpClient)
        {
            Receiver = new ProxyTcpReceiver(receiverBuffer);
            Receiver.TcpClient = tcpClient;
            Sender = new ProxyTcpSender(senderBuffer);
            Sender.TcpClient = tcpClient;

            _serverType = serverType;
        }
        #endregion

        #region public properties
        /// <summary>
        /// Gets server type of the server with which Proxy server is connected throught current worker.
        /// </summary>
        public override ServerType ServerType => _serverType;
        #endregion

        #region protected methods
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //sign on EndOfWork TCP handler events
            Receiver.EndOfWork += OnTcpHandlerEndHandler;
            Sender.EndOfWork += OnTcpHandlerEndHandler;

            //start receiver and sender handlers
            var receiverTask = Receiver.StartAsync(cancellationToken);
            var senderTAsk = Sender.StartAsync(cancellationToken);

            return Task.WhenAll(receiverTask, senderTAsk);
        }
        #endregion

        #region private fields
        /// <summary>
        /// stores server type of the server with which Proxy server is connected throught current worker.
        /// </summary>
        private ServerType _serverType;
        #endregion

        protected override void OnTcpHandlerEndHandler(object sender, EventArgs e)
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
    }
}
