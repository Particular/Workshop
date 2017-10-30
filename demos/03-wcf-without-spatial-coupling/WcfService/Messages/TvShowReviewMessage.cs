namespace WcfService
{
	public class TvShowReviewMessage : IMessage
	{
		public string NameOfGame { get; set; }
		public string Review { get; set; }
	}
}
