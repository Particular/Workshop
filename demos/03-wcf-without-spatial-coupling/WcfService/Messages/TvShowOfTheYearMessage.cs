namespace WcfService
{
	public class TvShowOfTheYearMessage : IMessage
	{
		public string Name { get; set; }
		public int Year { get; set; }
	}
}
