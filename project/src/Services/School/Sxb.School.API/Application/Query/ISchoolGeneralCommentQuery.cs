using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolGeneralCommentQuery
    {
        /// <summary>
        /// 根据学部eid查询学校总评
        /// </summary>
        /// <param name="eids">学部eid或学部短id</param>
        /// <returns></returns>
        Task<List<SchoolGeneralCommentDto>> QueryGeneralCommentByeids(IEnumerable<string> eids);
        /// <summary>
        /// 根据年级显示院校库中总评分数最高的前10所学校
        /// </summary>
        /// <param name="grade"></param>
        /// <returns></returns>
        Task<List<SchoolGeneralCommentDto>> QueryTop10GeneralCommentByGrade(int grade);
    }
}
