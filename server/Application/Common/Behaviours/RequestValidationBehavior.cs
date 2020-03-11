﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours
{
	public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
		where TRequest : IRequest<TResponse>
	{
		private readonly IEnumerable<IValidator<TRequest>> validators;

		public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
		{
			this.validators = validators;
		}

		public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var context = new ValidationContext(request);

			var failures = validators
				.Select(v => v.Validate(context))
				.SelectMany(result => result.Errors)
				.Where(f => f != null)
				.ToList();

			if (failures.Any())
			{
				throw new RequestValidationException(failures);
			}

			return await next();
		}
	}
}
