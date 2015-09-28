using System;

namespace AspNet.Identity.Shaolinq.DataModel.Interfaces
{
	public interface IShaolinqIdentityDbUser<TPrimaryKey>
	{
		TPrimaryKey Id { get; set; }
		string UserName { get; set; }
		string Name { get; set; }
		string Email { get; set; }
		bool EmailConfirmed { get; set; }
		string PasswordHash { get; set; }
		string SecurityStamp { get; set; }
		bool IsAnonymousUser { get; set; }
		DateTime ActivationDate { get; set; }
	}
}