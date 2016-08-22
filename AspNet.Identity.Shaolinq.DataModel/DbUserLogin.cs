using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserLogin")]
	public abstract class DbUserLogin<TDbUser> : DataAccessObject<Guid>, IShaolinqIdentityDbUserLogin<Guid, TDbUser>
		where TDbUser : DataAccessObject
	{
		[ValueRequired]
		[BackReference]
		[Index]
		public abstract TDbUser User { get; set; }

		[PersistedMember]
		[Index(IndexName = "UserLogin_LoginProvider_ProviderKey_idx", Unique = true, CompositeOrder = 0)]
		public abstract string LoginProvider { get; set; }

		[PersistedMember]
		[Index(IndexName = "UserLogin_LoginProvider_ProviderKey_idx", Unique = true, CompositeOrder = 1)]
		public abstract string ProviderKey { get; set; }
	}
}