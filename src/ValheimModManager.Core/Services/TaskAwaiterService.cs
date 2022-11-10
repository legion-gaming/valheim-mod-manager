using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Events;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public class TaskAwaiterService : ITaskAwaiterService
    {
        private readonly ILogger<TaskAwaiterService> _logger;
        private readonly IEventAggregator _eventAggregator;

        public TaskAwaiterService(ILogger<TaskAwaiterService> logger, IEventAggregator eventAggregator)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;
        }

        public void Await(Task task, Action<Exception> errorCallback = null, bool notifyStatus = false)
        {
            AwaitInternal(task, errorCallback, notifyStatus);
        }

        public T Await<T>(Func<Task<T>> task, Action<Exception> errorCallback = null, bool notifyStatus = false)
        {
            var unwrappedTask = Task.Factory.StartNew(task).Unwrap();

            AwaitInternal(unwrappedTask, errorCallback, notifyStatus);

            return unwrappedTask.Result;
        }

        private async void AwaitInternal(Task task, Action<Exception> errorCallback, bool notifyStatus)
        {
            try
            {
                if (notifyStatus)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(task);
                }

                await task;
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);

                errorCallback?.Invoke(error);
            }
            finally
            {
                if (notifyStatus)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(task);
                }
            }
        }
    }
}
