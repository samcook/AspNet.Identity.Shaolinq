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
		public abstract DbUser User { get; set; }

		[PersistedMember]
		public abstract string LoginProvider { get; set; }

		[PersistedMember]
		public abstract string ProviderKey { get; set; }
	}
}