# IUserRepository.InsertAsync method

Insert a new user.

```csharp
public Task InsertAsync(User user, CancellationToken cancellationToken = default(CancellationToken))
```

| parameter | description |
| --- | --- |
| user | The user to insert. |
| cancellationToken |  |

## Exceptions

| exception | condition |
| --- | --- |
| [ExistsException](../../CosmosTest.CommonExceptions/ExistsException.md) | Thrown if a conflicting record already exists. |
| [RepositoryException](../../CosmosTest.CosmosUtility/RepositoryException.md) | Thrown if a general failure occurs talking to the repository. |
| ValidationException | Thrown if the user object does not validate correctly. |

## See Also

* class [User](../../CosmosTest.DomainModels/User.md)
* interface [IUserRepository](../IUserRepository.md)
* namespace [CosmosTest](../../CosmosTest.md)

<!-- DO NOT EDIT: generated by xmldocmd for CosmosTest.dll -->