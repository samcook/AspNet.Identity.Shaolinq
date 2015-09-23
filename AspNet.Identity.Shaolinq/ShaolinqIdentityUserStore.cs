using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.Shaolinq.DataModel;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Microsoft.AspNet.Identity;
using Shaolinq;

namespace AspNet.Identity.Shaolinq
{

	public class ShaolinqIdentityUserStore<TDataModel> :
		IUserPasswordStore<ShaolinqIdentityUser, Guid>,
		IUserLoginStore<ShaolinqIdentityUser, Guid>,
		IUserClaimStore<ShaolinqIdentityUser, Guid>,
		IUserSecurityStampStore<ShaolinqIdentityUser, Guid>,
		IUserRoleStore<ShaolinqIdentityUser, Guid>,
		IUserEmailStore<ShaolinqIdentityUser, Guid>

		//IQueryableUserStore<ShaolinqIdentityUser, Guid>,
		//IUserLockoutStore<ShaolinqIdentityUser, Guid>,
		//IUserPhoneNumberStore<ShaolinqIdentityUser, Guid>,
		//IUserTwoFactorStore<ShaolinqIdentityUser, Guid>

		where TDataModel : DataAccessModel, IShaolinqIdentityDataAccessModel
	{
		private readonly TDataModel dataModel;

		public ShaolinqIdentityUserStore(
			TDataModel dataModel
		)
		{
			this.dataModel = dataModel;
		}

		public Task CreateAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUser = dataModel.Users.Create();

				dbUser.Id = Guid.NewGuid();
				dbUser.ActivationDate = DateTime.UtcNow;

				MapUser(user, dbUser);

				scope.Flush(dataModel);
				scope.Complete();

				user.Id = dbUser.Id;
			}

			return Task.FromResult<object>(null);
		}

		public Task UpdateAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				var dbUser = dataModel.Users.SingleOrDefault(x => x.Id == user.Id);

				if (dbUser == null)
				{
					throw new ApplicationException("User not found");
				}

				MapUser(user, dbUser);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task DeleteAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			using (var scope = TransactionScopeFactory.CreateReadCommitted())
			{
				dataModel.Users.DeleteWhere(x => x.Id == user.Id);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<ShaolinqIdentityUser> FindByIdAsync(Guid userId)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.Id == userId);

			return Task.FromResult(MapUser(dbUser));
		}

		public Task<ShaolinqIdentityUser> FindByNameAsync(string userName)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.UserName == userName);

			return Task.FromResult(MapUser(dbUser));
		}

		public Task SetPasswordHashAsync(ShaolinqIdentityUser user, string passwordHash)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.PasswordHash = passwordHash;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetPasswordHashAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
		}

		public Task AddLoginAsync(ShaolinqIdentityUser user, UserLoginInfo login)
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

		public Task RemoveLoginAsync(ShaolinqIdentityUser user, UserLoginInfo login)
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
				dataModel.UserLogins.DeleteWhere(x => x.User.Id == user.Id && x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbUserLogins = dataModel.UserLogins.Where(x => x.User.Id == user.Id);

			IList<UserLoginInfo> userLogins = dbUserLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToList();

			return Task.FromResult(userLogins);
		}

		public Task<ShaolinqIdentityUser> FindAsync(UserLoginInfo login)
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

			return Task.FromResult<ShaolinqIdentityUser>(null);
		}

		public Task<IList<Claim>> GetClaimsAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbClaims = dataModel.UserClaims.Where(x => x.User.Id == user.Id);

			IList<Claim> claims = dbClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();

			return Task.FromResult(claims);
		}

		public Task AddClaimAsync(ShaolinqIdentityUser user, Claim claim)
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

		public Task RemoveClaimAsync(ShaolinqIdentityUser user, Claim claim)
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
				dataModel.UserClaims.DeleteWhere(x => x.User.Id == user.Id && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task SetSecurityStampAsync(ShaolinqIdentityUser user, string stamp)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.SecurityStamp = stamp;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetSecurityStampAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.SecurityStamp);
		}

		public Task AddToRoleAsync(ShaolinqIdentityUser user, string roleName)
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

		public Task RemoveFromRoleAsync(ShaolinqIdentityUser user, string roleName)
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
				dataModel.UserRoles.DeleteWhere(x => x.User.Id == user.Id && x.Role == roleName);

				scope.Complete();
			}

			return Task.FromResult<object>(null);
		}

		public Task<IList<string>> GetRolesAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbRoles = dataModel.UserRoles.Where(x => x.User.Id == user.Id);

			IList<string> roles = dbRoles.Select(x => x.Role).ToList();

			return Task.FromResult(roles);
		}

		public Task<bool> IsInRoleAsync(ShaolinqIdentityUser user, string roleName)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			var dbRole = dataModel.UserRoles.SingleOrDefault(x => x.User.Id == user.Id && x.Role == roleName);

			return Task.FromResult(dbRole != null);
		}

		public Task SetEmailAsync(ShaolinqIdentityUser user, string email)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.Email = email;

			return Task.FromResult<object>(null);
		}

		public Task<string> GetEmailAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(ShaolinqIdentityUser user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			return Task.FromResult(user.EmailConfirmed);
		}

		public Task SetEmailConfirmedAsync(ShaolinqIdentityUser user, bool confirmed)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			user.EmailConfirmed = confirmed;

			return Task.FromResult<object>(null);
		}

		public Task<ShaolinqIdentityUser> FindByEmailAsync(string email)
		{
			var dbUser = dataModel.Users.SingleOrDefault(x => x.Email == email);

			return Task.FromResult(MapUser(dbUser));
		}

		public void Dispose()
		{
		}

		private static void MapUser(ShaolinqIdentityUser fromUser, DbUser toUser)
		{
			toUser.UserName = fromUser.UserName;
			toUser.Email = fromUser.Email;
			toUser.EmailConfirmed = fromUser.EmailConfirmed;
			toUser.PasswordHash = fromUser.PasswordHash;
			toUser.SecurityStamp = fromUser.SecurityStamp;
			toUser.IsAnonymousUser = fromUser.IsAnonymousUser;
		}

		private static ShaolinqIdentityUser MapUser(DbUser dbUser)
		{
			if (dbUser == null)
			{
				return null;
			}

			var user = new ShaolinqIdentityUser
			{
				Id = dbUser.Id,
				UserName = dbUser.UserName,
				Email = dbUser.Email,
				EmailConfirmed = dbUser.EmailConfirmed,
				PasswordHash = dbUser.PasswordHash,
				SecurityStamp = dbUser.SecurityStamp,
				IsAnonymousUser = dbUser.IsAnonymousUser
			};

			return user;
		}
	}
}
