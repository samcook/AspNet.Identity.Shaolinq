using System;
using AspNet.Identity.Shaolinq.DataModel;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.Tests
{
	[DataAccessModel]
	public abstract class TestDataAccessModel :
		DataAccessModel,
		IShaolinqIdentityDataAccessModel<Guid, DbUser, DbUserLogin<DbUser>, DbUserClaim<DbUser>, DbUserRole<DbUser>>
	{
		[DataAccessObjects]
		public abstract DataAccessObjects<DbUser> Users { get; }

		[DataAccessObjects]
		public abstract DataAccessObjects<DbUserLogin<DbUser>> UserLogins { get; }

		[DataAccessObjects]
		public abstract DataAccessObjects<DbUserClaim<DbUser>> UserClaims { get; }

		[DataAccessObjects]
		public abstract DataAccessObjects<DbUserRole<DbUser>> UserRoles { get; }
	}
}
