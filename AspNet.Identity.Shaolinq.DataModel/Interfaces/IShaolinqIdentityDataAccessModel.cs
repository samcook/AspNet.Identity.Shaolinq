﻿using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel.Interfaces
{
	public interface IShaolinqIdentityDataAccessModel<TPrimaryKey, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole>
		where TDbUser : DataAccessObject, IShaolinqIdentityDbUser<TPrimaryKey>
		where TDbUserLogin : DataAccessObject, IShaolinqIdentityDbUserLogin<TPrimaryKey, TDbUser>
		where TDbUserClaim : DataAccessObject, IShaolinqIdentityDbUserClaim<TPrimaryKey, TDbUser>
		where TDbUserRole : DataAccessObject, IShaolinqIdentityDbUserRole<TPrimaryKey, TDbUser>
	{
		DataAccessObjects<TDbUser> Users { get; }
		DataAccessObjects<TDbUserLogin> UserLogins { get; }
		DataAccessObjects<TDbUserClaim> UserClaims { get; }
		DataAccessObjects<TDbUserRole> UserRoles { get; }
	}
}