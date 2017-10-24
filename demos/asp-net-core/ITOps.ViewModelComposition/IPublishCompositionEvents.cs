namespace ITOps.ViewModelComposition
{
    public interface IPublishCompositionEvents
    {
        void Subscribe<TEvent>(EventHandler<TEvent> handler);
    }
}
