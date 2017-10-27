namespace WcfService2
{
	public interface IHandleMessages<T> where T : IMessage
	{
		void Handle(T message);
	}
}