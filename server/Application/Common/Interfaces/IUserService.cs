using System.Threading.Tasks;
using Application.Common.Models;

namespace Application.Common.Interfaces
{
	public interface IUserService
	{
		Task<(Result result, string userId)> CreateUser(string username, string email, string password);

		Task<string> Authenticate(string email, string password);
	}
}
