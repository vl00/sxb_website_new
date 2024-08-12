using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Queries.DegreeAnalyze;
using Sxb.School.Common.Consts;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Commands
{
    public class DgAySubmitQaCmdHandler : IRequestHandler<DgAySubmitQaCmd, DgAySubmitQaCmdResult>
    {
        readonly IDegreeAnalyzeQueries _degreeAnalyzeQueries;
        readonly IDegreeAnalyzeRepository _degreeAnalyzeRepository;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ILogger<DgAySubmitQaCmdHandler> _log;

        public DgAySubmitQaCmdHandler(IDegreeAnalyzeQueries degreeAnalyzeQueries, IDegreeAnalyzeRepository degreeAnalyzeRepository, ILogger<DgAySubmitQaCmdHandler> log,
            IEasyRedisClient easyRedisClient)
        {
            this._degreeAnalyzeQueries = degreeAnalyzeQueries;
            this._degreeAnalyzeRepository = degreeAnalyzeRepository;
            this._easyRedisClient = easyRedisClient;
            this._log = log;
        }

        public async Task<DgAySubmitQaCmdResult> Handle(DgAySubmitQaCmd cmd, CancellationToken cancellationToken)
        {
            var result = new DgAySubmitQaCmdResult();
            try
            {
                if (!await _easyRedisClient.CacheRedisClient.Database.StringSetAsync(
                    string.Format(CacheKeys.DgAyXzUserSubmitQa, cmd.UserId), 1, TimeSpan.FromSeconds(3), StackExchange.Redis.When.NotExists))
                {
                    throw new ResponseResultException("请不要频繁提交", 201);
                }

                var (qpaper, quesCtns, totalPoints) = await On_Submit(result, cmd);
                await On_Analyze(qpaper.Id, qpaper, quesCtns, totalPoints);

                await Task.Delay(1500);
            }
            finally
            { 
            }
            return result;
        }

        private async Task<(DgAyUserQaPaper, List<DgAyQaQuesItemVm>, double)> On_Submit(DgAySubmitQaCmdResult result, DgAySubmitQaCmd cmd)
        {
            if ((cmd.Items?.Length ?? 0) < 1) throw new ResponseResultException("请先做题目", 201);
            if (cmd.UserId == default) throw new ResponseResultException("请先登录", 401);            
            if (!Enum.IsDefined(typeof(DgAyTermTypeEnum), cmd.Termtyp)) throw new ResponseResultException("终端类型参数错误", Errcodes.DgAySubmitQaTermtypError);
            //
            var totalPoints = 0d;
            var quesCtns = new List<DgAyQaQuesItemVm>();   // 问卷内容 
            var last_CpOv = (DgAyAtypeEnum)0;   // 最后的地址选择还是地图
            await default(ValueTask);
            //
            var allQues = await _degreeAnalyzeQueries.GetQuestions(useReadConn: false, readCache: false, writeCache: false, showFindField: true);
            long nxt_i_qid = 0;
            for (var i = 0; i < cmd.Items.Length; i++)
            {
                var aItem = cmd.Items[i];                
                if (aItem == null)
                {
                    throw new ResponseResultException("请先做题目", 201);
                }
                //
                // 是否第一题
                if (i == 0 && allQues.Qid1st != aItem.Id)
                {
                    throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQIsChanged);
                }
                //
                // 是否之前的题目+选项会跳到本题
                if (i > 0 && aItem.Id != nxt_i_qid)
                {                    
                    throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyNextqidIsChanged);
                }
                //
                // 原题目
                var ques0 = allQues.Ques.FirstOrDefault(_ => _.Id == aItem.Id);
                if (ques0 == null || ques0.Type != aItem.Type)
                {
                    // 变题型了
                    throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQIsChanged);
                }
                if (i != cmd.Items.Length - 1 && ques0.IsLast)
                {
                    // 是否最后一题 做多题了
                    throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQIsChanged);
                }
                if (i == cmd.Items.Length - 1 && !ques0.IsLast)
                {
                    // 是否最后一题 做少题了
                    throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQIsChanged);
                }
                //                 
                var qtype = (DgAyQuestionTypeEnum)aItem.Type;
                switch (qtype)
                {
                    // 1=单选 3=单选计分 4=下拉计分 
                    case DgAyQuestionTypeEnum.Ty1:
                    case DgAyQuestionTypeEnum.Ty3:
                    case DgAyQuestionTypeEnum.Ty4:
                        {
                            if (aItem.Item == null) throw new ResponseResultException("参数错误", 201);
                            var opt0 = ques0.Opts.FirstOrDefault(_ => _.Id == aItem.Item.Id);
                            //
                            // 选项不存在或被删除了
                            if (opt0 == null)
                            {
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptIsChanged);
                            }
                            // 
                            // 分数不一样
                            if ((aItem.Item.Point ?? 0) != (opt0.Point ?? 0))
                            {
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptPointIsChanged);
                            }

                            totalPoints += (aItem.Item.Point ?? 0);
                            nxt_i_qid = opt0.NextQid ?? ques0.NextQid ?? 0;
                            //
                            // set qa content
                            var quesCtn = new DgAyQaQuesItemVm();
                            quesCtns.Add(quesCtn);
                            quesCtn.Id = aItem.Id;
                            quesCtn.Title = ques0.Title;
                            quesCtn.Type = ques0.Type;
                            quesCtn.Score = (opt0.Point);
                            quesCtn.Opts = ques0.Opts.Select(x => new DgAyQaOptItemVm
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Selected = x.Id == opt0.Id,
                                Point = x.Point,
                                FindField = x.FindField,
                                FindFieldFw = x.FindFieldFw,
                                FindFieldFwJx = x.FindFieldFwJx,
                            }).ToList();
                        }
                        break;

                    // 2=多选
                    case DgAyQuestionTypeEnum.Ty2:
                        {
                            if (aItem.Items == null) throw new ResponseResultException("参数错误", 201);
                            var opts0 = ques0.Opts.Where(x => aItem.Items.Any(_ => _.Id == x.Id)).ToArray();
                            //
                            // 存在部分选项不存在或被删除了
                            if (opts0.Length != aItem.Items.Length)
                            {
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptIsChanged);
                            }
                            // 选多了
                            if (ques0.MaxSelected != null && aItem.Items.Length > ques0.MaxSelected.Value)
                            {
                                throw new ResponseResultException("参数错误", 201);
                            }
                            // 分数不一样
                            if (aItem.Items.Any(x => (ques0.Opts.FirstOrDefault(_ => _.Id == x.Id)?.Point ?? 0) != (x.Point ?? 0)))
                            {
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptPointIsChanged);
                            }

                            totalPoints += aItem.Items.Sum(_ => _.Point ?? 0);
                            nxt_i_qid = ques0.NextQid ?? 0;
                            //
                            // set qa content
                            var quesCtn = new DgAyQaQuesItemVm();
                            quesCtns.Add(quesCtn);
                            quesCtn.Id = aItem.Id;
                            quesCtn.Title = ques0.Title;
                            quesCtn.Type = ques0.Type;
                            quesCtn.Score = aItem.Items.Sum(_ => _.Point ?? 0);
                            quesCtn.Opts = ques0.Opts.Select(x => new DgAyQaOptItemVm
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Selected = aItem.Items.Any(_ => _.Id == x.Id),
                                Point = x.Point,
                                FindField = x.FindField,
                                FindFieldFw = x.FindFieldFw,
                                FindFieldFwJx = x.FindFieldFwJx,
                            }).ToList();
                        }
                        break;

                    // 地区单选
                    case DgAyQuestionTypeEnum.Ty7:
                        {
                            if (aItem.Ty7Item == null || aItem.Ty7Item.Area <= 0) throw new ResponseResultException("参数错误", 201);
                            var opt0 = ques0.Ty7Opts.FirstOrDefault(_ => _.Area == aItem.Ty7Item.Area);
                            if (opt0 == null)
                            {
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptIsChanged);
                            }
                            //                            
                            nxt_i_qid = opt0.NextQid ?? ques0.NextQid ?? 0;
                            //
                            // set qa content
                            var quesCtn = new DgAyQaQuesItemVm();
                            quesCtns.Add(quesCtn);
                            quesCtn.Id = aItem.Id;
                            quesCtn.Title = ques0.Title;
                            quesCtn.Type = ques0.Type;
                            quesCtn.OptTy7 = ques0.Ty7Opts.Select(x => new DgAyQaOptItemTy7Vm
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Selected = x.Area == opt0.Area,
                                Area = x.Area,
                                FindField = x.FindField,
                                FindFieldFw = x.FindFieldFw,
                                FindFieldFwJx = x.FindFieldFwJx,
                            }).ToList();
                        }
                        break;

                    // 地址选择
                    case DgAyQuestionTypeEnum.Ty5:
                        {
                            if (aItem.Ty5Item == null) throw new ResponseResultException("参数错误", 201);
                            aItem.Ty5Item.Address = aItem.Ty5Item.Address?.Trim();
                            if (string.IsNullOrEmpty(aItem.Ty5Item.Address)) throw new ResponseResultException("参数错误", 201);

                            var areaAddressLs = await _degreeAnalyzeRepository.FindAddresses(aItem.Ty5Item.Area);
                            var areaAddress = areaAddressLs?.FirstOrDefault(_ => _.Address == aItem.Ty5Item.Address) ?? default;
                            if (areaAddress.Address == default)
                            {
                                // 地址变了
                                throw new ResponseResultException("题目已更新,请重新答题", Errcodes.DgAyQoptIsChanged);
                            }

                            nxt_i_qid = ques0.NextQid ?? 0;
                            last_CpOv = DgAyAtypeEnum.Cp;
                            //
                            // set qa content
                            var quesCtn = new DgAyQaQuesItemVm();
                            quesCtns.Add(quesCtn);
                            quesCtn.Id = aItem.Id;
                            quesCtn.Title = ques0.Title;
                            quesCtn.Type = ques0.Type;
                            quesCtn.OptTy5 = new DgAyQaOptItemTy5Vm
                            {
                                Items1 = new List<DgAyQaOptItem>
                                {
                                    new DgAyQaOptItem
                                    {
                                        Id = aItem.Ty5Item.Area,
                                        Title = areaAddress.AreaName,
                                        Selected = true,
                                    }
                                },
                                Items2 = new List<DgAyQaOptItem>
                                {
                                    new DgAyQaOptItem { Title = areaAddress.Address, Selected = true }
                                },
                            };
                        }
                        break;

                    // 地图定位
                    case DgAyQuestionTypeEnum.Ty6:
                        {
                            if (aItem.Ty6Item == null) throw new ResponseResultException("参数错误", 201);
                            aItem.Ty6Item.City = aItem.Ty6Item.City?.Trim();
                            aItem.Ty6Item.Area = aItem.Ty6Item.Area?.Trim();
                            var cityCode = -1L;
                            {
                                var ac = await _degreeAnalyzeRepository.GetCityAreaByCodeOrName(aItem.Ty6Item.City);
                                if (ac == default) throw new ResponseResultException("参数错误", 201);
                                cityCode = ac.Code;
                            }
                            var areaCode = -1L;
                            {
                                var ac = await _degreeAnalyzeRepository.GetCityAreaByCodeOrName(aItem.Ty6Item.Area, cityCode);
                                if (ac == default) throw new ResponseResultException("参数错误", 201);
                                areaCode = ac.Code;
                            }

                            nxt_i_qid = ques0.NextQid ?? 0;
                            last_CpOv = DgAyAtypeEnum.Ov;
                            //
                            // set qa content
                            var quesCtn = new DgAyQaQuesItemVm();
                            quesCtns.Add(quesCtn);
                            quesCtn.Id = aItem.Id;
                            quesCtn.Title = ques0.Title;
                            quesCtn.Type = ques0.Type;
                            quesCtn.OptTy6 = new DgAyQaOptItemTy6Vm
                            {
                                Lng = aItem.Ty6Item.Lng,
                                Lat = aItem.Ty6Item.Lat,
                                City = cityCode == -1 ? null : cityCode.ToString(),
                                Area = areaCode == -1 ? null : areaCode.ToString(),
                            };
                        }
                        break;
                }
            }

            //
            // check ok...
            var qaid = result.Qaid = Guid.NewGuid();
            var ques_1st = quesCtns.First();
            //
            var qpaper = new DgAyUserQaPaper();
            qpaper.Id = qaid;
            qpaper.UserId = cmd.UserId;
            qpaper.IsValid = true;
            qpaper.CreateTime = DateTime.Now;
            qpaper.Title = $"{qpaper.CreateTime:yyyy年MM月dd日}学位分析结果";
            qpaper.Status = (byte)DgAyStatusEnum.Todo;
            qpaper.SubmitCount = 1;
            qpaper.LastSubmitTime = DateTime.Now;
            qpaper.Termtyp = (byte)cmd.Termtyp;
            // qpaper.Atype
            {
                //var atype1 = qpaper.Atype = ques_1st.Opts.First(_ => _.Selected).Title is string optTitle1 ? (
                //    optTitle1.IndexOf("对口") > -1 || optTitle1.IndexOf("统筹") > -1 ? (byte)DgAyAtypeEnum.Cp
                //    : optTitle1.IndexOf("积分") > -1 ? (byte)DgAyAtypeEnum.Jf
                //    : optTitle1.IndexOf("民办") > -1 ? (byte)DgAyAtypeEnum.Mb
                //    : (byte)0
                //) : (byte)0;
                qpaper.Atype = ques_1st.Opts.First(_ => _.Selected).Id is long id1 ? (
                    id1 == 1 ? (byte)DgAyAtypeEnum.Cp
                    : id1 == 2 ? (byte)DgAyAtypeEnum.Jf
                    : id1 == 3 ? (byte)DgAyAtypeEnum.Mb
                    : (byte)0
                ) : (byte)0;

                if (qpaper.Atype == (byte)DgAyAtypeEnum.Cp)
                {
                    qpaper.Atype = (byte)last_CpOv;
                }
            }
            var dbm_DgAyUserQaContent = new List<DgAyUserQaContent>(quesCtns.Count);
            for (var i = 0; i < quesCtns.Count; i++)
            {
                dbm_DgAyUserQaContent.Add(new DgAyUserQaContent 
                {
                    Id = Guid.NewGuid(),
                    Qaid = qaid,
                    IsValid = true,
                    SubmitCount = 1,
                    SubmitTime = qpaper.LastSubmitTime,
                    Num = i + 1,
                    Ctn = ToJsonStr(quesCtns[i], true, true),
                });
            }

            // save
            try
            {
                await _degreeAnalyzeRepository.SaveQpaper(qpaper, dbm_DgAyUserQaContent);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "保存用户答题失败: {args}", new { cmd });
                throw new ResponseResultException("系统繁忙", Errcodes.DgAySaveQaError);
            }

            return (qpaper, quesCtns, totalPoints);
        }

        private async Task On_Analyze(Guid qaid, DgAyUserQaPaper qpaper, List<DgAyQaQuesItemVm> quesCtns, double totalPoints)
        {
            if (qpaper == null)
            {
                // find...
            }
            await default(ValueTask);

            var hasResult = false; // 是否完全有结果
            DgAyQaResultCtnDto resultCtn = null;
            switch ((DgAyAtypeEnum)qpaper.Atype)
            {
                // 对口入学
                case DgAyAtypeEnum.Cp:
                    {
                        var last_q = quesCtns.LastOrDefault(_ => _.Type == (int)DgAyQuestionTypeEnum.Ty5);
                        if (last_q == null) break;
                        var last_opt = last_q.OptTy5;
                        resultCtn = new DgAyQaResultCtnDto();
                        var area = last_opt.Items1.First(_ => _.Selected).Id.Value;
                        var address = last_opt.Items2.First(_ => _.Selected).Title;

                        // 找对应的对口小学
                        var eid_prishool = await _degreeAnalyzeRepository.GetDgAyPrimarySchoolByAreaAddress(area, address);
                        if (eid_prishool == default) break;
                        resultCtn.CpSchoolEid = eid_prishool;
                        resultCtn.Eids.Add(eid_prishool);
                        {
                            // 根据最后的地址或地图经纬度选出来的区
                            var schPcyFile = await _degreeAnalyzeRepository.GetDgAySchPcyFile(area, DgAySchPcyFileTypeEnum.Cp);
                            resultCtn.CpSchPcyFile = schPcyFile;
                        }

                        // 对口直升 
                        var cpals = await _degreeAnalyzeRepository.FindCpPcAssignAndHeliSchoolEids(eid_prishool, resultCtn.CpSchPcyFile?.Year);
                        if (cpals.Counterpart?.Length > 0)
                        {
                            resultCtn.Eids.AddRange(cpals.Counterpart);
                            resultCtn.CpHeliSchoolEids = cpals.Counterpart;
                        }
                        resultCtn.CpHeliSchPcyFile = await _degreeAnalyzeRepository.GetDgAySchPcyFile(area, DgAySchPcyFileTypeEnum.CpHeli);
                        //
                        // 电脑派位
                        if (cpals.Allocation?.Length > 0)
                        {
                            resultCtn.Eids.AddRange(cpals.Allocation);
                            resultCtn.CpPcAssignSchoolEids = cpals.Allocation;
                        }
                        resultCtn.CpPcAssignSchPcyFile = await _degreeAnalyzeRepository.GetDgAySchPcyFile(area, DgAySchPcyFileTypeEnum.CpPcAssign);

                        hasResult = true;
                    }
                    break;

                // 统筹入学
                case DgAyAtypeEnum.Ov:
                    {
                        var last_q = quesCtns.LastOrDefault(_ => _.Type == (int)DgAyQuestionTypeEnum.Ty6);
                        if (last_q == null) break;
                        var last_opt = last_q.OptTy6;
                        var area = Convert.ToInt64(last_opt.Area);
                        resultCtn = new DgAyQaResultCtnDto();
                        {
                            // 根据最后的地址或地图经纬度选出来的区
                            var schPcyFile = await _degreeAnalyzeRepository.GetDgAySchPcyFile(area, DgAySchPcyFileTypeEnum.Ov);
                            resultCtn.OvSchPcyFile = schPcyFile;
                        }
                        // 直径范围3公里内 该坐标所在的区 的公办小学
                        {
                            resultCtn.OvSchoolEids = await _degreeAnalyzeRepository.Find3kmOvPriSchoolEids(last_opt.Lng, last_opt.Lat, area);
                            if (resultCtn.OvSchoolEids?.Length > 0) resultCtn.Eids.AddRange(resultCtn.OvSchoolEids);
                            else resultCtn.OvSchoolEids = null;
                        }
                        hasResult = resultCtn.OvSchoolEids != null;
                    }
                    break;

                // 积分入学
                case DgAyAtypeEnum.Jf:
                    {
                        resultCtn = new DgAyQaResultCtnDto();
                        resultCtn.JfPoints = totalPoints;
                        var area = quesCtns.LastOrDefault(_ => _.Type == (int)DgAyQuestionTypeEnum.Ty7)?.OptTy7?.FirstOrDefault(_ => _.Selected)?.Area;
                        if (area == null) break;
                        var schPcyFile = await _degreeAnalyzeRepository.GetDgAySchPcyFile(area, DgAySchPcyFileTypeEnum.Jf);
                        if (schPcyFile == null) break;
                        var year = schPcyFile.Year;
                        resultCtn.JfSchPcyFile = schPcyFile;
                        {
                            resultCtn.JfSchoolEids = await _degreeAnalyzeRepository.FindTop5JfPriSchoolEids(year, area.Value, totalPoints);
                            if (resultCtn.JfSchoolEids?.Length > 0) resultCtn.Eids.AddRange(resultCtn.JfSchoolEids);
                            else resultCtn.JfSchoolEids = null;
                        }
                        hasResult = resultCtn.JfSchoolEids != null;
                    }
                    break;

                // 查找心仪民办小学
                case DgAyAtypeEnum.Mb:
                    {
                        var eids = await On_Analyze_Find_Mb(quesCtns);
                        if (eids == null || eids.Length <= 0) break;
                        resultCtn = new DgAyQaResultCtnDto();
                        resultCtn.Eids.AddRange(eids);
                        resultCtn.MbSchoolEids = eids;
                        hasResult = true;
                    }
                    break;

                default:
                    hasResult = false;
                    break;
            }

            qpaper.Status = (byte)DgAyStatusEnum.Analyzed;
            qpaper.AnalyzedTime = DateTime.Now;
            qpaper.Title = $"{qpaper.AnalyzedTime:yyyy年MM月dd日}学位分析";
            if (!hasResult)
            {
                qpaper.Status = (byte)DgAyStatusEnum.Unlocked;
                qpaper.UnlockedType = (byte)DgAyUnlockedTypeEnum.NoResult;
                qpaper.UnlockedTime = qpaper.AnalyzedTime;
                // 后续修改 qpaper.Title
            }

            // save 
            try
            {
                var ctn = resultCtn != null ? new DgAyUserQaResultContent() : null;
                if (resultCtn != null)
                {
                    ctn.Id = Guid.NewGuid();
                    ctn.Qaid = qaid;
                    ctn.Ctn = ToJsonStr(resultCtn, true, true);
                    ctn.Eids = ToJsonStr(resultCtn.Eids);
                    ctn.CpSchoolEid = resultCtn.CpSchoolEid;
                    ctn.CpPcAssignSchoolEids = ToJsonStr(resultCtn.CpPcAssignSchoolEids);
                    ctn.CpHeliSchoolEids = ToJsonStr(resultCtn.CpHeliSchoolEids);
                    ctn.OvSchoolEids = ToJsonStr(resultCtn.OvSchoolEids);
                    ctn.JfPoints = resultCtn.JfPoints;
                    ctn.JfSchoolEids = ToJsonStr(resultCtn.JfSchoolEids);
                    ctn.MbSchoolEids = ToJsonStr(resultCtn.MbSchoolEids);
                    ctn.IsValid = true;
                }
                await _degreeAnalyzeRepository.UpQpaperAndAnalyzedResult(qpaper, ctn);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "分析用户答题失败: {args}", new { qpaper, resultCtn });
                throw new ResponseResultException("系统繁忙", Errcodes.DgAyQaAnalyzeError);
            }
        }

        // 找民办
        private async Task<Guid[]> On_Analyze_Find_Mb(List<DgAyQaQuesItemVm> quesCtns)
        {
            var conditions = quesCtns.Select(x =>
            {
                if (x.Opts != null)
                {
                    return x.Opts.Where(_ => _.Selected).Select(_ => (_.FindField, _.FindFieldFw, _.FindFieldFwJx));
                }
                if (x.OptTy7 != null)
                {
                    return x.OptTy7.Where(_ => _.Selected).Select(_ => (_.FindField, _.FindFieldFw, _.FindFieldFwJx));
                }
                return default;
            }).SelectMany(_ => _).Where(_ => _ != default).ToArray();

            if (conditions.Length <= 0) return null;
            return await _degreeAnalyzeRepository.FindMbPriSchoolEids(conditions);
        }

        static string ToJsonStr(object obj, bool ignoreCase = false, bool ignoreNull = false, bool indented = false)
        {
            if (obj == null) return null;
            var options = new JsonSerializerSettings();
            if (ignoreCase) options.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            options.NullValueHandling = ignoreNull ? NullValueHandling.Ignore : NullValueHandling.Include;
            if (indented) options.Formatting = Formatting.Indented;
            return JsonConvert.SerializeObject(obj, options);
        }

        
    }
}
