namespace AspNet.Identity.Shaolinq.DataModel.Interfaces
{
	public interface IShaolinqIdentityDbUserRole<TPrimaryKey, TDbUser>
	{
		TPrimaryKey Id { get; set; }
		TDbUser User { get; set; }
		string Role { get; set; }
	}
}