# AspNet.Identity.Shaolinq

A [Shaolinq](https://github.com/tumtumtum/Shaolinq) provider for AspNet Identity.

Available on [NuGet](https://www.nuget.org/packages/AspNet.Identity.Shaolinq/).

## Usage

#### Define your Identity User class and matching Shaolinq DataAccessObjects (optional)

If you are happy with the default set of user fields, you can skip this step and use the supplied `DbUser`, `DbUserClaim`, `DbUserLogin`, `DbUserRole` and `ShaolinqIdentityUser` classes.

Alternatively:

- Create Shaolinq `DataAccessObject` classes derived from
  - `IShaolinqIdentityDbUser`
  - `IShaolinqIdentityDbUserClaim`
  - `IShaolinqIdentityDbUserLogin`
  - `IShaolinqIdentityDbUserRole`

- Create an Identity User class derived from `ShaolinqIdentityUser`
  - Override the `PopulateDbUser` and `PopulateFromDbUser` methods to map your additional fields between Shaolinq and Identity User classes

#### Define your Shaolinq DataAccessModel
Create a Shaolinq `DataAccessModel` class that implements `IShaolinqIdentityDataAccessModel`.

(replacing generic type parameters with your types if applicable)
```csharp
[DataAccessModel]
public abstract class MyDataAccessModel :
	DataAccessModel,
	IShaolinqIdentityDataAccessModel<Guid, DbUser, DbUserLogin, DbUserClaim, DbUserRole>
{
	[DataAccessObjects]
	public abstract DataAccessObjects<DbUser> Users { get; }
	[DataAccessObjects]
	public abstract DataAccessObjects<DbUserLogin> UserLogins { get; }
	[DataAccessObjects]
	public abstract DataAccessObjects<DbUserClaim> UserClaims { get; }
	[DataAccessObjects]
	public abstract DataAccessObjects<DbUserRole> UserRoles { get; }
}
```

#### Instantiate a UserManager

(replacing generic type parameters with your types if applicable)
```csharp
var dataModel = DataAccessModel.BuildDataAccessModel<MyDataAccessModel>();
var userStore = new ShaolinqIdentityUserStore<
                      ShaolinqIdentityUser<Guid>,
                      MyDataAccessModel,
                      Guid,
                      DbUser,
                      DbUserLogin,
                      DbUserClaim,
                      DbUserRole>(dataModel)
var userManager = new UserManager<ShaolinqIdentityUser<Guid>, Guid>();
```
