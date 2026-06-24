using System;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Interfaces
{
    /// <summary>
    /// Interface for managing a queue for background tasks.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Queue a background task.
        /// </summary>
        /// <param name="workItem">The task to queue.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Dequeue the next background task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use when waiting for the task.</param>
        /// <returns>The function containing the background task.</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}