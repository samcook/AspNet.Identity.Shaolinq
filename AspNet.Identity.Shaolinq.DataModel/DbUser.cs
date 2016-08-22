using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "User")]
	public abstract class DbUser : DbUserBase<DbUser>, IShaolinqIdentityDbUser<Guid>
	{
	}

	[DataAccessObject(NotPersisted = true)]
	public abstract class DbUserBase<TDbUser> : DataAccessObject<Guid>
		where TDbUser : DataAccessObject
	{
		[Index(Unique = true)]
		[PersistedMember]
		public abstract string UserName { get; set; }

		[PersistedMember]
		public abstract string Name { get; set; }

		[PersistedMember]
		[Index]
		public abstract string Email { get; set; }

		[PersistedMember]
		public abstract bool EmailConfirmed { get; set; }

		[PersistedMember]
		public abstract string PasswordHash { get; set; }

		[PersistedMember]
		public abstract string SecurityStamp { get; set; }

		[PersistedMember]
		public abstract bool IsAnonymousUser { get; set; }

		[PersistedMember]
		public abstract DateTime ActivationDate { get; set; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserLogin<TDbUser>> UserLogins { get; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserClaim<TDbUser>> UserClaims { get; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserRole<TDbUser>> UserRoles { get; }
	}
}