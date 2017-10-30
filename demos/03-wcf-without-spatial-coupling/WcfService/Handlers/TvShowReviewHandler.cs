using System;
using System.Diagnostics;

namespace WcfService.Handlers
{
	public class TvShowReviewHandler : IHandleMessages<TvShowReviewMessage>
	{
		public void Handle(TvShowReviewMessage message)
		{
			Debug.WriteLine(String.Format("{0} came in...", message.NameOfGame));
		}
	}
}