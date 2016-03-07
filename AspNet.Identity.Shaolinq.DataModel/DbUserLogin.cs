using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserLogin")]
	public abstract class DbUserLogin : DataAccessObject<Guid>, IShaolinqIdentityDbUserLogin<Guid, DbUser>
	{
		[ValueRequired]
		[BackReference]
		[Index]
		public abstract DbUser User { get; set; }

		[PersistedMember]
		[Index(IndexName = "UserLogin_LoginProvider_ProviderKey_idx", Unique = true, CompositeOrder = 0)]
		public abstract string LoginProvider { get; set; }

		[PersistedMember]
		[Index(IndexName = "UserLogin_LoginProvider_ProviderKey_idx", Unique = true, CompositeOrder = 1)]
		public abstract string ProviderKey { get; set; }
	}
}