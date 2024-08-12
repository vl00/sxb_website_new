using StackExchange.Redis;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Utils;

namespace Sxb.WenDa.API.Extensions
{
    internal static class XxxQueryExtensions
    {
        /// <summary>
        /// 根据 ids和nos (可能无序)查items, 优先查cache后查db <br/>
        /// <br/>
        /// <code><![CDATA[
        /// // eg:
        /// var ls_res = await _easyRedisClient.GetItemsByIdsOrNos(ids, nos,
        ///     (batch, no) => batch.StringGetAsync(string.Format(CacheKeys.SubjectNo2Id, no)).ContinueWith(t => (no, Guid.TryParse(t.Result.ToString(), out var _gid) ? _gid : default)),
        ///     (batch, id) => batch.StringGetAsync(string.Format(CacheKeys.Subject, id)).ContinueWith(t => (id, t.Result.ToString().JsonStrTo<SubjectDbDto>())),
        ///     (ids2, nos2) => _subjectRepository.GetSubjects(ids2, nos2),
        ///     (tasks, batch, item) => 
        ///     {
        ///         tasks.Add(batch.StringSetAsync(string.Format(CacheKeys.SubjectNo2Id, item.No), item.Id.ToString(), TimeSpan.FromSeconds(60 * 60 * 24)));
        ///         tasks.Add(batch.StringSetAsync(string.Format(CacheKeys.Subject, item.Id), item.ToJsonStr(true), TimeSpan.FromSeconds(60 * 60 * 1)));
        ///     },
        ///     (item) => item.Id
        /// );         
        /// ]]></code>
        /// </summary>
        public static async Task<IEnumerable<TItem>> GetItemsByIdsOrNos<TItem>(this IEasyRedisClient easyRedisClient, IEnumerable<Guid> ids, IEnumerable<long> nos,
            Func<IBatch, long, Task<(long, Guid)>> funcGetNo2IdFromCache,
            Func<IBatch, Guid, Task<(Guid, TItem)>> funcGetItemFromCache,
            Func<IEnumerable<Guid>, IEnumerable<long>, Task<IEnumerable<TItem>>> funcGetItemsFromDB,
            Action<List<Task>, IBatch, TItem> funcSetToCache,
            Func<TItem, Guid> funcDistinctBy = null)
        {
            var lsRes = new List<TItem>();

            // try no find id in cache
            nos = nos?.Where(_ => _ != default)?.Distinct();
            if (nos?.Any() == true)
            {
                var tasks = new List<Task<(long, Guid)>>();
                var batch = easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var no in nos)
                {
                    tasks.Add(funcGetNo2IdFromCache(batch, no));
                }
                batch.Execute();
                await Task.WhenAll(tasks);

                // no found id
                ids ??= Enumerable.Empty<Guid>();
                ids = ids.Union(tasks.Select(_ => _.Result).Where(_ => _.Item2 != default).Select(_ => _.Item2));

                // no not found id
                nos = tasks.Select(_ => _.Result).Where(_ => _.Item2 == default).Select(_ => _.Item1).ToArray();
            }
            // find in cache by id
            ids = ids?.Where(_ => _ != default)?.Distinct();
            if (ids?.Any() == true)
            {
                var tasks = new List<Task<(Guid, TItem)>>();
                var batch = easyRedisClient.CacheRedisClient.Database.CreateBatch();
                foreach (var id in ids)
                {
                    tasks.Add(funcGetItemFromCache(batch, id));
                }
                batch.Execute();
                await Task.WhenAll(tasks);

                lsRes.AddRange(tasks.Select(_ => _.Result).Where(_ => _.Item2 != null).Select(_ => _.Item2));

                ids = tasks.Select(_ => _.Result).Where(_ => _.Item2 == null).Select(_ => _.Item1).ToArray();
            }
            // find in db
            if (ids?.Any() == true || nos?.Any() == true)
            {
                var ls = await funcGetItemsFromDB(ids, nos);
                if (ls != null && funcSetToCache != null)
                {
                    lsRes.AddRange(ls);

                    var tasks = new List<Task>();
                    var batch = easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var dto in ls)
                    {
                        funcSetToCache(tasks, batch, dto);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                }
            }

            return funcDistinctBy == null ? lsRes : lsRes.DistinctBy(_ => funcDistinctBy(_)).ToList();
        }

        /// <summary>
        /// 根据 ids和nos (可能无序)查items, 优先查cache后查db <br/>
        /// <br/>
        /// <code><![CDATA[
        /// // eg:
        /// var ls_res = await _easyRedisClient.GetItemsByIdsOrNos(ids, nos,
        ///     no => string.Format(CacheKeys.SubjectNo2Id, no),
        ///     id => string.Format(CacheKeys.Subject, id),
        ///     (ids2, nos2) => _subjectRepository.GetSubjects(ids2, nos2),
        ///     item => (item.Id, item.No),
        ///     60 * 60 * 1
        /// );         
        /// ]]></code>
        /// </summary>
        public static Task<IEnumerable<TItem>> GetItemsByIdsOrNos<TItem>(this IEasyRedisClient easyRedisClient, IEnumerable<Guid> ids, IEnumerable<long> nos,
            Func<long, string> funcCacheKey4No2Id, Func<Guid, string> funcCacheKey4Id,
            Func<IEnumerable<Guid>, IEnumerable<long>, Task<IEnumerable<TItem>>> funcGetItemsFromDB,
            Func<TItem, (Guid Id, long No)> funcItemIdAndNo, 
            int itemExpSec, int noExpSec = 60 * 60 * 36,
            bool setNx = false,
            Action<TItem> funcBeforeSetItemToCache = null)
        {
            return GetItemsByIdsOrNos(easyRedisClient, ids, nos,
                (batch, no) => batch.StringGetAsync(funcCacheKey4No2Id(no)).ContinueWith(t => (no, Guid.TryParse(t.Result.ToString(), out var _gid) ? _gid : default)),
                (batch, id) => batch.StringGetAsync(funcCacheKey4Id(id)).ContinueWith(t => (id, t.Result.ToString().JsonStrTo<TItem>())),
                funcGetItemsFromDB,
                (tasks, batch, item) => 
                {
                    funcBeforeSetItemToCache?.Invoke(item);
                    //
                    var (id, no) = funcItemIdAndNo(item);
                    var key = funcCacheKey4No2Id?.Invoke(no);
                    if (key != null) tasks.Add(batch.StringSetAsync(key, id.ToString(), TimeSpan.FromSeconds(noExpSec)));
                    key = funcCacheKey4Id?.Invoke(id);
                    if (key != null) tasks.Add(batch.StringSetAsync(key, item.ToJsonStr(true), TimeSpan.FromSeconds(itemExpSec), setNx ? When.NotExists : When.Always));
                },                
                (dto) => funcItemIdAndNo(dto).Id
            );
        }
    }
}