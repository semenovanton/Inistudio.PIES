using System.Threading;
using System.Threading.Tasks;

namespace Inostudio.PIES.Core
{
    /// <summary>
    /// Base abstraction for background workers aka workers in the separate threads
    /// </summary>
    public interface IBackgroundWorker
    {
        /// <summary>
        /// Starts the background work.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task StartAsync(CancellationToken cancellationToken);
        /// <summary>
        /// Stops the background work.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task StopAsync(CancellationToken cancellationToken);
    }
}
