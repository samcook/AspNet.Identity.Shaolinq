using System;
using AspNet.Identity.Shaolinq.DataModel.Interfaces;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserRole")]
	public abstract class DbUserRole : DataAccessObject<Guid>, IShaolinqIdentityDbUserRole<Guid, DbUser>
	{
		[ValueRequired]
		[BackReference]
		[Index]
		[Index(IndexName = "UserRole_User_Role_idx", CompositeOrder = 0, Unique = true)]
		public abstract DbUser User { get; set; }

		[ValueRequired]
		[PersistedMember]
		[Index(IndexName = "UserRole_User_Role_idx", CompositeOrder = 1, Unique = true)]
		public abstract string Role { get; set; }
	}
}