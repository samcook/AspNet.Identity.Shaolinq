using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserClaim")]
	public abstract class DbUserClaim<TDbUser> : DataAccessObject<Guid>, IShaolinqIdentityDbUserClaim<Guid, TDbUser>
		where TDbUser : DataAccessObject
	{
		[ValueRequired]
		[BackReference]
		[Index]
		public abstract TDbUser User { get; set; }

		[PersistedMember]
		public abstract string ClaimType { get; set; }

		[PersistedMember]
		public abstract string ClaimValue { get; set; }
	}
}