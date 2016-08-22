using NUnit.Framework;
using Shaolinq;

namespace AspNet.Identity.Shaolinq.Tests
{
	[TestFixture]
	public class ShaolinqIdentityUserManagerTests
	{
		[Test]
		public void TestFoo()
		{
			var dataModel = DataAccessModel.BuildDataAccessModel<TestDataAccessModel>();

			dataModel.Create(DatabaseCreationOptions.IfDatabaseNotExist);

			using (var scope = new DataAccessScope())
			{
				var dbUser = dataModel.Users.Create();

				var dbUserClaim = dataModel.UserClaims.Create();

				dbUserClaim.User = dbUser;

				scope.Complete();
			}
		}
	}
}