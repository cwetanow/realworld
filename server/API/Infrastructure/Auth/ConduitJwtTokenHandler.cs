using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace API.Infrastructure.Auth
{
	public class ConduitJwtTokenHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly JwtTokenService tokenService;

		public ConduitJwtTokenHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock,
			JwtTokenService tokenService)
			: base(options, logger, encoder, clock)
		{
			this.tokenService = tokenService;
		}

		protected override Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			if (!Request.Headers.ContainsKey("Authorization"))
			{
				return Task.FromResult(AuthenticateResult.Fail("Missing header"));
			}

			var token = Request.Headers["Authorization"].SingleOrDefault();
			token = token?.Replace("Token ", string.Empty);

			if (string.IsNullOrEmpty(token))
			{
				return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
			}

			var result = tokenService.ValidateToken(token, Scheme.Name);

			return Task.FromResult(result);
		}
	}
}
