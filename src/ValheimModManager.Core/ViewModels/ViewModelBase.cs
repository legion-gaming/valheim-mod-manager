using Prism.Mvvm;
using Prism.Navigation;

namespace ValheimModManager.Core.ViewModels
{
    public abstract class ViewModelBase : BindableBase, IDestructible
    {
        protected ViewModelBase()
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
