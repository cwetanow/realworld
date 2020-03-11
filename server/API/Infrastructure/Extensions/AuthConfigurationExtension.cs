using System;
using System.Text;
using System.Threading.Tasks;
using API.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Infrastructure.Extensions
{
	public static class AuthConfigurationExtension
	{
		private const string ConduitJwtAuthenticationScheme = "ConduitJwt";

		public static IServiceCollection ConfigureConduitJwtAuthentication(this IServiceCollection services)
		{
			services
				.AddAuthentication(ConduitJwtAuthenticationScheme)
				.AddScheme<AuthenticationSchemeOptions, ConduitJwtTokenHandler>(ConduitJwtAuthenticationScheme, null);

			return services;
		}

		public static IServiceCollection ConfigureAuthorization(this IServiceCollection services) =>
			services.AddAuthorization(options => {
				options.DefaultPolicy = new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.AddAuthenticationSchemes(ConduitJwtAuthenticationScheme)
					.Build();

				options.AddPolicy("default", p => 
					p.RequireAuthenticatedUser().AddAuthenticationSchemes(ConduitJwtAuthenticationScheme));
			});

		public static IServiceCollection ConfigureJwtAuthentication(this IServiceCollection services, string secretKey, string issuer, string audience)
		{
			var keyByteArray = Encoding.UTF8.GetBytes(secretKey);
			var signingKey = new SymmetricSecurityKey(keyByteArray);

			var tokenValidationParameters = new TokenValidationParameters {
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = signingKey,
				ValidIssuer = issuer,
				ValidAudience = audience,
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			};

			services
				.AddAuthentication(o => {
					o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					o.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(o => {
					o.RequireHttpsMetadata = false;
					o.SaveToken = true;
					o.TokenValidationParameters = tokenValidationParameters;
					o.Events = new JwtBearerEvents {
						OnAuthenticationFailed = context => Task.CompletedTask,
						OnTokenValidated = context => Task.CompletedTask
					};
				});

			return services;
		}
	}
}
