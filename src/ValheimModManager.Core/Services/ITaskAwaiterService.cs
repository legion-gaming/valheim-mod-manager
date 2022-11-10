using System;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Services
{
    public interface ITaskAwaiterService
    {
        void Await(Task task, Action<Exception> errorCallback = null, bool notifyStatus = false);
        T Await<T>(Func<Task<T>> task, Action<Exception> errorCallback = null, bool notifyStatus = false);
    }
}
