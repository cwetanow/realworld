namespace Application.Common.Interfaces
{
	public interface ICurrentUserService
	{
		string Email { get; }

		bool IsAuthenticated { get; }
	}
}
