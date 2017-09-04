using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core.Implementations
{
    /// <summary>
    /// Provides base functionalty for background workers aka workers in the separate threads
    /// </summary>
    public abstract class BackgroundWorkerBase : IBackgroundWorker
    {

        #region public methods
        /// <summary>
        /// Starts background work.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask =Task.Run(() =>ExecuteBackgroundAsync(_cts.Token));

            // If the task is completed then return it, otherwise it's running
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            // Signal cancellation to the executing method
            _cts.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken));

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }
        #endregion

        #region protected methods
        // Derived classes should override this and execute a long running method until 
        // cancellation is requested
        protected abstract Task ExecuteBackgroundAsync(CancellationToken cancellationToken);
        #endregion

        #region private and protected fields
        private Task _executingTask;
        private CancellationTokenSource _cts;
        #endregion
    }
}
