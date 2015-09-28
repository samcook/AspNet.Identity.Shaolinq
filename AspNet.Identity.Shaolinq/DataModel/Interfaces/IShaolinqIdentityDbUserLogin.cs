namespace AspNet.Identity.Shaolinq.DataModel.Interfaces
{
	public interface IShaolinqIdentityDbUserLogin<TPrimaryKey, TDbUser>
	{
		TPrimaryKey Id { get; set; }
		TDbUser User { get; set; }
		string LoginProvider { get; set; }
		string ProviderKey { get; set; }
	}
}