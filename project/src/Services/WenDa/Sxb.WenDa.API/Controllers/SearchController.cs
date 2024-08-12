using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.WenDa.API.Application.Commands;
using Sxb.WenDa.API.Application.Query;
using Sxb.WenDa.API.Filters;
using Sxb.WenDa.API.RequestContact.Wenda;
using Sxb.WenDa.Common.Enums;
using Sxb.WenDa.Common.ResponseDto;
using Sxb.WenDa.Query.ElasticSearch.Base;
using Sxb.WenDa.Query.ElasticSearch.Models;
using Sxb.WenDa.Query.ElasticSearch.QueryModels;

namespace Sxb.WenDa.API.Controllers
{

    /// <summary>
    /// 搜索
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ApiControllerBase
    {
        private readonly ISearchQuery _searchQuery;
        private readonly IQuestionQuery _questionQuery;
        private readonly ISubjectQuery _subjectQuery;

        public SearchController(ISearchQuery searchQuery, IQuestionQuery questionQuery, ISubjectQuery subjectQuery)
        {
            _searchQuery = searchQuery;
            _questionQuery = questionQuery;
            _subjectQuery = subjectQuery;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>        
        [HttpGet("aggregate")]
        [ProducesResponseType(typeof(SearchDto), 200)]
        //[ProducesResponseType(typeof(QaQuestionListItemDto), 200)]
        public async Task<ResponseResult> AggregateSearchAsync([FromQuery] AggregateQueryModel queryModel)
        {
            var page = await _searchQuery.AggregateSearchAsync(queryModel);

            //问题列表
            var questionIds = page.Data.Where(s => s.RefTable == RefTable.Question).Select(s => s.Id).ToList();
            var questions = (await _questionQuery.GetQaQuestionListItemDtos(questionIds, UserId)).ToList();
            SetHighlights(ref questions, queryModel.Keyword);

            //专栏列表
            var subjectIds = page.Data.Where(s => s.RefTable == RefTable.Subject).Select(s => s.Id).ToList();
            var subjects = (await _subjectQuery.GetSubjectItems(subjectIds, UserId)).ToList();
            HighlightHelper.SetHighlights(ref subjects, queryModel.Keyword, nameof(SubjectItemDto.Title));
            HighlightHelper.SetHighlights(ref subjects, queryModel.Keyword, nameof(SubjectItemDto.Content));

            //按原顺序
            var newData = page.Data.Select(s =>
            {
                if (s.RefTable == RefTable.Question)
                    return SearchDto.Create(s.RefTable, questions.FirstOrDefault(q => q.QuestionId == s.Id));
                else
                    return SearchDto.Create(s.RefTable, subjects.FirstOrDefault(q => q.SubjectId == s.Id));
            });
            return ResponseResult.Success(page.ChangeData(newData));
        }


        /// <summary>
        /// <para>added by Labbor on 20201130 设置高亮字段到原数据</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="keyword"></param>
        /// <param name="name"></param>
        public static void SetHighlights(ref List<QaQuestionListItemDto> source, string keyword)
        {
            if (source == null || !source.Any())
            {
                return;
            }

            var type = typeof(QaQuestionListItemDto);
            var propTitle = type.GetProperty(nameof(QaQuestionListItemDto.Title));
            var propSchools = type.GetProperty(nameof(QaQuestionListItemDto.SchoolNames));


            var length = source.Count;
            for (int position = 0; position < length; position++)
            {
                var item = source[position];

                var propValue = propTitle.GetValue(item);
                var propStrValue = propValue == null ? string.Empty : propValue.ToString();

                //把高亮数据赋值到原数据
                var highValue = HighlightHelper.GetHighLightValue(keyword, propStrValue, def: propStrValue);
                propTitle.SetValue(item, highValue);


                if (propSchools.GetValue(item) is List<string> propSchoolsValue && propSchoolsValue?.Any() == true)
                {
                    var highSchoolsValue = propSchoolsValue.Select(s => HighlightHelper.GetHighLightValue(keyword, s, def: s)).ToList();
                    propSchools.SetValue(item, highSchoolsValue);
                }
            }
        }

    }
}
