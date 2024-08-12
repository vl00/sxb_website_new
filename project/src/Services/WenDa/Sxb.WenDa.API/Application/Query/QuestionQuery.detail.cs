using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Foundation;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Consts;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.RequestDto;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.SQL.Repositories;

namespace Sxb.WenDa.API.Application.Query
{
    public partial class QuestionQuery : IQuestionQuery
    {
        public async Task<LoadQuestionForSaveDto> GetQuestionByIdForSave(Guid? id)
        {
            var result = new LoadQuestionForSaveDto { Id = id };

            (Common.Entity.Question question, Guid[] eids, long[] tagIds) = (null, null, null);
            if (id != null)
            {
                (question, eids, tagIds) = await _questionRepository.LoadQuestion(id.Value);
                if (question == null) throw new ResponseResultException("问题不存在", Errcodes.Wenda_NotFound);
                result.Id = question.Id;
                result.Title = question.Title;
                result.Content = question.Content;
                result.Imgs = question.Imgs?.FromJson<string[]>() ?? Array.Empty<string>();
                result.Imgs_s = question.Imgs_s?.FromJson<string[]>() ?? Array.Empty<string>();
                result.IsAnony = question.IsAnony;
            }

            result.Citys = default!;
            {
                var citys = await _cityCategoryQuery.GetCitys();
                result.Citys = citys.Select(_ => new LoadQuestionForSaveDto_City 
                { 
                    Id = _.Id, Name = _.Name,
                    Selected = question?.City == _.Id,
                }).ToArray();
            }
            
            // 学校
            if (eids?.Length > 0)
            {
                var schools = await _schoolApiService.GetSchoolsIdAndName(eids.Select(_ => _.ToString()));
                if (schools?.Count > 0)
                {
                    result.Schools = schools //.Where(_ => _.IsValid)
                        .Select(_ => new LoadQuestionForSaveDto_School { Eid = _.Eid, SchoolName = $"{_.Schname}-{_.Extname}" }).ToArray();
                }
            }

            // 分类 标签
            if (question?.CategoryId != null)
            {
                var ctgs = await _cityCategoryQuery.GetCityCategory(new GetCityCategoryQuery { City = question.City, CategoryId = question.CategoryId ?? 0 });

                result.Tags = ctgs.Tags == null ? null : ctgs.Tags.Select(x => new CategoryTagDto { Id = x.Id, Name = x.Name, Selected = tagIds?.Contains(x.Id) == true }).ToArray();

                var category = await _cityCategoryRepository.GetCategory(question.CategoryId ?? 0);
                if (category == null) throw new ResponseResultException("分类不存在", Errcodes.Wenda_NotFound);

                var pcids = $"/0/{category.Path}".Split('/', StringSplitOptions.RemoveEmptyEntries).Select(_ => Convert.ToInt64(_)).Reverse();
                var cid = category.Id;
                foreach (var pid in pcids)
                {
                    if (pid == category.Id) continue;
                    ctgs = await _cityCategoryQuery.GetCityCategory(new GetCityCategoryQuery { City = question.City, CategoryId = pid });
                    var cCategories = ctgs.ChildrenCategories.Select(x => new CategoryDto { Id = x.Id, Name = x.Name, Selected = x.Id == cid, CanFindSchool = x.CanFindSchool }).ToArray();
                    result.Categories ??= new List<CategoryDto[]>();
                    result.Categories.Insert(0, cCategories);
                    cid = pid;
                }
            }

            return result;
        }

        public async Task<QuestionDetailVm> GetQuestionDetail(string questionId, string answerId, Guid userId = default)
        {
            var result = new QuestionDetailVm();
            var _answerQuery = _services.GetService<IAnswerQuery>();

            var qid = Guid.TryParse(questionId, out var _qid) ? _qid : default;
            var qno = qid == default ? UrlShortIdUtil.Base322Long(questionId) : default;

            result.Question = (await GetQaItemDtos<QaQuestionItemDto>(new[] { qid }, new[] { qno }, userId)).FirstOrDefault();
            if (result.Question == null) throw new ResponseResultException("问题不存在", Errcodes.Wenda_QuestionNotExists);

            result.Cities = (await _cityCategoryQuery.GetCitys()).Where(_ => _.IsOpen).ToArray();

            // 分享
            if (!string.IsNullOrEmpty(answerId))
            {
                var aid = Guid.TryParse(answerId, out var _aid) ? _aid : default;
                var ano = aid == default ? UrlShortIdUtil.Base322Long(answerId) : default;
                result.SharedAnswer = (await _answerQuery.GetQaAnswerItemDtos(new[] { aid }, new[] { ano }, userId)).FirstOrDefault();
            }

            // 前端另外调接口
            //result.RecommendQuestions = 
            //result.RecommendQuestions = 

            return result;
        }

        public async Task<EveryoneTalkingAboutDetailVm> GetEveryoneTalkingAboutDetail()
        {
            // 不用查条件 城市
            var result = new EveryoneTalkingAboutDetailVm();

            result.Cities = (await _cityCategoryQuery.GetCitys()).Where(_ => _.IsOpen).ToArray();

            // 前端另外调接口
            //result.HotRecommends = 

            return result;
        }

    }
}
