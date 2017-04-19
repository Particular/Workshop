using System.Threading.Tasks;

namespace ITOps.ViewModelComposition
{
    public interface IViewModel
    {
        Task RaiseEventAsync(object @event);
    }
}
