using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Inostudio.Test.Core;
using System;

namespace Inostudio.Test.InitiatorServer
{
    /// <summary>
    /// Provides the main server process for the INITIATOR server.
    /// </summary>
    class InitiatorServer 
        : BackgroundWorkerBase, IServer
    {
        #region public constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InitiatorServer"/>.
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which current server would connect.</param>
        /// <param name="proxyPort">The port number of the proxy server, to which current server would connect.</param>
        /// <param name="threadsCount">The number of threads, which should be created in the ECHO server for handling TCP connections. Also count of TCP connections at all</param>
        public InitiatorServer(string proxyHostName, int proxyPort, int threadsCount)
        {
            //remove previous outputfiles
            if (System.IO.File.Exists(SEND_OUT_FILE_NAME))
            {
                System.IO.File.Delete(SEND_OUT_FILE_NAME);
            }

            if (System.IO.File.Exists(RECEIVE_OUT_FILE_NAME))
            {
                System.IO.File.Delete(RECEIVE_OUT_FILE_NAME);
            }

            //init base params
            _proxyHostName = proxyHostName;
            _proxyPort = proxyPort;
            _threadCount = threadsCount;

            //init buffers
            _senderBuffer = new InitiatorSequenceBuffer();                                                  //set buffer, that generates numbers
            _senderOutBuffer = new FlushingBuffer<long>(SEND_OUT_FILE_NAME, SEND_OUT_BUFFER_SIZE);          //this buffer is used to write sending to PROXY numbers into the output file.
            _receiverOutBuffer = new FlushingBuffer<long>(RECEIVE_OUT_FILE_NAME, RECEIVE_OUT_BUFFER_SIZE);  //this buffer is used to write received from PROXY numbers into the output file.


            //init server workers
            _serverWorkers = new List<InitiatorServerTcpWorker>();
            for (var i = 0; i < threadsCount; i++)
            {
                var serverWorker = new InitiatorServerTcpWorker(
                    _proxyHostName,
                    _proxyPort,
                    _receiverOutBuffer,
                    _senderBuffer,
                    _senderOutBuffer,
                    i);
                serverWorker.EndOfWork += OnEndOfWork;
                _serverWorkers.Add(serverWorker);
            }
        }
        #endregion

        #region protected methods
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            //wait until workers will be done
            return Task.WhenAll(_serverWorkers.Select(x => x.StartAsync(cancellationToken)));
        }
        #endregion

        #region private methods
        private void OnEndOfWork(object sender, EventArgs e)
        {
            lock (this)
            {
                _stoppedThreadCount++;

                if (_stoppedThreadCount == _threadCount)
                {
                    //flush buffers on the end of all threads
                    _senderOutBuffer.Flush();
                    _receiverOutBuffer.Flush();
                }
            }
        }
        #endregion


        #region private fields
        /// <summary>
        /// buffer, that generates numbers
        /// </summary>
        private IBuffer<long> _senderBuffer;
        /// <summary>
        /// this buffer is used to write sending to PROXY numbers into the output file.
        /// </summary>
        private IFlushingBuffer<long> _senderOutBuffer;
        /// <summary>
        /// this buffer is used to write received from PROXY numbers into the output file.
        /// </summary>
        private IFlushingBuffer<long> _receiverOutBuffer;
        /// <summary>
        /// server TCP workers.
        /// </summary>
        private List<InitiatorServerTcpWorker> _serverWorkers;
        /// <summary>
        /// the host name of the proxy server, to which current server would connect.
        /// </summary>
        private string _proxyHostName;
        /// <summary>
        /// the port number of the proxy server, to which current server would connect.
        /// </summary>
        private int _proxyPort;
        /// <summary>
        /// the number of threads, which should be created in the ECHO server for handling TCP connections. Also count of TCP connections at all
        /// </summary>
        private int _threadCount;
        private int _stoppedThreadCount;
        #endregion

        #region private constants
        private const string SEND_OUT_FILE_NAME = "initiator_send.txt";
        private const int SEND_OUT_BUFFER_SIZE = 50;
        private const string RECEIVE_OUT_FILE_NAME = "initiator_receive.txt";
        private const int RECEIVE_OUT_BUFFER_SIZE = 300;
        #endregion
    }
}
