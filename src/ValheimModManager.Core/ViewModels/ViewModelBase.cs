using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

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
            ITaskAwaiterService taskAwaiterService
        )
        {
            _taskAwaiterService = taskAwaiterService;

            Logger = logger;
        }

        protected ILogger<TViewModel> Logger { get; }

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
