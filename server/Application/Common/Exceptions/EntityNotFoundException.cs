using System;

namespace Application.Common.Exceptions
{
	public class EntityNotFoundException<TEntity> : Exception
		where TEntity : class
	{
		public EntityNotFoundException(object key)
			: base($"Entity \"{typeof(TEntity).Name}\" ({key}) was not found.")
		{ }
	}
}
