using Sxb.Framework.Foundation;
using Sxb.WenDa.Common.ResponseDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.SQL.QueryDto
{
    public class QuestionCategoryQueryDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 问题短链
        /// </summary>
        public long No { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 问题分类
        /// </summary>
        public string CategoryName { get; set; }

        public QuestionCategoryItemDto Convert()
        {
            return new QuestionCategoryItemDto()
            {
                Id = Id,
                QuestionNo = UrlShortIdUtil.Long2Base32(No),
                QuestionTitle = Title,
                CategoryName = CategoryName,
            };
        }
    }
}
