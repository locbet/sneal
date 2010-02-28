using Castle.DynamicProxy;
using NHibernate.ByteCode.Castle;

namespace Stormwind.Infrastructure.Properties
{
	/// <summary>
	/// These references cause VS to copy these dependencies to any
	/// consumers of Stormwind.Infrastructure.dll.
	/// </summary>
	internal static class ShadowCopy
	{
#pragma warning disable 649

		// NHibernate.ByteCode.Castle
		public static ProxyFactory Factory;

		// Castle.DynamicProxy2 & Castle.Core
		public static PersistentProxyBuilder Builder;

#pragma warning restore 649
	}
}
