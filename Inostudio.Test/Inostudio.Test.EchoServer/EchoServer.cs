using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Inostudio.Test.Core;
using System;

namespace Inostudio.Test.EchoServer
{
    /// <summary>
    /// Provides the main server process for ECHO server.
    /// </summary>
    class EchoServer 
        : BackgroundWorkerBase, IServer
    {
        #region public constructors
        /// <summary>
        /// initializes a new instance of the <see cref="EchoServer"/>
        /// </summary>
        /// <param name="proxyHostName">The host name of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="proxyPort">The port of the proxy server, to which ECHO server should be connected.</param>
        /// <param name="threadsCount">The number of threads, which should be created in the ECHO server for handling TCP connections. Also count of TCP connections at all</param>
        public EchoServer(string proxyHostName, int proxyPort, int threadsCount)
        {
            //remove output file if it exists
            if (System.IO.File.Exists(ECHO_OUT_FILE_NAME))
            {
                System.IO.File.Delete(ECHO_OUT_FILE_NAME);
            }

            //init base params
            _proxtHostName = proxyHostName;
            _proxyPort = proxyPort;
            _threadCount = threadsCount;

            //this buffer should be used both for receiving numbers and sending them back.
            _receiverBuffer = new Buffer<long>();

            //this buffer should be used for flushing received numbers into the output file.
            _echoBuffer = new FlushingBuffer<long>(ECHO_OUT_FILE_NAME, ECHO_OUT_BUFFER_SIZE);

            //creates the server workers (TCP incapsulators)
            _serverWorkers = new List<EchoServerTcpWorker>();
            for (var i = 0; i < threadsCount; i++)
            {
                var serverWorker = new EchoServerTcpWorker(_proxtHostName, _proxyPort, _receiverBuffer, _echoBuffer, i);
                serverWorker.EndOfWork += OnEndOfWork;
                _serverWorkers.Add(serverWorker);
            }
        }
        #endregion


        #region protected methods
        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return  Task.WhenAll(_serverWorkers.Select(x => x.StartAsync(cancellationToken)).ToArray());
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
                    //flush buffer on the end of all threads
                    _echoBuffer.Flush();
                }
            }
        }
        #endregion


        #region private fields
        /// <summary>
        /// this buffer should be used both for receiving numbers and sending them back.
        /// </summary>
        private Buffer<long> _receiverBuffer;
        /// <summary>
        /// server tcp workers.
        /// </summary>
        private List<EchoServerTcpWorker> _serverWorkers;
        /// <summary>
        /// this buffer should be used for flushing received numbers into the output file.
        /// </summary>
        private IFlushingBuffer<long> _echoBuffer;
        /// <summary>
        /// proxy server host name.
        /// </summary>
        private string _proxtHostName;
        /// <summary>
        /// proxy server port.
        /// </summary>
        private int _proxyPort;
        /// <summary>
        /// count of TCP connections and threads for handle them.
        /// </summary>
        private int _threadCount;
        private int _stoppedThreadCount;
        #endregion


        #region private constants
        private const string ECHO_OUT_FILE_NAME = "echo.txt";
        private const int ECHO_OUT_BUFFER_SIZE = 300;
        #endregion
    }
}
