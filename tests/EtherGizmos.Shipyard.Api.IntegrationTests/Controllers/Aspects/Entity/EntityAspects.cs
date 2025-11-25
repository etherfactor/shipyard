using EtherGizmos.Shipyard.Api.IntegrationTests.Abstractions;

namespace EtherGizmos.Shipyard.Api.IntegrationTests.Controllers.Aspects.Entity;

public static class EntityAspects
{
    private static class Generic<TEntity, TId>
        where TEntity : class, new()
    {
        public static IEnumerable<IAspect<TEntity, TId>> All 
            =>
            [
                new SearchAuthAspect<TEntity, TId>(),
                new SearchGroupFilterAspect<TEntity, TId>(),
                new SearchConformanceAspect<TEntity, TId>(),
                new SearchSelectOptionAspect<TEntity, TId>(),

                new GetAuthAspect<TEntity, TId>(),
                new GetConformanceAspect<TEntity, TId>(),
                new GetGroupFilterAspect<TEntity, TId>(),
                new GetRecordNotFoundAspect<TEntity, TId>(),
                new GetSelectOptionAspect<TEntity, TId>(),

                new CreateAuthAspect<TEntity, TId>(),
                new CreateConformanceAspect<TEntity, TId>(),
                new CreateSelectOptionAspect<TEntity, TId>(),

                new PatchAuthAspect<TEntity, TId>(),
                new PatchConformanceAspect<TEntity, TId>(),
                new PatchGroupFilterAspect<TEntity, TId>(),
                new PatchRecordNotFoundAspect<TEntity, TId>(),
                new PatchSelectOptionAspect<TEntity, TId>(),

                new DeleteAuthAspect<TEntity, TId>(),
                new DeleteConformanceAspect<TEntity, TId>(),
                new DeleteGroupFilterAspect<TEntity, TId>(),
                new DeleteRecordNotFoundAspect<TEntity, TId>(),
            ];
    }

    public static IEnumerable<AspectCase> BuildAll<TEntity, TId>(
        IODataResourceSpec<TEntity, TId> specification)
        where TEntity : class, new()
    {
        foreach (var aspect in Generic<TEntity, TId>.All)
        {
            foreach (var test in aspect.Build(specification))
            {
                yield return test;
            }
        }
    }
}
