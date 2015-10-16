using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserClaim")]
	public abstract class DbUserClaim : DataAccessObject<Guid>, IShaolinqIdentityDbUserClaim<Guid, DbUser>
	{
		[ValueRequired]
		[BackReference]
		[Index]
		public abstract DbUser User { get; set; }

		[PersistedMember]
		public abstract string ClaimType { get; set; }

		[PersistedMember]
		public abstract string ClaimValue { get; set; }
	}
}