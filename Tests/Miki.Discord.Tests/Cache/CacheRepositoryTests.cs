using Miki.Cache;
using Miki.Cache.InMemory;
using Miki.Discord.Internal.Repositories;
using Miki.Serialization.Protobuf;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Miki.Discord.Tests.Cache
{
    class TestCache : BaseCacheRepository<string>
    {
        public TestCache(IExtendedCacheClient cacheClient)
            : base(cacheClient)
        {
        }

        protected override string GetCacheKey(string value)
            => value;

        protected override ValueTask<string> GetFromApiAsync(params object[] id)
            => new ValueTask<string>(id[0].ToString());

        protected override ValueTask<string> GetFromCacheAsync(params object[] id)
            => new ValueTask<string>(id[0].ToString());

        protected override string GetMemberKey(string value)
            => value;
    }

    public class CacheRepositoryTests
    {
        [Fact]
        public async Task GetAsync_FetchesFromCache()
        {
            var cacheMock = new Mock<IExtendedCacheClient>();
            cacheMock.Setup(x => x.GetAsync<string>(It.IsAny<string>()))
                .Returns<string>(x => Task.FromResult(x));

            var cache = new TestCache(cacheMock.Object);

            var response = await cache.GetAsync("12");

            Assert.Equal("12", response);
        }
    }
}
