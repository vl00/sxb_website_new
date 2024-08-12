using Newtonsoft.Json;
using StackExchange.Redis;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Utils;
using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Common.OtherAPIClient.Comment;
using Sxb.School.Common.OtherAPIClient.Comment.Model.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class CallSchoolApiQuery : ICallSchoolApiQuery
    {
        readonly ISchoolRepository _schoolRepository;
        readonly IEasyRedisClient _easyRedisClient;

        public CallSchoolApiQuery(ISchoolRepository schoolRepository, 
            IEasyRedisClient easyRedisClient)
        {
            this._easyRedisClient = easyRedisClient;
            this._schoolRepository = schoolRepository;
        }


        public async Task<List<SchoolIdAndNameDto>> GetSchoolsIdAndName(string[] eids)
        {
            var result = new List<SchoolIdAndNameDto>();

            var ls_eid = new List<Guid>();
            var ls_eno = new List<long>();
            foreach (var eid0 in eids)
            {
                var eid = Guid.TryParse(eid0, out var _eid) ? _eid : default;
                if (eid != default) ls_eid.Add(eid);
                else
                {
                    if (long.TryParse(eid0, out var eno))
                        ls_eno.Add(eno);
                }
            }
            ls_eid = ls_eid.Distinct().ToList();
            ls_eno = ls_eno.Distinct().ToList();

            if (ls_eid.Count > 0)
            {
                var rr = new (Guid, SchoolIdAndNameDto)[ls_eid.Count];
                // find in redis
                {
                    var tasks = new List<Task<RedisValue>>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var eid in ls_eid)
                    {
                        var t = batch.StringGetAsync(string.Format(RedisCacheKeys.CallSchoolApi_School_idandname, eid));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);

                    for (var i = 0; i < tasks.Count; i++)
                    {
                        var dto = ((string)tasks[i].Result).JsonStrTo<SchoolIdAndNameDto>();
                        rr[i] = (ls_eid[i], dto);
                    }

                    result.AddRange(rr.Where(_ => _.Item2 != null).Select(_ => _.Item2));
                }

                // find in db
                var ls_notIn = ls_eid.Where(x => rr.Any(_ => _.Item1 == x && _.Item2 == null)).ToArray();
                if (ls_notIn.Any())
                {
                    var r2 = await _schoolRepository.GetSchoolsIdAndName(eids: ls_notIn);
                    result.AddRange(r2);

                    if (r2.Any())
                    {
                        var tasks2 = new List<Task>();
                        var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                        foreach (var dto in r2)
                        {
                            var t = batch.StringSetAsync(string.Format(RedisCacheKeys.CallSchoolApi_School_idandname, dto.Eid), dto.ToJsonStr(), TimeSpan.FromSeconds(60 * 60 * 24));
                            tasks2.Add(t);
                        }
                        batch.Execute();
                        await Task.WhenAll(tasks2);
                    }
                }
            }
            if (ls_eno.Count > 0)
            {                
                var rr = new (long, SchoolIdAndNameDto)[ls_eno.Count];
                // find in redis
                {
                    var tasks = new List<Task<RedisValue>>();
                    var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                    foreach (var eno in ls_eno)
                    {
                        var t = batch.StringGetAsync(string.Format(RedisCacheKeys.CallSchoolApi_School_noandname, eno));
                        tasks.Add(t);
                    }
                    batch.Execute();
                    await Task.WhenAll(tasks);
                    
                    for (var i = 0; i < tasks.Count; i++)
                    {
                        var dto = ((string)tasks[i].Result).JsonStrTo<SchoolIdAndNameDto>();
                        rr[i] = (ls_eno[i], dto);
                    }
                    
                    result.AddRange(rr.Where(_ => _.Item2 != null).Select(_ => _.Item2));
                }

                // find in db
                var ls_notIn = ls_eno.Where(x => rr.Any(_ => _.Item1 == x && _.Item2 == null)).ToArray();
                if (ls_notIn.Any())
                {
                    var r2 = await _schoolRepository.GetSchoolsIdAndName(nos: ls_notIn);
                    result.AddRange(r2);

                    if (r2.Any())
                    {
                        var tasks2 = new List<Task>();
                        var batch = _easyRedisClient.CacheRedisClient.Database.CreateBatch();
                        foreach (var dto in r2)
                        {
                            var t = batch.StringSetAsync(string.Format(RedisCacheKeys.CallSchoolApi_School_idandname, dto.Eid), dto.ToJsonStr(), TimeSpan.FromSeconds(60 * 60 * 24));
                            tasks2.Add(t);
                            t = batch.StringSetAsync(string.Format(RedisCacheKeys.CallSchoolApi_School_noandname, dto.SchoolNo), dto.ToJsonStr(), TimeSpan.FromSeconds(60 * 60 * 24));
                            tasks2.Add(t);
                        }
                        batch.Execute();
                        await Task.WhenAll(tasks2);
                    }
                }
            }

            return result;
        }

        
    }
}
