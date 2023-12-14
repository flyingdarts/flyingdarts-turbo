using Flyingdarts.Persistence;

namespace Flyingdarts.Backend.Shared.Services
{
    public abstract class MetadataService<T> where T : IGameState
    {
        private readonly CachingService<T> _cachingService;

        protected MetadataService(CachingService<T> cachingService)
        {
            _cachingService = cachingService;
        }
    }
}
