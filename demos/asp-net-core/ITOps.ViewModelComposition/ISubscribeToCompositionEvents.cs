namespace ITOps.ViewModelComposition
{
    public interface ISubscribeToCompositionEvents : IRouteInterceptor
    {
        void Subscribe(IPublishCompositionEvents publisher);
    }
}
