using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Shaolinq;

namespace AspNet.Identity.Shaolinq
{
	public class ShaolinqIdentityUserManager<TIdentityUser, TKey> : UserManager<TIdentityUser, TKey>
		where TIdentityUser : ShaolinqIdentityUser<TKey>, new()
		where TKey : IEquatable<TKey>
	{
		public ShaolinqIdentityUserManager(IUserStore<TIdentityUser, TKey> store)
			: base(store)
		{
		}

		public static ShaolinqIdentityUserManager<TIdentityUser, TKey> Create<TDataModel, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole>(
			IdentityFactoryOptions<ShaolinqIdentityUserManager<TIdentityUser, TKey>> options,
			IOwinContext context,
			TDataModel dataModel)
			where TDataModel : DataAccessModel, IShaolinqIdentityDataAccessModel<TKey, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole>
			where TDbUser : DataAccessObject, IShaolinqIdentityDbUser<TKey>
			where TDbUserLogin : DataAccessObject, IShaolinqIdentityDbUserLogin<TKey, TDbUser>
			where TDbUserClaim : DataAccessObject, IShaolinqIdentityDbUserClaim<TKey, TDbUser>
			where TDbUserRole : DataAccessObject, IShaolinqIdentityDbUserRole<TKey, TDbUser>
		{
			var manager = new ShaolinqIdentityUserManager<TIdentityUser, TKey>(new ShaolinqIdentityUserStore<TIdentityUser, TDataModel, TKey, TDbUser, TDbUserLogin, TDbUserClaim, TDbUserRole>(dataModel));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<TIdentityUser, TKey>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};
			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				//RequireNonLetterOrDigit = true,
				//RequireDigit = true,
				//RequireLowercase = true,
				//RequireUppercase = true,
			};
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider = new DataProtectorTokenProvider<TIdentityUser, TKey>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}
}