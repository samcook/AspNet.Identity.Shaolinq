using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.Shaolinq
{
	public class ShaolinqIdentityUser : IUser<Guid>, IPasswordHash
	{
		public Guid Id { get; internal set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public bool IsAnonymousUser { get; set; }

		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ShaolinqIdentityUser, Guid> manager, string authenticationType)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
			// Add custom user claims here
			return userIdentity;
		}
	}
}