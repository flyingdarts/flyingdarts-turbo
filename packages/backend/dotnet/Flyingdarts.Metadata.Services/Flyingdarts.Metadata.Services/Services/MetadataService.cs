namespace Flyingdarts.Metadata.Services.Services;

public abstract class MetadataService<T>
    where T : IGameState<T>
{
    private readonly CachingService<T> _cachingService;

    protected MetadataService(CachingService<T> cachingService)
    {
        _cachingService = cachingService;
    }
}
