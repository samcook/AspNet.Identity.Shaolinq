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

		public async Task CreateAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbUser = dataModel.Users.Create();

				MapUser(user, dbUser);

				dbUser.ActivationDate = DateTime.UtcNow;

				await scope.FlushAsync();

                user.Id = dbUser.Id;

                await scope.CompleteAsync();
			}
		}

		public async Task UpdateAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbUser = await dataModel.Users.SingleOrDefaultAsync(x => x.Id.Equals(user.Id));

				if (dbUser == null)
				{
					throw new ApplicationException("User not found");
				}

				MapUser(user, dbUser);

				await scope.CompleteAsync();
			}
		}

		public async Task DeleteAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				await dataModel.Users.DeleteAsync(x => x.Id.Equals(user.Id));

				await scope.CompleteAsync();
			}
		}

		public async Task<TIdentityUser> FindByIdAsync(TPrimaryKey userId)
		{
			var dbUser = await dataModel.Users.SingleOrDefaultAsync(x => x.Id.Equals(userId));

		    return MapUser(dbUser);
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
				throw new ArgumentNullException(nameof(user));
			}

			user.PasswordHash = passwordHash;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetPasswordHashAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		public async Task AddLoginAsync(TIdentityUser user, UserLoginInfo login)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (login == null)
			{
				throw new ArgumentNullException(nameof(login));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbUserLogin = dataModel.UserLogins.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserLogin.User = dbUser;
				dbUserLogin.LoginProvider = login.LoginProvider;
				dbUserLogin.ProviderKey = login.ProviderKey;

				await scope.CompleteAsync();
			}
		}

		public async Task RemoveLoginAsync(TIdentityUser user, UserLoginInfo login)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (login == null)
			{
				throw new ArgumentNullException(nameof(login));
			}

			using (var scope = DataAccessScope.CreateReadUncommited())
			{
				await dataModel.UserLogins.DeleteAsync(x => x.User.Id.Equals(user.Id) && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

				await scope.CompleteAsync();
			}
		}

        public async Task<IList<UserLoginInfo>> GetLoginsAsync(TIdentityUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var dbUserLogins = dataModel.UserLogins.Where(x => x.User.Id.Equals(user.Id));

            var userLogins = await dbUserLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToListAsync();

            return userLogins;
        }
        
		public async Task<TIdentityUser> FindAsync(UserLoginInfo login)
		{
			if (login == null)
			{
				throw new ArgumentNullException(nameof(login));
			}

			var userLogin = await dataModel.UserLogins.SingleOrDefaultAsync(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

			if (userLogin != null)
			{
			    return MapUser(userLogin.User);
			}

            return null;
		}

		public async Task<IList<Claim>> GetClaimsAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var dbClaims = dataModel.UserClaims.Where(x => x.User.Id.Equals(user.Id));

			var claims = await dbClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToListAsync();

		    return claims;
		}

		public async Task AddClaimAsync(TIdentityUser user, Claim claim)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (claim == null)
			{
				throw new ArgumentNullException(nameof(claim));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbUserClaim = dataModel.UserClaims.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserClaim.User = dbUser;
				dbUserClaim.ClaimType = claim.Type;
				dbUserClaim.ClaimValue = claim.Value;

				await scope.CompleteAsync();
			}
		}

		public async Task RemoveClaimAsync(TIdentityUser user, Claim claim)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (claim == null)
			{
				throw new ArgumentNullException(nameof(claim));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				await dataModel.UserClaims.DeleteAsync(x => x.User.Id.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

				await scope.CompleteAsync();
			}
		}

		public Task SetSecurityStampAsync(TIdentityUser user, string stamp)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.SecurityStamp = stamp;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetSecurityStampAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.SecurityStamp);
		}

		public async Task AddToRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException(nameof(roleName));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				var dbUserRole = dataModel.UserRoles.Create();
				var dbUser = dataModel.Users.GetReference(user.Id);

				dbUserRole.User = dbUser;
				dbUserRole.Role = roleName;

				await scope.CompleteAsync();
			}
		}

		public async Task RemoveFromRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException(nameof(roleName));
			}

			using (var scope = DataAccessScope.CreateReadCommitted())
			{
				await dataModel.UserRoles.DeleteAsync(x => x.User.Id.Equals(user.Id) && x.Role == roleName);

				await scope.CompleteAsync();
			}
		}

		public async Task<IList<string>> GetRolesAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			var dbRoles = dataModel.UserRoles.Where(x => x.User.Id.Equals(user.Id));

			return await dbRoles.Select(x => x.Role).ToListAsync();
		}

		public async Task<bool> IsInRoleAsync(TIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return (await dataModel.UserRoles.SingleOrDefaultAsync(x => x.User.Id.Equals(user.Id) && x.Role == roleName)) != null;
		}

		public Task SetEmailAsync(TIdentityUser user, string email)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.Email = email;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetEmailAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(TIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(TIdentityUser user, bool confirmed)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.EmailConfirmed = confirmed;

			return Task.FromResult<object>(null);
		}

		public async Task<TIdentityUser> FindByEmailAsync(string email)
		{
		    return MapUser(await dataModel.Users.SingleOrDefaultAsync(x => x.Email == email));
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
