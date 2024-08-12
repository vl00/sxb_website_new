using Sxb.WenDa.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Query.SQL.Helper
{
    internal class LocalSqlHelper
    {
        /// <summary>
        /// 判断是否存在这些分类(包括其子分类)
        /// </summary>
        /// <param name="categoryIds"></param>
        /// <returns></returns>
        internal static string GetCategoryExistsSql(IEnumerable<long> categoryIds, string questionAlias = "")
        {
            if (categoryIds == null || !categoryIds.Any())
            {
                return string.Empty;
            }

            var categorySql = new StringBuilder();

            var aliasSql = !string.IsNullOrWhiteSpace(questionAlias) ? questionAlias + "." : "";
            //分类及其所有子分类
            categorySql.Append($"and {aliasSql}CategoryId IN (")
                    .Append(" select c.Id from Category c where ");


            var isFirst = true;
            foreach (var id in categoryIds)
            {
                //and charindex('/15/',c.path)>0
                if (isFirst) { 
                    isFirst = false;

                    categorySql
                        .Append("charindex('/")
                        .Append(id)
                        .Append("/',c.path)>0 ");
                }
                else
                {
                    categorySql
                        .Append("or charindex('/")
                        .Append(id)
                        .Append("/',c.path)>0 ");
                }

            }
            categorySql.Append(')');

            return categorySql.ToString();
        }

        internal static string GetQuestionOrderBySql(QuestionOrderBy orderBy, string questionAlias = "")
        {
            var aliasSql = !string.IsNullOrWhiteSpace(questionAlias) ? questionAlias + "." : "";

            return orderBy switch
            {
                QuestionOrderBy.CreateTimeDesc => $"{aliasSql}CreateTime DESC",
                QuestionOrderBy.ReplyDesc => $"{aliasSql}ReplyCount DESC, {aliasSql}LikeTotalCount DESC",
                QuestionOrderBy.LikeTotalDesc => $"{aliasSql}LikeTotalCount DESC",
                _ => $"{aliasSql}Id",
            };
        }
    }
}
