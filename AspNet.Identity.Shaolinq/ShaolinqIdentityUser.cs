using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Microsoft.AspNet.Identity;

namespace AspNet.Identity.Shaolinq
{
	public class ShaolinqIdentityUser<TKey> : IUser<TKey>
		where TKey : IEquatable<TKey>
	{
		public TKey Id { get; internal set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public bool IsAnonymousUser { get; set; }
		public bool IsLockoutEnabled { get; set; }
		public int AccessFailedCount { get; set; }
		public DateTimeOffset LockoutEndDate { get; set; }

		public virtual void PopulateFromDbUser(IShaolinqIdentityDbUser<TKey> dbUser)
		{
			if (dbUser == null)
			{
				return;
			}

			Id = dbUser.Id;
			UserName = dbUser.UserName;
			Email = dbUser.Email;
			EmailConfirmed = dbUser.EmailConfirmed;
			PasswordHash = dbUser.PasswordHash;
			SecurityStamp = dbUser.SecurityStamp;
			IsAnonymousUser = dbUser.IsAnonymousUser;
			IsLockoutEnabled = dbUser.IsLockoutEnabled;
			AccessFailedCount = dbUser.AccessFailedCount;
			LockoutEndDate = dbUser.LockoutEndDate ?? DateTimeOffset.MinValue;
		}

		public virtual void PopulateDbUser(IShaolinqIdentityDbUser<TKey> toUser)
		{
			toUser.UserName = UserName;
			toUser.Email = Email;
			toUser.EmailConfirmed = EmailConfirmed;
			toUser.PasswordHash = PasswordHash;
			toUser.SecurityStamp = SecurityStamp;
			toUser.IsAnonymousUser = IsAnonymousUser;
			toUser.IsLockoutEnabled = IsLockoutEnabled;
			toUser.AccessFailedCount = AccessFailedCount;
			toUser.LockoutEndDate = LockoutEndDate == DateTimeOffset.MinValue ? (DateTime?) null : LockoutEndDate.UtcDateTime;
		}
	}
}