using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Miki.Cache;
using Miki.Logging;
using Miki.Patterns.Repositories;

namespace Miki.Discord.Internal.Repositories
{
    public abstract class BaseCacheRepository<T> : IAsyncRepository<T>
    {
        protected readonly IExtendedCacheClient cacheClient;

        protected BaseCacheRepository(IExtendedCacheClient cacheClient)
        {
            this.cacheClient = cacheClient;
        }

        /// <summary>
        /// Gets the cache key for this entity.
        /// </summary>
        protected abstract string GetCacheKey(T value);

        /// <summary>
        /// Gets the inner hash key for this entity.
        /// </summary>
        protected abstract string GetMemberKey(T value);

        /// <summary>
        /// Get entity from cache operation.
        /// </summary>
        protected abstract ValueTask<T> GetFromCacheAsync(params object[] id);

        /// <summary>
        /// Get entity from api operation. Gets called when the object does not exist in cache.
        /// </summary>
        protected abstract ValueTask<T> GetFromApiAsync(params object[] id);

        /// <inheritdoc />
        public async ValueTask<T> GetAsync(params object[] id)
        {
            var t = await GetFromCacheAsync(id);
            if (t == null)
            {
                t = await GetFromApiAsync(id);
                if (t != null)
                {
                    await AddAsync(t);
                }
            }

            return t;
        }
        
        /// <inheritdoc />
        public ValueTask<IEnumerable<T>> ListAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public async ValueTask<T> AddAsync(T entity)
        {
            Log.Debug($"Pushing {typeof(T).Name} to cache as {GetCacheKey(entity)} - {GetMemberKey(entity)}");

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await cacheClient.HashUpsertAsync(
                GetCacheKey(entity), GetMemberKey(entity), entity);
            return entity;
        }

        /// <inheritdoc />
        public ValueTask<IEnumerable<T>> AddAsync(IEnumerable<T> entities)
        {
            return BulkAddAsync(entities);
        }

        /// <inheritdoc />
        public async ValueTask EditAsync(T entity)
        {
            await AddAsync(entity);
        }

        /// <inheritdoc />
        public async ValueTask EditAsync(IEnumerable<T> entities)
        {
            await AddAsync(entities);
        }

        /// <inheritdoc />
        public async ValueTask DeleteAsync(T entity)
        {
            await cacheClient.HashDeleteAsync(
                GetCacheKey(entity), GetMemberKey(entity));
        }

        /// <inheritdoc />
        public ValueTask DeleteAsync(IEnumerable<T> entities)
        {
            return BulkDeleteAsync(entities);
        }
        
        private static T[] ValidateEntities(IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var members = values as T[] ?? values.ToArray();
            if (!members.Any())
            {
                throw new ArgumentNullException(nameof(values));
            }

            return members;
        }
        
        private async ValueTask<IEnumerable<T>> BulkAddAsync(IEnumerable<T> values)
        {
            var members = ValidateEntities(values);
            foreach (var m in members)
            {
                Log.Debug($"Pushing {typeof(T).Name} to cache as {GetCacheKey(m)} - {GetMemberKey(m)}");
            }

            await cacheClient.HashUpsertAsync(
                GetCacheKey(members.First()), 
                members.Select(x => new KeyValuePair<string,T>(GetMemberKey(x), x)));
            return members;
        }
        
        private async ValueTask BulkDeleteAsync(IEnumerable<T> values)
        {
            var members = ValidateEntities(values);
            await cacheClient.HashDeleteAsync(
                GetCacheKey(members.First()), 
                members.Select(GetMemberKey));
        }
    }
}