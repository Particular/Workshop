using System;
using System.ServiceModel;

namespace WcfService
{
	[ServiceContract]
	public interface IService
	{
		[OperationContract]
	    int CreateTvShow(string name, int year);

		[OperationContract]
        bool UpdateTvShow(string name, int year);

		[OperationContract]
	    bool CloseVoting(int tvShow);

        [OperationContract]
	    bool OpenVoting(int tvShow);

        [OperationContract]
	    int GetNumberOfVotes(int tvShow);

        [OperationContract(Name="GetNumberOfVotesByPeriod")]
	    int GetNumberOfVotes(int tvShow, DateTime from, DateTime until);

	    [OperationContract]
	    bool CanUserStillVote(int user);

	    bool AddVote(int user, int tvShow, int numberOfStars);

        [OperationContract]
		string SetTvShowOfTheYear(TvShowOfTheYearMessage message);

	    [OperationContract]
	    string AddTvShowReview(TvShowReviewMessage message);
	}
}
