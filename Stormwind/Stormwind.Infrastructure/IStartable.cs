namespace Stormwind.Infrastructure
{
	/// <summary>
	/// Implementors of this need to have their Start() method called
	/// before they are fully initialized.
	/// </summary>
	public interface IStartable
	{
		/// <summary>
		/// Starts the given service instance.
		/// </summary>
		void Start();
	}
}
