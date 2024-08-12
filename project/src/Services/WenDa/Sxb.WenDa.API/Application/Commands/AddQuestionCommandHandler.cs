using MediatR;
using Polly;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Extensions;
using Sxb.WenDa.API.Utils;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.OtherAPIClient.Org;
using Sxb.WenDa.Common.OtherAPIClient.School;
using Sxb.WenDa.Common.OtherAPIClient.User;
using Sxb.WenDa.Query.SQL.Repositories;
using System.Diagnostics;

namespace Sxb.WenDa.API.Application.Commands
{
    public class AddQuestionCommandHandler : IRequestHandler<AddQuestionCommand, AddQuestionCommandResult>
    {
        readonly IQuestionRepository _questionRepository;
        readonly ICityCategoryQuery _cityCategoryQuery;
        readonly IQaSubjectRepository _qaSubjectRepository;
        readonly ICityCategoryRepository _cityCategoryRepository;
        readonly ILogger log;
        readonly ISchoolApiService _schoolApiService;
        readonly IEasyRedisClient _easyRedisClient;
        readonly IUserQuery _userQuery;

        public AddQuestionCommandHandler(IQuestionRepository questionRepository, ICityCategoryQuery cityCategoryQuery, 
            IQaSubjectRepository qaSubjectRepository, ICityCategoryRepository cityCategoryRepository, ILoggerFactory loggerFactory,
            IUserQuery userQuery, ISchoolApiService schoolApiService, IEasyRedisClient easyRedisClient,
            IServiceProvider services)
        {
            _questionRepository = questionRepository;
            _cityCategoryQuery = cityCategoryQuery;
            _qaSubjectRepository = qaSubjectRepository;
            _cityCategoryRepository = cityCategoryRepository;
            this.log = loggerFactory.CreateLogger(this.GetType());
            _userQuery = userQuery;
            _schoolApiService = schoolApiService;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<AddQuestionCommandResult> Handle(AddQuestionCommand cmd, CancellationToken cancellationToken)
        {
            var result = new AddQuestionCommandResult();
            Guid? subjectId = null;
            var city = cmd.City;
            string category_path = null;
            cmd.Title = cmd.Title?.Trim();
            await default(ValueTask);

            if (string.IsNullOrEmpty(cmd.Title)) throw new ResponseResultException("标题不能为空", 201);
            if (cmd.Title.Length > 40) throw new ResponseResultException("标题最多可输入40个字符", 201);
            if (cmd.City == 0) throw new ResponseResultException("请选择城市", 201);
            if (cmd.CategoryId == 0) throw new ResponseResultException("请选择分类", 201);
            if (cmd.Imgs?.Length != cmd.Imgs_s?.Length) throw new ResponseResultException("图片参数错误", 201);
            if (cmd.Imgs?.Length > 6) throw new ResponseResultException("最多6张图", 201);
            if (cmd.TagIds?.Length > 3) throw new ResponseResultException("最多3个标签", 201);
            if (cmd.Eids?.Length > 3) throw new ResponseResultException("最多3个学校", 201);

            // 标题
            if (cmd.Title != null && await _questionRepository.IsTitleExists(cmd.Title))
            {
                throw new ResponseResultException("已存在相同标题的问题", Errcodes.Wenda_QuesTitleExists);
            }
            // 城市
            if (!(await _cityCategoryQuery.GetCitys()).Any(_ => _.IsOpen && _.Id == city))
            {
                throw new ResponseResultException("该城市的问答广场未开放", Errcodes.Wenda_CityIsNotOpen);
            }
            // 专栏
            if (!string.IsNullOrEmpty(cmd.SubjectId))
            {
                subjectId = Guid.TryParse(cmd.SubjectId, out var _subjectId) ? _subjectId : default;
                var subjectNo = (subjectId == null || subjectId == Guid.Empty) && long.TryParse(cmd.SubjectId, out var _subjectNo) ? _subjectNo : default;
                var subject = await _qaSubjectRepository.GetSubject(subjectId ?? default, subjectNo);
                if (subject?.IsValid != true) throw new ResponseResultException("专栏不存在", Errcodes.Wenda_SubjectNotExists);
                subjectId = subject.Id;
            }

            // CategoryId tagids eids
            //Debugger.Break();
            {
                var category = await _cityCategoryRepository.GetCategory(cmd.CategoryId);
                if (category?.IsValid != true) throw new ResponseResultException("分类不存在", Errcodes.Wenda_CategoryNotExists);
                category_path = category.Path;

                var allTags = (await _cityCategoryRepository.GetCategories(city, category_path, true)).ToList();
                if (!allTags.Any()) throw new ResponseResultException("城市与分类不匹配", Errcodes.Wenda_CityNotHasThisCategory);
                allTags.RemoveAll(_ => _.Id == category.Id);

                if (allTags.Any(_ => _.Type == (byte)CategoryOrTagEnum.Category)) 
                    throw new ResponseResultException("分类参数错误", Errcodes.Wenda_CategoryHasChildCategory);
                if (allTags.Count < (cmd.TagIds?.Length ?? 0))
                    throw new ResponseResultException("标签参数错误", Errcodes.Wenda_TagIdError);
                if (cmd.TagIds != null && cmd.TagIds.Any(t => !allTags.Any(_ => _.Id == t)) == true)
                    throw new ResponseResultException("标签中包含不在此分类里的标签", Errcodes.Wenda_TagIdError);

                if (category.CanFindSchool == true)
                {
                    if ((cmd.Eids?.Length ?? 0) <= 0)
                        throw new ResponseResultException("需要学校参数", 201);
                    if (cmd.Eids.Length > 3)
                        throw new ResponseResultException("最多3个学校", 201);

                    // check school eids
                    var (schools, ex) = await _schoolApiService.GetSchoolsIdAndName(cmd.Eids.Select(_ => _.ToString())).AwaitResOrErr();
                    if (ex != null) throw new ResponseResultException(ex.Message, Errcodes.Wenda_CallApiError);
                    if (!schools.Any()) throw new ResponseResultException("学校参数错误", 201);
                    //
                    // check school isValid
                    var s2 = schools.Where(_ => _.IsValid);
                    if (!s2.Any()) throw new ResponseResultException("学校已下架", 201);
                    cmd.Eids = s2.Select(_ => _.Eid).ToArray();
                }
                else cmd.Eids = null;
            }

            //
            // check ok
            //

            var task_user = _userQuery.GetRealUser(cmd.UserId);

            //Debugger.Break();
            Question question = new();
            question.Id = Guid.NewGuid();
            question.IsValid = true;
            question.CreateTime = DateTime.Now;            
            question.UserId = cmd.UserId;
            question.ModifyDateTime = question.CreateTime;
            question.Modifier = question.UserId;
            question.LastEditTime = null;
            question.EditCount = 0;
            question.Title = cmd.Title;
            question.IsAnony = cmd.IsAnony;
            question.AnonyUserName = !cmd.IsAnony ? null : BusinessLogicUtils.RandomAnonyUserName();
            question.SubjectId = subjectId;
            question.Platform = long.TryParse(category_path.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(), out var _cid1) ? _cid1 : 0;
            question.City = city;
            question.CategoryId = cmd.CategoryId;
            question.Content = cmd.Content;
            question.Imgs = cmd.Imgs.ToJsonStr(true);
            question.Imgs_s = cmd.Imgs_s.ToJsonStr(true);            
            //
            List<QuestionEids> quesionEids = null;
            if (cmd.Eids?.Length > 0)
            {
                quesionEids = new();
                quesionEids.AddRange(cmd.Eids.Select(_ => new QuestionEids { Id = Guid.NewGuid(), Qid = question.Id, Eid = _ }));
            }
            //
            List<QuestionTag> questionTags = null;
            if (cmd.TagIds?.Length > 0)
            {
                questionTags = new();
                questionTags.AddRange(cmd.TagIds.Select(_ => new QuestionTag { Qid = question.Id, TagId = _ }));
            }

            if (cmd.UserId != default)
            {
                var gzDto = await task_user;
                //result.HasGzWxGzh = gzDto?.HasGzWxGzh ?? false;
                //result.HasJoinWxEnt = gzDto?.HasJoinWxEnt ?? false;

                question.IsRealUser = gzDto?.IsRealUser ?? false;
            }

            try
            { 
                await _questionRepository.AddQuestion(question, quesionEids, questionTags);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "发问题失败 {@cmd}", cmd);
                throw;
            }

            try
            {
                await Policy.Handle<Exception>()
                    .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600))
                    .ExecuteAsync(async () =>
                    {
                        var q1 = await _questionRepository.GetQuestion(id: question.Id);
                        if (q1 == null) throw new Exception("write-read not sync");
                        //question.No = q1.No;
                    });
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "added question Id={qid}", question.Id);
                throw new ResponseResultException($"系统繁忙:{Errcodes.Wenda_WriteReadNotSync}", Errcodes.Wenda_WriteReadNotSync);
            }

            // clear cache
            await _easyRedisClient.DelRedisKeys(new[]
            {
                string.Format(CacheKeys.MyQuestionCount, question.UserId),
                string.Format(CacheKeys.MyAll, question.UserId),
                question.SubjectId == null ? null : string.Format(CacheKeys.Subject, question.SubjectId),
                question.SubjectId == null ? null : string.Format(CacheKeys.Subject, question.SubjectId) + ":*",
                CacheKeys.QuestionsAll,
                //CacheKeys.WendaAll,
            });

            result.QuestionId = question.Id;
            result.QuestionNo = UrlShortIdUtil.Long2Base32(question.No);
            return result;
        }
    }
}
