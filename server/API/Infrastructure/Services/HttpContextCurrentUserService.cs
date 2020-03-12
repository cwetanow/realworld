using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace API.Infrastructure.Services
{
	public class HttpContextCurrentUserService : ICurrentUserService
	{
		private readonly IHttpContextAccessor httpContextAccessor;

		public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}

		public string Email => httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type.Equals(JwtRegisteredClaimNames.Sub))?.Value;
		public bool IsAuthenticated => httpContextAccessor.HttpContext.User?.Identity != null;
	}
}
