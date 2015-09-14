using System;
using Platform.Validation;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.DataModel
{
	[DataAccessObject(Name = "UserRole")]
	public abstract class DbUserRole : DataAccessObject<Guid>
	{
		[ValueRequired]
		[BackReference]
		[Index(IndexName = "User_Role_Idx", CompositeOrder = 0, Unique = true)]
		public abstract DbUser User { get; set; }

		[ValueRequired]
		[PersistedMember]
		[Index(IndexName = "User_Role_Idx", CompositeOrder = 1, Unique = true)]
		public abstract string Role { get; set; }
	}
}