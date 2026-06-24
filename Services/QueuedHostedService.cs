using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PDS.ViewYourFunding.Data.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PDS.ViewYourFunding.Data.Services
{
    /// <summary>
    /// A <see cref="BackgroundService"/> that polls a task queue for work items to execute.
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<QueuedHostedService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedHostedService"/> class.
        /// </summary>
        /// <param name="taskQueue">The queue that will contain tasks to run.</param>
        /// <param name="logger">The logger.</param>
        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger)
        {
            _taskQueue = taskQueue;
            _logger = logger;
        }

        /// <summary>
        /// The method that is called when the service starts. Polls <see cref="_taskQueue"/> for tasks to run.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token to notify when operations should be cancelled.</param>
        /// <returns>Async task.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error occurred executing {WorkItem}.",
                        nameof(workItem));
                }
            }
        }
    }
}