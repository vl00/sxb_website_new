using Newtonsoft.Json;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
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
    /// <summary>
    /// (首页)学校总评
    /// </summary>
    public class SchoolGeneralCommentQuery : ISchoolGeneralCommentQuery
    {
        readonly ISchoolScoreRepository _schoolScoreRepository;
        readonly ISchoolRepository _schoolRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ICommentAPIClient _commentAPIClient;
        readonly ISchoolScoreQuery _schoolScoreQuery;

        public SchoolGeneralCommentQuery(ISchoolScoreRepository schoolScoreRepository, ISchoolRepository schoolRepository, ISchoolScoreQuery schoolScoreQuery,
            ICommentAPIClient commentAPIClient,
            IEasyRedisClient easyRedisClient)
        {
            this._schoolScoreRepository = schoolScoreRepository;
            this._easyRedisClient = easyRedisClient;
            this._schoolRepository = schoolRepository;
            this._schoolScoreQuery = schoolScoreQuery;
            this._commentAPIClient = commentAPIClient;
        }

        public async Task<List<SchoolGeneralCommentDto>> QueryGeneralCommentByeids(IEnumerable<string> eids)
        {
            var items = new List<SchoolGeneralCommentDto>();

            if ((eids?.Count() ?? 0) <= 0)
            {
                return items;
            }

            foreach (var eidStr in eids)
            {
                var eid = Guid.TryParse(eidStr, out var _eid) ? _eid : default;
                var int_SchoolNo = eid != default ? default : UrlShortIdUtil.Base322Long(eidStr);

                // see '_schoolExtensionQuery.GetSchoolExtensionDetails'
                var schoolInfo = await _schoolRepository.GetExtensionDTO(eid, int_SchoolNo);
                if (schoolInfo != null)
                {
                    schoolInfo.SchFType = new SchFType0(schoolInfo.Grade, schoolInfo.Type, schoolInfo.Discount, schoolInfo.Diglossia, schoolInfo.Chinese);
                    CommonHelper.PropertyStringDef(schoolInfo, "暂未收录", "[]");
                }
                if (schoolInfo == null || schoolInfo.ExtId == Guid.Empty)
                {
                    items.Add(null);
                    continue;
                }

                //
                var resItem = new SchoolGeneralCommentDto();
                items.Add(resItem);
                eid = resItem.Eid = schoolInfo.ExtId;
                resItem.Name = $"{schoolInfo.SchoolName}-{schoolInfo.ExtName}";
                resItem.Intro = schoolInfo.Intro;
                resItem.Logo = schoolInfo.Logo;
                resItem.Url = $"/school-{UrlShortIdUtil.Long2Base32(schoolInfo.SchoolNo)}/";

                // 标签
                // 参考 https://m.sxkid.com/api/school/PageScoreSortSchool?pageIndex=2&pageSize=12&grade=1              
                resItem.Tags ??= new List<string>();
                resItem.Tags.Add(((SchoolType)schoolInfo.Type).Description());
                if (!string.IsNullOrWhiteSpace(schoolInfo.Authentication))
                {
                    try
                    {
                        var authes = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(schoolInfo.Authentication);
                        if (authes.Count > 0)
                        {
                            var tagCount = authes.Count(p => p.Key != "未收录" && p.Key.Count() <= 8);
                            if (tagCount > 0)
                            {
                                resItem.Tags.AddRange(authes.Select(p => p.Key));
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                // 总评分数
                // 参考 https://m.sxkid.com/api/schoolscore/GetSchoolExtScore?extID=
                resItem.SchoolScore = await _schoolScoreQuery.GetSchoolScoreTreeByEID(resItem.Eid);
            }

            // (家长评价)共n人点评
            // 参考 https://m.sxkid.com/api/schoolscore/GetCommentScoreStatistics?extID=
            {
                IEnumerable<SchoolScoreCommentCountDto> sscc = null;
                try { sscc = await _commentAPIClient.GetSchoolScoreCommentCountByEids(items.Select(_ => _.Eid)); } catch { }
                if (sscc != null)
                {
                    foreach (var resItem in items)
                    {
                        resItem.CommentCount = sscc.FirstOrDefault(_ => _.Eid == resItem.Eid)?.CommentCount;
                    }
                }
            }

            return items;
        }

        public Task<List<SchoolGeneralCommentDto>> QueryTop10GeneralCommentByGrade(int grade)
        {
            return _easyRedisClient.GetOrAddAsync($"ext:top10GeneralComment:grade_{grade}", async () =>
            {
                var eids = (await _schoolScoreRepository.GetEidsForSchoolScoreTopNByGrade(10, grade)).Select(_ => _.ToString());
                var ls = await QueryGeneralCommentByeids(eids);
                return ls;
            }, TimeSpan.FromSeconds(60 * 60 * 24));
        }

    }
}
