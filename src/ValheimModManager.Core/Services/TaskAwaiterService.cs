using System;
using System.Threading.Tasks;

using Prism.Events;

using ValheimModManager.Core.Data;

namespace ValheimModManager.Core.Services
{
    public class TaskAwaiterService : ITaskAwaiterService
    {
        private readonly IEventAggregator _eventAggregator;

        public TaskAwaiterService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public async void Await(Task task, Action<Exception> errorCallback = null, bool showProgress = false)
        {
            try
            {
                if (showProgress)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(new TaskStatusEvent());
                }

                await task;
            }
            catch (Exception error)
            {
                errorCallback?.Invoke(error);
            }
            finally
            {
                if (showProgress)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(new TaskStatusEvent { IsCompleted = true });
                }
            }
        }

        public T Await<T>(Func<Task<T>> task, Action<Exception> errorCallback = null, bool showProgress = false)
        {
            try
            {
                if (showProgress)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(new TaskStatusEvent());
                }

                return Task.Factory.StartNew(task).Unwrap().GetAwaiter().GetResult();
            }
            catch (Exception error)
            {
                errorCallback?.Invoke(error);
            }
            finally
            {
                if (showProgress)
                {
                    _eventAggregator.GetEvent<TaskStatusEvent>().Publish(new TaskStatusEvent { IsCompleted = true });
                }
            }

            return default;
        }
    }
}
