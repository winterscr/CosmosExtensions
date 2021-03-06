# CosmosExtensions.WhereCommon&lt;T&gt; method

Include common predicates where the type of the entity matches and the version is at most as specified.

```csharp
public static IQueryable<T> WhereCommon<T>(this IQueryable<T> source, string type, 
    int versionAtMost)
    where T : CosmosEntity
```

| parameter | description |
| --- | --- |
| T |  |
| source |  |
| type |  |
| versionAtMost |  |

## See Also

* class [CosmosEntity](../CosmosEntity.md)
* class [CosmosExtensions](../CosmosExtensions.md)
* namespace [CosmosTest.CosmosUtility](../../CosmosTest.md)

<!-- DO NOT EDIT: generated by xmldocmd for CosmosTest.dll -->
