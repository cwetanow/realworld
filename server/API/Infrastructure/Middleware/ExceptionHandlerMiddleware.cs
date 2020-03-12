using System;
using System.Net;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace API.Infrastructure.Middleware
{
	public class ExceptionHandlerMiddleware
	{
		private readonly RequestDelegate next;

		public ExceptionHandlerMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var code = HttpStatusCode.InternalServerError;

			var result = string.Empty;

			if (exception is RequestValidationException validationException)
			{
				code = HttpStatusCode.BadRequest;
				result = JsonConvert.SerializeObject(validationException.Failures);
			}
			else if (exception.GetType().IsAssignableFrom(typeof(EntityNotFoundException<>)))
			{
				code = HttpStatusCode.NotFound;
			}

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;

			if (result == string.Empty)
			{
				result = JsonConvert.SerializeObject(new { error = exception.Message });
			}

			await context.Response.WriteAsync(result);
		}
	}
}
