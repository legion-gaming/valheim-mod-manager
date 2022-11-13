using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;

using ValheimModManager.Core.Services;

namespace ValheimModManager.Core.ViewModels
{
    public abstract class ViewModelBase<TViewModel> : BindableBase, IDestructible
        where TViewModel : BindableBase
    {
        private readonly ITaskAwaiterService _taskAwaiterService;

        protected ViewModelBase
        (
            ILogger<TViewModel> logger,
            ITaskAwaiterService taskAwaiterService,
            IEventAggregator eventAggregator
        )
        {
            _taskAwaiterService = taskAwaiterService;

            Logger = logger;
            EventAggregator = eventAggregator;
        }

        protected ILogger<TViewModel> Logger { get; }
        protected IEventAggregator EventAggregator { get; }

        public virtual void Destroy()
        {
        }

        protected void RunAsync(Task task, Action<Exception> errorCallback = null, bool notifyStatus = false)
        {
            _taskAwaiterService.Await(task, errorCallback, notifyStatus);
        }

        protected T RunAsync<T>(Func<Task<T>> task, Action<Exception> errorCallback = null, bool notifyStatus = false)
        {
            return _taskAwaiterService.Await(task, errorCallback, notifyStatus);
        }
    }
}
