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
    public class EditQuestionCommandHandler : IRequestHandler<EditQuestionCommand, EditQuestionCommandResult>
    {
        readonly IQuestionRepository _questionRepository;
        readonly ICityCategoryQuery _cityCategoryQuery;
        readonly IQaSubjectRepository _qaSubjectRepository;
        readonly ICityCategoryRepository _cityCategoryRepository;
        readonly ILogger log;
        readonly IUserQuery _userQuery;
        readonly ISchoolApiService _schoolApiService;
        readonly IEasyRedisClient _easyRedisClient;

        public EditQuestionCommandHandler(IQuestionRepository questionRepository, ICityCategoryQuery cityCategoryQuery, 
            IQaSubjectRepository qaSubjectRepository, ICityCategoryRepository cityCategoryRepository, ILoggerFactory loggerFactory,
            IUserQuery userQuery, ISchoolApiService schoolApiService, IEasyRedisClient easyRedisClient,
            IServiceProvider services)
        {
            _questionRepository = questionRepository;
            _cityCategoryQuery = cityCategoryQuery;
            _qaSubjectRepository = qaSubjectRepository;
            _cityCategoryRepository = cityCategoryRepository;
            this.log = loggerFactory.CreateLogger(GetType());
            _userQuery = userQuery;
            _schoolApiService = schoolApiService;       
            _easyRedisClient = easyRedisClient;
        }

        public async Task<EditQuestionCommandResult> Handle(EditQuestionCommand cmd, CancellationToken cancellationToken)
        {
            var result = new EditQuestionCommandResult();            
            var city = cmd.City;
            string category_path = null; 
            await default(ValueTask);
            
            if (cmd.City == 0) throw new ResponseResultException("请选择城市", 201);
            if (cmd.CategoryId == 0) throw new ResponseResultException("请选择分类", 201);
            if (cmd.Imgs?.Length != cmd.Imgs_s?.Length) throw new ResponseResultException("图片参数错误", 201);
            if (cmd.Imgs?.Length > 6) throw new ResponseResultException("最多6张图", 201);
            if (cmd.TagIds?.Length > 3) throw new ResponseResultException("最多3个标签", 201);
            if (cmd.Eids?.Length > 3) throw new ResponseResultException("最多3个学校", 201);

            // 城市
            if (!(await _cityCategoryQuery.GetCitys()).Any(_ => _.IsOpen && _.Id == city))
            {
                throw new ResponseResultException("该城市的问答广场未开放", Errcodes.Wenda_CityIsNotOpen);
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

            //Debugger.Break();
            var question = await _questionRepository.GetQuestion(cmd.Id, default);
            if (question?.IsValid != true) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);
            if (question.UserId != cmd.UserId) throw new ResponseResultException("不是我的问题", Errcodes.Wenda_IsNotMyQuestion);

            //
            // check ok
            //
            var task_user = _userQuery.GetRealUser(cmd.UserId);
            
            question.LastEditTime = DateTime.Now;
            question.EditCount += 1;
            question.Modifier = question.UserId;
            question.ModifyDateTime = question.LastEditTime;
            question.IsAnony = cmd.IsAnony;
            //question.AnonyUserName = !cmd.IsAnony ? null : AnonyUserUtils.RndName();
            question.Platform = long.TryParse(category_path.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(), out var _cid1) ? _cid1 : 0;
            question.City = city;
            question.CategoryId = cmd.CategoryId;
            question.Content = cmd.Content;
            question.Imgs = cmd.Imgs.ToJson();
            question.Imgs_s = cmd.Imgs_s.ToJson();
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

            try
            {
                await task_user;
                await _questionRepository.EditQuestion(question, quesionEids, questionTags);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "编辑问题失败 {@cmd}", cmd);
                throw;
            }

            try
            {
                await Policy.Handle<Exception>()
                    .WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600))
                    .ExecuteAsync(async () =>
                    {
                        var q1 = await _questionRepository.GetQuestion(question.Id, default);
                        if (q1?.EditCount != question.EditCount) throw new Exception($"write-read not sync. read='{q1?.EditCount}' write='{question.EditCount}' ");
                        //question.No = q1.No;
                    });
            }
            catch (Exception ex)
            {
                log.LogWarning(ex, "edited question Id={qid}", question.Id);
                throw new ResponseResultException($"系统繁忙:{Errcodes.Wenda_WriteReadNotSync}", Errcodes.Wenda_WriteReadNotSync);
            }

            if (cmd.UserId != default)
            {
                //var gzDto = await task_user;
                //result.HasGzWxGzh = gzDto?.HasGzWxGzh ?? false;
                //result.HasJoinWxEnt = gzDto?.HasJoinWxEnt ?? false;
            }

            // clear cache
            await _easyRedisClient.DelRedisKeys(new[]
            {
                string.Format(CacheKeys.Question, question.Id),
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
