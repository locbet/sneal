using System;

namespace Stormwind.Core.Models
{
	/// <summary>
	/// Implementors are entities and identifable by Id.
	/// </summary>
	public interface IEntity
	{
		/// <summary>
		/// The entity identifier.
		/// </summary>
		Guid Id { get; }
	}
}
