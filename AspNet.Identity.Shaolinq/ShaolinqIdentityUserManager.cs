using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Shaolinq;

namespace AspNet.Identity.Shaolinq
{
	public class ShaolinqIdentityUserManager : UserManager<ShaolinqIdentityUser, Guid>
	{
		public ShaolinqIdentityUserManager(IUserStore<ShaolinqIdentityUser, Guid> store)
			: base(store)
		{
		}

		public static ShaolinqIdentityUserManager Create<TDataModel>(
			IdentityFactoryOptions<ShaolinqIdentityUserManager> options,
			IOwinContext context,
			TDataModel dataModel)
			where TDataModel : DataAccessModel, IShaolinqIdentityDataAccessModel
		{
			var manager = new ShaolinqIdentityUserManager(new ShaolinqIdentityUserStore<TDataModel>(dataModel));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ShaolinqIdentityUser, Guid>(manager)
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
				manager.UserTokenProvider = new DataProtectorTokenProvider<ShaolinqIdentityUser, Guid>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}
}