using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel.Interfaces
{
	public interface IShaolinqIdentityDataAccessModel
	{
		DataAccessObjects<DbUser> Users { get; }
		DataAccessObjects<DbUserLogin> UserLogins { get; }
		DataAccessObjects<DbUserClaim> UserClaims { get; }
		DataAccessObjects<DbUserRole> UserRoles { get; } 
	}
}