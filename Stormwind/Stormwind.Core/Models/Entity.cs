using System;
using Sneal.Core;

namespace Stormwind.Core.Models
{
	public abstract class AggregateRoot<TEntity> :
		IEquatable<AggregateRoot<TEntity>>,
		IEntity where TEntity : IEntity
	{
		/// <summary>
		/// Parameterless constructor is required for NHibernate runtime proxying.
		/// </summary>
		protected AggregateRoot()
		{
			Id = SequentialGuid.NewGuid();
		}

		/// <summary>
		/// The entity identifier.
		/// </summary>
		public virtual Guid Id { get; protected set; }

		/// <summary>
		/// The optimistic lock entity version.
		/// </summary>
		public virtual int Version { get; protected set; }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public virtual bool Equals(AggregateRoot<TEntity> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Id.Equals(Id);
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
		/// </returns>
		/// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (AggregateRoot<TEntity>)) return false;
			return Equals((AggregateRoot<TEntity>) obj);
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public static bool operator ==(AggregateRoot<TEntity> left, AggregateRoot<TEntity> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(AggregateRoot<TEntity> left, AggregateRoot<TEntity> right)
		{
			return !Equals(left, right);
		}
	}
}
