using System;

namespace WcfService2
{
	public class Service1 : IService1
	{
        #region LotsOfMethods
        public int CreateTvShow(string name, int year)
	    {
	        throw new NotImplementedException();
	    }

	    public bool UpdateTvShow(string name, int year)
	    {
	        throw new NotImplementedException();
	    }

	    public bool CloseVoting(int tvShow)
	    {
	        throw new NotImplementedException();
	    }

	    public bool OpenVoting(int tvShow)
	    {
	        throw new NotImplementedException();
	    }

	    public int GetNumberOfVotes(int tvShow)
	    {
	        throw new NotImplementedException();
	    }

        public int GetNumberOfVotes(int tvShow, DateTime @from, DateTime until)
	    {
	        throw new NotImplementedException();
	    }

	    public bool CanUserStillVote(int user)
	    {
	        throw new NotImplementedException();
	    }

	    public bool AddVote(int user, int tvShow, int numberOfStars)
	    {
	        throw new NotImplementedException();
	    }
        #endregion

        public string SetTvShowOfTheYear(TvShowOfTheYearMessage message)
	    {
	        return $"I've set tv-show of the year for {message.Year} to {message.Name}";
	    }

	    public string AddTvShowReview(TvShowReviewMessage message)
	    {
	        return $"Thanks for your review of {message.Name}";
        }
	}
}
