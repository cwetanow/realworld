using Application.Common.Models;

namespace Application.Common.Interfaces
{
	public interface IUserService
	{
		(Result result, string userId) CreateUser(string username, string email, string password);
	}
}
