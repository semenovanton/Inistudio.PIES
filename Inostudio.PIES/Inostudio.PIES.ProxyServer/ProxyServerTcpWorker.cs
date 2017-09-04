using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System;

using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;

namespace Inostudio.PIES.ProxyServer
{
    /// <summary>
    /// Server TCP worker implementation for the PROXY server.
    /// <para>This worker wraps TCP connection with one another server, Initiator or Proxy.</para>
    /// </summary>
    class ProxyServerTcpWorker 
        : ServerTcpWorkerBase
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyServerTcpWorker"/>.
        /// </summary>
        /// <param name="receiver">TCP receiver handler.</param>
        /// <param name="sender">TCP sender handler.</param>
        /// <param name="fromEchoBuffer">The buffer which is used for sending numbers from ECHO server to INITIATOR.</param>
        /// <param name="fromInitiatorBuffer">The buffer which is used for sending numbers from INITIATOR server to ECHO.</param>
        public ProxyServerTcpWorker(
            ITcpReceiver receiver, 
            ITcpSender sender,
            IBuffer<long> fromInitiatorBuffer,
            IBuffer<long> fromEchoBuffer) 
            : base (null, receiver, sender)
        {
            _fromInitiatorBuffer = fromInitiatorBuffer;
            _fromEchoBuffer = fromEchoBuffer;
        }
        #endregion

        #region public methods
        public override void ReceiveServerType()
        {
            //get server type of the other server, with wich we connected via current TCP connection
            base.ReceiveServerType();

            //set buffers for handlers according to this server type;
            switch (ServerType)
            {
                case ServerType.Initiator:
                    Receiver.Buffer = _fromInitiatorBuffer;
                    Sender.Buffer = _fromEchoBuffer;
                    break;
                case ServerType.Echo:
                    Receiver.Buffer = _fromEchoBuffer;
                    Sender.Buffer = _fromInitiatorBuffer;
                    break;
            }
        }
        #endregion

        #region protected methods
        protected override Task ExecuteBackgroundAsync(CancellationToken cancellationToken)
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
        private IBuffer<long> _fromInitiatorBuffer;
        private IBuffer<long> _fromEchoBuffer;
        #endregion
    }
}
