using System.Threading.Tasks;

namespace Divergent.ITOps.ViewModelComposition
{
    public interface IViewModel
    {
        Task RaiseEventAsync(ICompositionEvent @event);
        //void OnDataRetrivalCompleted(Action<dynamic> handler);
    }
}
