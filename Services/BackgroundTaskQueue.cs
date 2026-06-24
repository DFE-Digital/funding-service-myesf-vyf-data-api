using PDS.ViewYourFunding.Data.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// Implementation of <see cref="IBackgroundTaskQueue"/> using a <see cref="ConcurrentQueue{T}"/>.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        /// <summary>
        /// The workitems queue.
        /// </summary>
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        /// <summary>
        /// The signal semaphore.
        /// </summary>
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        /// <summary>
        /// Queue a background task.
        /// </summary>
        /// <param name="workItem">The task to queue.</param>
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        /// <summary>
        /// Dequeue the next background task.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token to use when waiting for the task.</param>
        /// <returns>The function containing the background task.</returns>
        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}