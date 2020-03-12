namespace Domain.Entities
{
	public class UserFollower
	{
		private UserFollower() { }

		public UserFollower(int userId, int followerId)
		{
			UserId = userId;
			FollowerId = followerId;
		}

		public int UserId { get; set; }
		public UserProfile User { get; set; }

		public int FollowerId { get; set; }
		public UserProfile Follower { get; set; }
	}
}
