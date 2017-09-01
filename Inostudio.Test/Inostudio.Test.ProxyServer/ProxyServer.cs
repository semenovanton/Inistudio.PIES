using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Inostudio.Test.Core;

namespace Inostudio.Test.ProxyServer
{
    /// <summary>
    /// Provides the main server process for ECHO server.
    /// </summary>
    class ProxyServer
        : BackgroundWorkerBase, IServer
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyServer"/>
        /// </summary>
        /// <param name="hostName">The server hostname.</param>
        /// <param name="port">The server port number.</param>
        public ProxyServer(
            string hostName,
            int port)
        {
            //init base params
            _hostName = hostName;
            _port = port;

            //init TCP listener
            var localAddr = IPAddress.Parse(_hostName);

            _server = new TcpListener(localAddr, _port);

            //init buffers
            _initiatorSenderBuffer = new Buffer<long>();
            _echoSenderBuffer = new Buffer<long>();

            //init workers collections
            _initiatorWorkers = new List<ProxyServerWorker>();
            _echoWorkers = new List<ProxyServerWorker>();
        }
        #endregion

        #region public methods
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _server.Stop();
            return base.StopAsync(cancellationToken);
        }
        #endregion

        #region protected methods
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            ///start TCP listener.
            _server.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                //wait a new connection from both servers.
                var client = await _server.AcceptTcpClientAsync();

                var stream = client.GetStream();

                //wait until init message from server was received
                while (client.Available < 8)
                {
                    Thread.Sleep(10);
                }

                //detect, which server is connected.
                var bytes = new byte[8];
                await stream.ReadAsync(bytes, 0, bytes.Length);
                var serverType = (ServerType)BitConverter.ToInt64(bytes, 0);

                ProxyServerWorker proxyServerWorker;
                Task task;
                switch (serverType)
                {
                    case ServerType.Initiator:
                        proxyServerWorker = new ProxyServerWorker(client, _echoSenderBuffer, _initiatorSenderBuffer, serverType);
                        proxyServerWorker.EndOfWork += OnEndOfWork;
                        task = proxyServerWorker.StartAsync(cancellationToken);
                        _initiatorWorkers.Add(proxyServerWorker);



                        break;
                    case ServerType.Echo:
                        proxyServerWorker = new ProxyServerWorker(client, _initiatorSenderBuffer, _echoSenderBuffer, serverType);
                        proxyServerWorker.EndOfWork += OnEndOfWork;
                        task = proxyServerWorker.StartAsync(cancellationToken);
                        _echoWorkers.Add(proxyServerWorker);
                        break;
                }

                //show message that the connection was added
                Console.WriteLine(TextConstants.ResourceManager.GetString("SeparatorLine"));
                Console.WriteLine(TextConstants.ResourceManager.GetString("ServerConnected").Replace("{ServerType}", serverType.ToString()));
                Console.WriteLine(TextConstants.ResourceManager.GetString("SeparatorLine"));
            }
        }
        #endregion

        #region private methods
        private void OnEndOfWork(object sender, EventArgs e)
        {
            var worker = sender as ProxyServerWorker;
            if (worker != null)
            {
                worker.EndOfWork -= OnEndOfWork;
                switch (worker.ServerType)
                {
                    case ServerType.Echo:
                        _echoWorkers.Remove(worker);
                        break;
                    case ServerType.Initiator:
                        _initiatorWorkers.Remove(worker);
                        break;
                }

                //show message that the connection was deleted
                Console.WriteLine(TextConstants.ResourceManager.GetString("SeparatorLine"));
                Console.WriteLine(TextConstants.ResourceManager.GetString("ServerDisconnected").Replace("{ServerType}", worker.ServerType.ToString()));
                Console.WriteLine(TextConstants.ResourceManager.GetString("SeparatorLine"));
            }
        }
        #endregion

        #region private fields
        /// <summary>
        /// buffer, in which we push numbers received from INITIATOR server and from which we take numbers to send to ECHO server.
        /// </summary>
        private Buffer<long> _initiatorSenderBuffer;
        /// <summary>
        /// buffer, in which we push numbers received from ECHO server and from which we take numbers to send to INITIATOR server.
        /// </summary>
        private Buffer<long> _echoSenderBuffer;

        /// <summary>
        /// collection with workers, which handle connections with the INITIATOR server.
        /// </summary>
        private List<ProxyServerWorker> _initiatorWorkers;
        /// <summary>
        /// collection with workers, which handle connections with the INITIATOR server.
        /// </summary>
        private List<ProxyServerWorker> _echoWorkers;

        /// <summary>
        /// the server host name
        /// </summary>
        private string _hostName;
        /// <summary>
        /// the server port number
        /// </summary>
        private int _port;
        /// <summary>
        /// the server TCP  listener.
        /// </summary>
        private TcpListener _server;
        #endregion
    }
}
