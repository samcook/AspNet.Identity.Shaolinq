using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Microsoft.AspNet.Identity;
using Shaolinq;

namespace AspNet.Identity.Shaolinq
{
	public class ShaolinqIdentityUserStore<TIdentityUser, TDataModel, TPrimaryKey, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole> :
		IUserPasswordStore<TIdentityUser, TPrimaryKey>,
		IUserLoginStore<TIdentityUser, TPrimaryKey>,
		IUserClaimStore<TIdentityUser, TPrimaryKey>,
		IUserSecurityStampStore<TIdentityUser, TPrimaryKey>,
		IUserRoleStore<TIdentityUser, TPrimaryKey>,
		IUserEmailStore<TIdentityUser, TPrimaryKey>

		//IQueryableUserStore<TIdentityUser, TPrimaryKey>,
		//IUserLockoutStore<TIdentityUser, TPrimaryKey>,
		//IUserPhoneNumberStore<TIdentityUser, TPrimaryKey>,
		//IUserTwoFactorStore<TIdentityUser, TPrimaryKey>

		where TIdentityUser : ShaolinqIdentityUser<TPrimaryKey>, new()
		where TDataModel : DataAccessModel, IShaolinqIdentityDataAccessModel<TPrimaryKey, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole>
		where TPrimaryKey : IEquatable<TPrimaryKey>
		where TDbUser : DataAccessObject, IShaolinqIdentityDbUser<TPrimaryKey>
		where TDbUserLogin : DataAccessObject, IShaolinqIdentityDbUserLogin<TPrimaryKey, TDbUser>
		where TDbUserClaim : DataAccessObject, IShaolinqIdentityDbUserClaim<TPrimaryKey, TDbUser>
		where TDbUserRole : DataAccessObject, IShaolinqIdentityDbUserRole<TPrimaryKey, TDbUser>
	{
		private readonly TDataModel dataModel;

		public ShaolinqIdentityUserStore(
			TDataModel dataModel
		)
		{
			this.dataModel = dataModel;
		}

		public Task CreateAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUser = dataModel.Users.Create();

				MapUser(user, dbUser);

				dbUser.ActivationDate = DateTime.UtcNow;

				scope.Flush(dataModel);
				scope.Complete();

				user.Id = dbUser.Id;
			}

			return Task.FromResult<object>(null);
		}

		public Task UpdateAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUser = dataModel.Users.SingleOrDefault(x => x.Id.Equals(user.Id));

				if (dbUser == null)
				{
					throw new ApplicationException("User not found");
				}

				MapUser(user, dbUser);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task DeleteAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				dataModel.Users.DeleteWhere(x => x.Id.Equals(user.Id));

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<TIdentityUser> FindByIdAsync(TPrimaryKey userId)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.Id.Equals(userId));

			return Task.FromResult(MapUser(dbUser));
		}

		public Task<TIdentityUser> FindByNameAsync(string userName)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.UserName == userName);

			return Task.FromResult(MapUser(dbUser));
		}

		public Task SetPasswordHashAsync(TIdentityUser user, string passwordHash)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.PasswordHash = passwordHash;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetPasswordHashAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		public Task AddLoginAsync(TIdentityUser user, UserLoginInfo login)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (login == null)
			{
				throw new ArgumentNullException("login");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUserLogin = dataModel.UserLogins.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserLogin.User = dbUser;
				dbUserLogin.LoginProvider = login.LoginProvider;
				dbUserLogin.ProviderKey = login.ProviderKey;

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task RemoveLoginAsync(TIdentityUser user, UserLoginInfo login)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (login == null)
			{
				throw new ArgumentNullException("login");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				dataModel.UserLogins.DeleteWhere(x => x.User.Id.Equals(user.Id) && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbUserLogins = dataModel.UserLogins.Where(x => x.User.Id.Equals(user.Id));

			IList<UserLoginInfo> userLogins = dbUserLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToList();

			return Task.FromResult(userLogins);
		}

		public Task<TIdentityUser> FindAsync(UserLoginInfo login)
		{
			if (login == null)
			{
				throw new ArgumentNullException("login");
			}

			var userLogin = dataModel.UserLogins.SingleOrDefault(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

			if (userLogin != null)
			{
				return Task.FromResult(MapUser(userLogin.User));
			}

			return Task.FromResult<TIdentityUser>(null);
		}

		public Task<IList<Claim>> GetClaimsAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbClaims = dataModel.UserClaims.Where(x => x.User.Id.Equals(user.Id));

			IList<Claim> claims = dbClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

			return Task.FromResult(claims);
		}

		public Task AddClaimAsync(TIdentityUser user, Claim claim)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (claim == null)
			{
				throw new ArgumentNullException("claim");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUserClaim = dataModel.UserClaims.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserClaim.User = dbUser;
				dbUserClaim.ClaimType = claim.Type;
				dbUserClaim.ClaimValue = claim.Value;

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task RemoveClaimAsync(TIdentityUser user, Claim claim)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (claim == null)
			{
				throw new ArgumentNullException("claim");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				dataModel.UserClaims.DeleteWhere(x => x.User.Id.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task SetSecurityStampAsync(TIdentityUser user, string stamp)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.SecurityStamp = stamp;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetSecurityStampAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.SecurityStamp);
		}

		public Task AddToRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUserRole = dataModel.UserRoles.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserRole.User = dbUser;
				dbUserRole.Role = roleName;

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task RemoveFromRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				dataModel.UserRoles.DeleteWhere(x => x.User.Id.Equals(user.Id) && x.Role == roleName);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<IList<string>> GetRolesAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbRoles = dataModel.UserRoles.Where(x => x.User.Id.Equals(user.Id));

			IList<string> roles = dbRoles.Select(x => x.Role).ToList();

			return Task.FromResult(roles);
		}

		public Task<bool> IsInRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbRole = dataModel.UserRoles.SingleOrDefault(x => x.User.Id.Equals(user.Id) && x.Role == roleName);

			return Task.FromResult(dbRole != null);
		}

		public Task SetEmailAsync(TIdentityUser user, string email)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.Email = email;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetEmailAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(TIdentityUser user, bool confirmed)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.EmailConfirmed = confirmed;

			return Task.FromResult<object>(null);
		}

		public Task<TIdentityUser> FindByEmailAsync(string email)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.Email == email);

			return Task.FromResult(MapUser(dbUser));
		}

		public void Dispose()
		{
		}

		private static void MapUser(TIdentityUser fromUser, TDbUser toUser)
		{
			fromUser.PopulateDbUser(toUser);
		}

		private static TIdentityUser MapUser(TDbUser dbUser)
		{
			if (dbUser == null)
			{
				return null;
			}

			var user = new TIdentityUser();

			user.PopulateFromDbUser(dbUser);

			return user;
		}
	}
}
