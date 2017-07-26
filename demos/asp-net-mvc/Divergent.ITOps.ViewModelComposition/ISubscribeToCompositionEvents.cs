namespace Divergent.ITOps.ViewModelComposition
{
    public interface ISubscribeToCompositionEvents: IRouteInterceptor
    {
        void Subscribe(ISubscriptionStorage subscriptionStorage);
    }
}
