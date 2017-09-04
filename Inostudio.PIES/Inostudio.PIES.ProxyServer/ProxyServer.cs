using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

using Inostudio.PIES.Core;
using Inostudio.PIES.Core.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Inostudio.PIES.ProxyServer
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
            _fromInitiatorBuffer = new Buffer<long>();
            _fromEchoBuffer = new Buffer<long>();

            //config DI
            _serviceProvider = GetServiceProvider();
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
        protected override async Task ExecuteBackgroundAsync(CancellationToken cancellationToken)
        {
            ///start TCP listener.
            _server.Start();

            while (!cancellationToken.IsCancellationRequested)
            {
                //wait a new connection from both servers.
                var client = await _server.AcceptTcpClientAsync();

                //create a new TCP worker;
                var serverWorker = _serviceProvider.GetService<IServerTcpWorker>();
                serverWorker.EndOfWork += OnEndOfWork;

                //set TCP client to it
                serverWorker.TcpClient = client;
                serverWorker.Receiver.TcpClient = client;
                serverWorker.Sender.TcpClient = client;

                //detect, which server is connected (INITIATOR or ECHO); and set buffers according to this type.
                serverWorker.ReceiveServerType();

                //show message that the connection was added
                Console.WriteLine(TextConstants.ResourceManager.GetString("ServerConnected").Replace("{ServerType}", serverWorker.ServerType.ToString()));

                //start its working
                var task = serverWorker.StartAsync(cancellationToken);
            }
        }

        protected IServiceProvider GetServiceProvider()
        {
            //TODO: create a proper factory 
            var serviceProvider = new ServiceCollection()
                .AddTransient<IServerTcpWorker, ProxyServerTcpWorker>(sp =>
                    new ProxyServerTcpWorker(
                        sp.GetService<ITcpReceiver>(),
                        sp.GetService<ITcpSender>(), 
                        _fromInitiatorBuffer, 
                        _fromEchoBuffer))
                .AddTransient<ITcpSender, ProxyTcpSender>()
                .AddTransient<ITcpReceiver, ProxyTcpReceiver>()
                .BuildServiceProvider();

            return serviceProvider;
        }
        #endregion

        #region private methods
        private void OnEndOfWork(object sender, EventArgs e)
        {
            var worker = sender as ProxyServerTcpWorker;
            if (worker != null)
            {
                worker.EndOfWork -= OnEndOfWork;

                //show message that the connection was deleted
                Console.WriteLine(TextConstants.ResourceManager.GetString("ServerDisconnected").Replace("{ServerType}", worker.ServerType.ToString()));
            }
        }


        /// <summary>
        /// Receives the server type over the TCP connection.
        /// <para>Use when this worker initializes the connection and its server type is known.</para>
        /// </summary>
        private ServerType ReceiveServerType(TcpClient client)
        {
            using (var reader = new System.IO.BinaryReader(client.GetStream()))
            {
                return (ServerType)reader.ReadByte();
            }
        }
        #endregion

        #region private fields
        /// <summary>
        /// buffer, in which we push numbers received from INITIATOR server and from which we take numbers to send to ECHO server.
        /// </summary>
        private Buffer<long> _fromInitiatorBuffer;
        /// <summary>
        /// buffer, in which we push numbers received from ECHO server and from which we take numbers to send to INITIATOR server.
        /// </summary>
        private Buffer<long> _fromEchoBuffer;
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
        private IServiceProvider _serviceProvider;
        #endregion
    }
}
