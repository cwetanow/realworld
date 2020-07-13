namespace Application.Common.Interfaces
{
	public interface ICurrentUserService
	{
		string Email { get; }
		int UserId { get; }

		bool IsAuthenticated { get; }
	}
}
