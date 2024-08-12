using Dapper;
using Sxb.Comment.Common.DB;
using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.Enum;
using Sxb.Comment.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.Repository
{
    public class SchoolQuestionRepository : ISchoolQuestionRepository
    {
        readonly SchoolProductDB _schoolProductDB;
        public SchoolQuestionRepository(SchoolProductDB schoolProductDB)
        {
            _schoolProductDB = schoolProductDB;
        }

        public QuestionDTO ConvertToQuestionDTO(QuestionInfo questionInfo, Guid userID, IEnumerable<GiveLikeInfo> likes, IEnumerable<SchoolImageInfo> images, IEnumerable<AnswerInfoDTO> answers = null)
        {
            if (questionInfo == null) return null;
            return new QuestionDTO()
            {
                ID = questionInfo.ID,
                No = questionInfo.No,
                UserID = questionInfo.UserID,
                AnswerCount = questionInfo.ReplyCount,
                LikeCount = questionInfo.LikeCount,
                IsLike = likes.FirstOrDefault(q => q.SourceID == questionInfo.ID) != null,
                QuestionCreateTime = questionInfo.CreateTime,
                QuestionContent = questionInfo.Content,
                Answer = answers?.Where(x => x.QuestionID == questionInfo.ID)?.ToList(),
                Images = images?.Where(q => q.DataSourcetID == questionInfo.ID)?.Select(x => x.ImageUrl)?.ToList(),
                SchoolID = questionInfo.SchoolID,
                SchoolSectionID = questionInfo.SchoolSectionID,
                IsAnony = questionInfo.IsAnony
            };
        }

        public async Task<IEnumerable<SchoolQuestionTotalDTO>> GetQuestionTotalsByEID(Guid eid)
        {
            if (eid == default) return null;
            var str_SQL = @"
            select 1 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and QuestionInfos.State in (0,1,2,3)
            UNION
            select 2 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'辟谣') and QuestionInfos.State in (0,1,2,3)
            union
            select 3 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'师资') and QuestionInfos.State in (0,1,2,3)
            union
            select 4 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'硬件') and QuestionInfos.State in (0,1,2,3)
            union
            select 5 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'环境') and QuestionInfos.State in (0,1,2,3)
            union
            select 6 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'学分') and QuestionInfos.State in (0,1,2,3)
            union
            select 7 as TotalType,count(1) as Total,null as SchoolSectionId from QuestionInfos where SchoolSectionId = @eid and CONTAINS(Content,'校园') and QuestionInfos.State in (0,1,2,3)";
            return await _schoolProductDB.SlaveConnection.QueryAsync<SchoolQuestionTotalDTO>(str_SQL, new { eid });
        }

        public async Task<IEnumerable<QuestionInfo>> GetSchoolSelectedQuestion(IEnumerable<Guid> schoolSectionIDs, SelectedQuestionOrderType order, int take = 1)
        {
            if (schoolSectionIDs == null || !schoolSectionIDs.Any()) return null;
            string str_Order;
            if (order == SelectedQuestionOrderType.CreateTime)
            {
                str_Order = " CreateTime desc ";
            }
            else
            {
                str_Order = " IsTop, isSchoolUser DESC, LikeCount DESC";
            }
            var str_SQL = $"SELECT top {take} *, CASE WHEN PostUserRole = 1 THEN 1 ELSE 0 END AS isSchoolUser FROM QuestionInfos WHERE SchoolSectionID = @schoolSectionID AND State < 4 order by" + str_Order;
            //if (order == SelectedQuestionOrderType.CreateTime)
            //{
            //    str_SQL += "  order by CreateTime desc";
            //}
            var schoolSectionID = schoolSectionIDs.First();
            return await _schoolProductDB.SlaveConnection.QueryAsync<QuestionInfo>(str_SQL, new { schoolSectionID });
        }
    }
}
