using System;
using System.Threading.Tasks;

namespace ValheimModManager.Core.Helpers
{
    public class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<T> valueFactory)
            : base(() => Task.Factory.StartNew(valueFactory))
        {
        }

        public AsyncLazy(Func<Task<T>> valueFactory)
            : base(() => Task.Factory.StartNew(valueFactory).Unwrap())
        {
        }
    }
}
