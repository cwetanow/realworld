using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Identity
{
	public class JwtTokenService
	{
		private readonly AuthConfiguration configuration;

		public JwtTokenService(AuthConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string CreateToken(string email, params KeyValuePair<string, string>[] additionalClaims)
		{
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.Sub, email),
				new Claim(JwtRegisteredClaimNames.Jti, email)
			};

			foreach (var additionalClaim in additionalClaims)
			{
				var claim = new Claim(additionalClaim.Key, additionalClaim.Value);
				claims.Add(claim);
			}

			return GenerateToken(claims);
		}

		public string GenerateToken(IEnumerable<Claim> claims)
		{
			var expires = DateTime.UtcNow.AddHours(configuration.ValidHours);

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.SecretKey));

			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(claims),
				Expires = expires,
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
				Issuer = configuration.Issuer,
				Audience = configuration.Audience
			};

			var handler = new JwtSecurityTokenHandler();

			var token = handler.CreateToken(tokenDescriptor);

			return handler.WriteToken(token);
		}

		public AuthenticateResult ValidateToken(string token, string scheme)
		{
			var keyByteArray = Encoding.UTF8.GetBytes(configuration.SecretKey);
			var signingKey = new SymmetricSecurityKey(keyByteArray);

			var tokenValidationParameters = new TokenValidationParameters {
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,
				ValidIssuer = configuration.Issuer,
				ValidAudience = configuration.Audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			try
			{
				var handler = new JwtSecurityTokenHandler();

				handler.ValidateToken(token, tokenValidationParameters, out _);
				var securityToken = handler.ReadJwtToken(token);

				var identity = new ClaimsIdentity(securityToken.Claims, scheme);
				var principal = new ClaimsPrincipal(identity);
				var ticket = new AuthenticationTicket(principal, scheme);

				return AuthenticateResult.Success(ticket);
			}
			catch
			{
				return AuthenticateResult.Fail("Invalid token");
			}
		}
	}
}
