using System;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "User")]
	public abstract class DbUser : DataAccessObject<Guid>
	{
		[Index(Unique = true)]
		[PersistedMember]
		public abstract string UserName { get; set; }

		[PersistedMember]
		public abstract string Name { get; set; }

		[PersistedMember]
		public abstract string Email { get; set; }

		[PersistedMember]
		public abstract string PasswordHash { get; set; }

		[PersistedMember]
		public abstract string SecurityStamp { get; set; }

		[PersistedMember]
		public abstract bool IsAnonymousUser { get; set; }

		[PersistedMember]
		public abstract DateTime ActivationDate { get; set; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserLogin> UserLogins { get; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserClaim> UserClaims { get; }

		[RelatedDataAccessObjects]
		public abstract RelatedDataAccessObjects<DbUserRole> UserRoles { get; }
	}
}