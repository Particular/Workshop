using System.Diagnostics;

namespace WcfService2.Handlers
{
	public class TvShowOfTheYearHandler : IHandleMessages<TvShowOfTheYearMessage>
	{
		public void Handle(TvShowOfTheYearMessage message)
		{
			Debug.WriteLine("The game of the year {0} is {1}", message.Year, message.Name);
		}
	}
}