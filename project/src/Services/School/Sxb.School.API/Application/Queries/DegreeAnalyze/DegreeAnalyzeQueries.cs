using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.RequestContact.DegreeAnalyze;
using Sxb.School.Common;
using Sxb.School.Common.Consts;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Sxb.School.API.Application.Queries.DegreeAnalyze
{
    public class DegreeAnalyzeQueries : IDegreeAnalyzeQueries
    {
        readonly IDegreeAnalyzeRepository _degreeAnalyzeRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ISchoolScoreRepository _schoolScoreRepository;

        public DegreeAnalyzeQueries(IDegreeAnalyzeRepository degreeAnalyzeRepository, ISchoolScoreRepository schoolScoreRepository,
            IEasyRedisClient easyRedisClient)
        { 
            this._degreeAnalyzeRepository = degreeAnalyzeRepository;
            this._easyRedisClient = easyRedisClient;
            this._schoolScoreRepository = schoolScoreRepository;
        }

        public async Task<DgAyGetQuestionResponse> GetQuestions(bool readCache = true, bool writeCache = true, bool useReadConn = true, bool showFindField = false)
        {
            var result = readCache ? await _easyRedisClient.GetAsync<DgAyGetQuestionResponse>(CacheKeys.DgAyQuestions) : null;
            if (result == null)
            {
                result = await GetQuestions_core(useReadConn, showFindField);
                if (writeCache) await _easyRedisClient.AddAsync(CacheKeys.DgAyQuestions, result, TimeSpan.FromSeconds(60 * 60));
            }
            return result;
        }
        private async Task<DgAyGetQuestionResponse> GetQuestions_core(bool useReadConn, bool showFindField)
        { 
            var result = new DgAyGetQuestionResponse();
            result.Ques = new List<DgAyQuestionVm>();

            var quesLs = await _degreeAnalyzeRepository.FindQuestions(useReadConn);
            var optsLs = await _degreeAnalyzeRepository.FindQuestionOptions(useReadConn);
            IEnumerable<(long Code, string Name)> areaLs = null;
            IEnumerable<(long City, string CityName, long Area, string AreaName)> addrAreasLs = null;
            List<DgAyQuesOptionTy5Vm_Item2> ty5OptsItems2 = null;

            // ques
            foreach (var ques in quesLs)
            {
                var q = new DgAyQuestionVm();
                result.Ques.Add(q);
                q.Id = ques.Id;
                q.Title = ques.Title;
                q.Type = ques.Type;
                q.NextQid = ques.NextQid;
                q.Is1st = ques.Is1st;
                q.MaxSelected = ques.MaxSelected;
                q.FindField = ques.FindField;

                var qtype = (DgAyQuestionTypeEnum)q.Type;
                switch (qtype)
                {
                    // 单选 多选
                    case DgAyQuestionTypeEnum _ when (q.Type > 0 && q.Type < 5):
                        {
                            var opts = optsLs.Where(_ => _.Qid == q.Id).OrderBy(_ => _.Sort).ToArray();
                            if (opts.Length <= 0) break;
                            q.Opts = opts.Select(x =>
                            {
                                var opt = new DgAyQuesOptionVm();
                                opt.Id = x.Id;
                                opt.Qid = x.Qid ?? q.Id;
                                opt.Title = x.Title;
                                opt.Point = x.Point;
                                opt.NextQid = x.NextQid;
                                if (showFindField)
                                {
                                    opt.FindField = x.FindField ?? ques.FindField;
                                    opt.FindFieldFw = x.FindFieldFw;
                                    opt.FindFieldFwJx = x.FindFieldFwJx;
                                }
                                return opt;
                            }).ToList();
                        }
                        break;

                    // 地址选择
                    case DgAyQuestionTypeEnum.Ty5:
                        {
                            addrAreasLs ??= await _degreeAnalyzeRepository.FindAddrAreas(440100);
                            q.Ty5Opts = new DgAyQuesOptionTy5Vm();

                            q.Ty5Opts.Items1 = addrAreasLs.Select(_ => (_.Area, _.AreaName)).Distinct().OrderBy(_ => _.Area)
                                .Select(_ => new DgAyQuesOptionTy5Vm_Item1 { Id = _.Area, Title = _.AreaName })
                                .ToList();

                            var area1 = q.Ty5Opts.Items1.FirstOrDefault()?.Id;
                            ty5OptsItems2 ??= area1 == null ? new List<DgAyQuesOptionTy5Vm_Item2>()
                                : (await _degreeAnalyzeRepository.FindAddresses(area1)).OrderBy(_ => _.Sort).Select(_ => new DgAyQuesOptionTy5Vm_Item2 { Title = _.Address }).ToList();
                            q.Ty5Opts.Items2 = ty5OptsItems2;
                        }
                        break;

                    // 地区单选
                    case DgAyQuestionTypeEnum.Ty7:
                        {
                            var opts = optsLs.Where(_ => _.Qid == q.Id).OrderBy(_ => _.Sort);
                            q.Ty7Opts = new List<DgAyQuesOptionTy7Vm_Item1>();
                            if (opts.Any())
                            {
                                foreach (var opt in opts)
                                {
                                    var ac = await _degreeAnalyzeRepository.GetCityAreaByCodeOrName(opt.Title);
                                    q.Ty7Opts.Add(new DgAyQuesOptionTy7Vm_Item1
                                    {
                                        Id = opt.Id,
                                        Title = opt.Title,
                                        Area = ac.Code,
                                        NextQid = opt.NextQid,
                                        FindField = showFindField ? (opt.FindField ?? ques.FindField) : null,
                                        FindFieldFw = showFindField ? opt.FindFieldFw : null,
                                        FindFieldFwJx = showFindField ? opt.FindFieldFwJx : null,
                                    });
                                }
                            }
                            else 
                            {
                                // 没录入选项,查全广州的区
                                areaLs ??= await _degreeAnalyzeRepository.GetCityAreas(440100);

                                q.Ty7Opts.AddRange(areaLs.Select(x => new DgAyQuesOptionTy7Vm_Item1
                                {
                                    Title = x.Name,
                                    Area = x.Code,
                                    FindField = showFindField ? (ques.FindField) : null,
                                    FindFieldFw = showFindField && ques.FindField != null ? $"{x.Code}" : null,
                                    FindFieldFwJx = showFindField && ques.FindField != null ? $"{{tb}}.Area={x.Code}" : null,
                                }));
                            }
                        }
                        break;

                    case DgAyQuestionTypeEnum.Ty6:
                    default:
                        // no need query
                        break;
                }
            }

            result.Qid1st = quesLs.Where(_ => _.Is1st).FirstOrDefault()?.Id ?? 0;
            // 优先选项跳问题

            return result;
        }

        public async Task<DgAyGetQuesAddressesResponse> GetQuesAddresses(int area)
        { 
            var result = new DgAyGetQuesAddressesResponse();
            result.Area = area;

            var addressLs = await _degreeAnalyzeRepository.FindAddresses(area);
            result.Address = addressLs.OrderBy(_ => _.Sort).Select(_ => _.Address).ToList();

            return result;
        }

        public async Task<PagedList<string>> GetQuesAddresses(DgAyFindAddressesQuery query)
        {
            var result = await _degreeAnalyzeRepository.FindAddresses(query.Area, query.Kw, query.PageIndex, query.PageSize);
            return result.CurrentPageItems.Select(_ => _.Address).ToList().ToPagedList(query.PageSize, query.PageIndex, result.TotalItemCount);
        }

        public async Task<PagedList<DgAyMyQaResultListItem>> GetMyQaResultList(Guid userid, int pageIndex, int pageSize)
        {
            var rr = await _degreeAnalyzeRepository.GetMyQaResultList(userid, pageIndex, pageSize);
            return rr.CurrentPageItems.Select(x =>
            {
                var item = new DgAyMyQaResultListItem();
                item.Id = x.Id;
                item.Title = $"{x.Time:yyyy年MM月dd日}{(x.i <= 1 ? "" : $"第{x.i}次")}学位分析结果";
                item.Time = x.Time;
                return item;
            }).ToArray().ToPagedList(pageSize, pageIndex, rr.TotalItemCount);            
        }

        // 我的分析报告结果页
        public async Task<DgAyQaResultVm> GetQaResult(Guid id, bool showAll = false, Guid? me = null, bool includeDeletedSchool = false)
        {
            var dto = await _degreeAnalyzeRepository.GetQaPaperAndResult(id);
            if (dto == null) throw new ResponseResultException("结果不存在", 201);
            if (!showAll && me != null && dto.UserId != me)
            {
                throw new ResponseResultException("不是我的问卷+报告", Errcodes.DgAyTheQapaperIsNotMy);
            }
            var result = new DgAyQaResultVm();
            result.Id = id;
            result.Title = dto.Title;
            result.Type = dto.Atype;
            var atype = (DgAyAtypeEnum)dto.Atype;
            result.TypeDesc = (atype).GetDescription();
            result.IsUnlocked = dto.Status == (byte)DgAyStatusEnum.Unlocked;
            result.IsMy = me == null ? (bool?)null : dto.UserId == me;
            var isUnlocked = showAll || result.IsUnlocked;
            //
            // result content 
            // 未解锁时 学校list里面的数据为null
            {
                var ctn = JsonStrTo<DgAyQaResultCtnDto>(dto.Ctn) ?? new DgAyQaResultCtnDto();
                var pcyFileYear = (dto.UnlockedTime ?? dto.AnalyzedTime ?? dto.LastSubmitTime ?? dto.CreateTime ?? DateTime.Now).Year - 1;

                var schools = new List<DgAySchoolItemVm>();
                if (isUnlocked && ctn.Eids?.Count > 0)
                {
                    var schs = await _degreeAnalyzeRepository.GetSchoolItems(ctn.Eids);
                    if (!includeDeletedSchool) schools.AddRange(schs.Where(_ => _.IsValid).Select(x => Mapper(x))); // 过滤掉已删除的学部
                    else schools.AddRange(schs.Select(x => Mapper(x))); // 未过滤掉已删除的学部

                    var scores = await _schoolScoreRepository.GetSchoolsScoreEids(ctn.Eids);
                    foreach (var school in schools)
                    {
                        if (!TryGetOne(scores, out var score, _ => _.Eid == school.Eid)) continue;
                        school.Score = score.Score;
                    }
                }

                switch (atype)
                {
                    case DgAyAtypeEnum.Cp:
                        {
                            var resTy = result.ResultTy1 = new DgAyQaResultVm_ResultTy1();
                            resTy.CpPcyFile = Mapper(isUnlocked ? ctn.CpSchPcyFile : null) ?? new DgAySchPcyFileVm { Year = pcyFileYear };
                            resTy.CpHeliPcyFile = Mapper(isUnlocked ? ctn.CpHeliSchPcyFile : null) ?? new DgAySchPcyFileVm { Year = pcyFileYear };
                            resTy.CpPcAssignPcyFile = Mapper(isUnlocked ? ctn.CpPcAssignSchPcyFile : null) ?? new DgAySchPcyFileVm { Year = pcyFileYear };

                            resTy.CpSchool = schools.FirstOrDefault(_ => _.Eid == ctn.CpSchoolEid) ?? new DgAySchoolItemVm();
                            resTy.CpHeliSchools = ctn.CpHeliSchoolEids == null ? null : ctn.CpHeliSchoolEids.Select(x => schools.FirstOrDefault(_ => _.Eid == x)).ToList();
                            resTy.CpPcAssignSchools = ctn.CpPcAssignSchoolEids == null ? null : ctn.CpPcAssignSchoolEids.Select(x => schools.FirstOrDefault(_ => _.Eid == x)).ToList();
                        }
                        break;

                    case DgAyAtypeEnum.Ov:
                        {
                            var resTy = result.ResultTy2 = new DgAyQaResultVm_ResultTy2();
                            resTy.OvPcyFile = Mapper(isUnlocked ? ctn?.OvSchPcyFile : null) ?? new DgAySchPcyFileVm { Year = pcyFileYear };
                            resTy.OvSchools = ctn.OvSchoolEids == null ? null : ctn.OvSchoolEids.Select(x => schools.FirstOrDefault(_ => _.Eid == x)).ToList();
                        }
                        break;

                    case DgAyAtypeEnum.Jf:
                        {
                            var resTy = result.ResultTy3 = new DgAyQaResultVm_ResultTy3();
                            resTy.JfPoints = ctn.JfPoints ?? 0;
                            resTy.JfPcyFile = Mapper(isUnlocked ? ctn?.JfSchPcyFile : null) ?? new DgAySchPcyFileVm { Year = pcyFileYear };
                            resTy.JfSchools = ctn.JfSchoolEids == null ? null : ctn.JfSchoolEids.Select(x => schools.FirstOrDefault(_ => _.Eid == x)).ToList();
                            // 积分入学-录取分数线
                            if (resTy.JfSchools?.Any() == true)
                            {
                                var scores = await _degreeAnalyzeRepository.GetSchoolJfScoreLine(ctn.Eids, resTy.JfPcyFile.Year);
                                foreach (var school in resTy.JfSchools)
                                {
                                    if (!isUnlocked) continue;
                                    school.JfYear = resTy.JfPcyFile.Year;
                                    if (!TryGetOne(scores, out var score, _ => _.Eid == school.Eid)) continue;
                                    school.JfScoreLine = score.Score;
                                }
                            }
                        }
                        break;

                    case DgAyAtypeEnum.Mb:
                        {
                            var resTy = result.ResultTy4 = new DgAyQaResultVm_ResultTy4();
                            resTy.MbSchools = ctn.MbSchoolEids == null ? null : ctn.MbSchoolEids.Select(x => schools.FirstOrDefault(_ => _.Eid == x)).ToList();
                        }
                        break;
                }
            }
            // qa content
            if (isUnlocked)
            {
                result.QaContent = new DgAyUserQaContentVm();
                var ctns = await _degreeAnalyzeRepository.GetQaContents(id) ?? Array.Empty<DgAyUserQaContent>();
                result.QaContent.Ques = ctns.Select(x => JsonStrTo<DgAyQaQuesItemVm>(x.Ctn)).ToList();
            }
            
            return result;
        }

        public async Task<DgAyQaResultVm0> GetQaCtn(Guid id)
        {
            var dto = await _degreeAnalyzeRepository.GetQaPaperAndResult(id);
            if (dto == null) throw new ResponseResultException("结果不存在", 201);
            var result = new DgAyQaResultVm0();
            result.Id = id;
            result.Title = dto.Title;
            result.Type = dto.Atype;
            var atype = (DgAyAtypeEnum)dto.Atype;
            result.TypeDesc = (atype).GetDescription();
            result.IsUnlocked = dto.Status == (byte)DgAyStatusEnum.Unlocked;
            // qa content
            if (true)
            {
                result.QaContent = new DgAyUserQaContentVm();
                var ctns = await _degreeAnalyzeRepository.GetQaContents(id) ?? Array.Empty<DgAyUserQaContent>();
                result.QaContent.Ques = ctns.Select(x => JsonStrTo<DgAyQaQuesItemVm>(x.Ctn)).ToList();
            }
            return result;
        }

        public async Task<DgAyQaIsUnlockedVm> IsUnlocked(Guid id, Guid? me = null)
        {
            var dto = await _degreeAnalyzeRepository.GetQaIsUnlocked(id);
            if (dto == null) throw new ResponseResultException("结果不存在", 201);
            if (me != null && me != dto.UserId)
            {
                throw new ResponseResultException("不是我的问卷+报告", Errcodes.DgAyTheQapaperIsNotMy);
            }
            var result = new DgAyQaIsUnlockedVm();
            result.Id = dto.Id;
            result.IsUnlocked = dto.Status == (byte)DgAyStatusEnum.Unlocked;
            return result;
        }

        static DgAySchPcyFileVm Mapper(DgAySchPcyFileDto s)
        {
            if (s == null) return null;
            var t = new DgAySchPcyFileVm();
            t.Year = s.Year;
            t.Title = s.Title;
            t.Url = s.Url;
            return t;
        }

        static DgAySchoolItemVm Mapper(DgAySchoolItemDto s)
        {
            if (s == null) return null;
            var t = new DgAySchoolItemVm();
            t.Eid = s.Eid;
            t.Eid_s = UrlShortIdUtil.Long2Base32(s.No);
            t.Schname = s.Schname;
            t.Extname = s.Extname;
            t.Address = s.Address;
            t.RecruitWay = !string.IsNullOrEmpty(s.RecruitWay) ? JsonStrTo<List<string>>(s.RecruitWay) : new List<string>();
            t.Tags = new List<string>();
            t.Tags.Add(SchFTypeUtil.ReplaceSchFTypeName(SchFType0.Parse(s.SchFtype)));
            t.Tags.Add(((EduSysType)s.EduSysType).GetDescription());
            if (!string.IsNullOrWhiteSpace(s.Authentication))
            {
                try
                {
                    var authes = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(s.Authentication);
                    if (authes.Count > 0)
                    {
                        var tagCount = authes.Count(p => p.Key != "未收录" && p.Key.Count() <= 8);
                        if (tagCount > 0)
                        {
                            t.Tags.AddRange(authes.Select(p => p.Key));
                        }
                    }
                }
                catch
                {
                }
            }
            t.Tags.RemoveAll(_ => string.IsNullOrEmpty(_));
            return t;
        }

        static T JsonStrTo<T>(string json, JsonSerializerSettings jsonSerializerSettings = null)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                if (string.IsNullOrEmpty(json)) return (T)(object)null;
                throw new SerializationException(ex.Message, ex);
            }
        }

        static bool TryGetOne<T>(IEnumerable<T> enumerable, out T item, Func<T, bool> condition = null)
        {
            item = default;
            foreach (var item0 in enumerable)
            {
                if (condition == null || condition(item0))
                {
                    item = item0;
                    return true;
                }
            }
            return false;
        }


    }
}
