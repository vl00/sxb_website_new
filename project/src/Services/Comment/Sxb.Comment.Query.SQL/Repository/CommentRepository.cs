using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Comment.Common.DB;
using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.EntityExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.Query.SQL.IRepository
{
    public class CommentRepository : ICommentRepository
    {
        readonly SchoolProductDB _schoolProductDB;
        public CommentRepository(SchoolProductDB schoolProductDB)
        {
            _schoolProductDB = schoolProductDB;
        }

        public async Task<IEnumerable<SchoolCommentScoreInfo>> GetCommentScoresByCommentIDs(IEnumerable<Guid> commentIDs)
        {
            if (commentIDs == default || !commentIDs.Any()) return null;
            return await _schoolProductDB.SlaveConnection.QuerySet<SchoolCommentScoreInfo>().Where($"CommentID In('{string.Join("','", commentIDs)}')").ToIEnumerableAsync();
        }

        public async Task<IEnumerable<CommentTagExtend>> GetCommentTagsByCommentIDs(IEnumerable<Guid> commentIDs)
        {
            if (commentIDs == default || !commentIDs.Any()) return null;
            return await _schoolProductDB.SlaveConnection.QuerySet<CommentTagInfo>().Join<CommentTagInfo, TagInfo>((x, y) => x.TagID == y.ID).Where($"SchoolCommentID In ('{string.Join("','", commentIDs)}')").ToIEnumerableAsync<CommentTagExtend>();
        }

        public async Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalBySID(Guid sid)
        {
            if (sid == default) return null;
            string sql = @"
                            SELECT
	                        1 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND State in (0,1,2,3)
	                        AND e.IsValid = 1 
UNION
                        SELECT
	                        2 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND ( State = 3 OR ReplyCount > 10 ) 
	                        AND e.IsValid = 1 
	                        AND State in (0,1,2,3) 
UNION
                        SELECT
	                        3 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments AS c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id
	                        LEFT JOIN SchoolCommentScores AS s ON c.Id = s.CommentId 
                        WHERE
	                        s.IsAttend = 1 
	                        AND c.SchoolId = @SchoolId 
	                        AND c.State in (0,1,2,3)
	                        AND e.IsValid = 1 
UNION
                        SELECT
	                        4 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        SchoolId = @SchoolId 
	                        AND RumorRefuting = 1 
	                        AND State in (0,1,2,3) 
	                        AND e.IsValid = 1 
UNION
                        SELECT
	                        5 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments AS s
	                        LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON s.SchoolSectionId = e.id 
                        WHERE
	                        s.SchoolId = @SchoolId 
	                        AND c.AggScore >= 80 
	                        AND s.State in (0,1,2,3) 
	                        AND e.IsValid = 1 
UNION
                        SELECT
	                        6 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments AS s
	                        LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON s.SchoolSectionId = e.id 
                        WHERE
	                        s.SchoolId = @SchoolId 
	                        AND c.AggScore <= 40 
	                        AND s.State in (0,1,2,3)
	                        AND e.IsValid = 1 
UNION
                        SELECT
	                        7 AS TotalType,
	                        COUNT(1) AS Total,
	                        null AS SchoolSectionId 
                        FROM
	                        SchoolComments as c
	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
                        WHERE
	                        IsHaveImagers = 1 
	                        AND SchoolId = @SchoolId 
	                        AND State in (0,1,2,3) 
	                        AND e.IsValid = 1 ";
            //UNION
            //                        SELECT
            //	                        8 AS TotalType,
            //	                        COUNT(1) AS Total,
            //	                        SchoolSectionId 
            //                        FROM
            //	                        SchoolComments AS c
            //	                        LEFT JOIN [iSchoolData].[dbo].[OnlineSchoolExtension] AS e ON c.SchoolSectionId = e.id 
            //                        WHERE
            //	                        c.SchoolId = @SchoolId 
            //	                        AND c.State in (0,1,2,3) 
            //	                        AND e.IsValid = 1 
            //                        GROUP BY
            //	                        c.SchoolSectionId";


            return await _schoolProductDB.SlaveConnection.QueryAsync<SchoolCommentTotalDTO>(sql, new { SchoolId = sid });
        }

        public async Task<IEnumerable<SchoolCommentInfo>> GetSchoolSelectedComment(Guid eid, int order, int take = 0)
        {
            if (eid == default) return null;
            var str_SQL = string.Empty;
            var str_Take = string.Empty;
            if (take > 0) str_Take = $"top {take}";
            if (order == 3 || order == -1)
            {
                str_SQL = @$"select {str_Take} s.* from SchoolComments s
	                        RIGHT JOIN
                        (
	                        SELECT
		                        Id,
	                        CASE

			                        WHEN PostUserRole = '1' THEN
			                        1 ELSE 0
		                        END AS isSchoolUser,
		                        ROW_NUMBER() OVER(partition BY SchoolSectionId order by IsTop DESC,( LikeCount + ReplyCount ) DESC) AS Row_Index
	                        FROM
		                        SchoolComments
	                        WHERE
		                        SchoolSectionId = @eid
		                        AND State in (0,1,2,3)
                        ) as hot
	                        on s.Id = hot.Id where hot.Row_Index = 1
		                        order by hot.isSchoolUser desc";
            }
            else if (order == 1)
            {
                str_SQL = @$"select {str_Take} s.* from SchoolComments as s
			                RIGHT JOIN
		                (select
			                Id,
		                ROW_NUMBER() OVER( partition BY SchoolSectionId order by AddTime desc) AS Row_Index
		                from SchoolComments
			                where SchoolSectionId = @eid
                            AND State in (0,1,2,3)
		                ) as new on new.Id = s.Id";
                if (take < 2) str_SQL += " where new.Row_Index = 1";
            }
            if (string.IsNullOrWhiteSpace(str_SQL)) return null;
            return await _schoolProductDB.SlaveConnection.QueryAsync<SchoolCommentInfo>(str_SQL, new { eid });
        }


        public async Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids)
        {
            var sql = $@"
					SELECT sc.SchoolSectionId as eid, count(1) as commentCount
					 FROM
					   SchoolCommentScores AS ssc
					   LEFT JOIN SchoolComments AS sc ON ssc.CommentId = sc.ID    
					 WHERE
					   sc.SchoolSectionId in @eids
					   AND sc.State != 4
					group by sc.SchoolSectionId
				";
            var ls = await _schoolProductDB.SlaveConnection.QueryAsync<SchoolScoreCommentCountDto>(sql, new { eids });
            return ls;
        }

        public async Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalByEIDsAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
      //          var str_SQL = $@"SELECT
						//			1 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments AS c
						//		WHERE
						//			c.State in (0,1,2,3)
						//			AND c.SchoolSectionId in @eids
						//UNION
						//		SELECT
						//			2 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId
						//		FROM
						//			SchoolComments as c
						//		WHERE
						//			(State = 3 OR ReplyCount > 10) 
						//			AND c.SchoolSectionId in @eids
						//			AND State in (0,1,2,3) 
						//UNION
						//		SELECT
						//			3 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments AS c
						//			LEFT JOIN SchoolCommentScores AS s ON c.Id = s.CommentId 
						//		WHERE
						//			s.IsAttend = 1 
						//			AND c.State in (0,1,2,3)
						//			AND c.SchoolSectionId in @eids
						//UNION
						//		SELECT
						//			4 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments as c
						//		WHERE
						//			RumorRefuting = 1 
						//			AND State in (0,1,2,3) 
						//			AND c.SchoolSectionId in @eids
						//UNION
						//		SELECT
						//			5 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments AS s
						//			LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
						//		WHERE
						//			c.AggScore >= 80 
						//			AND s.State in (0,1,2,3) 
						//			AND s.SchoolSectionId in @eids
						//UNION
						//		SELECT
						//			6 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments AS s
						//			LEFT JOIN SchoolCommentScores AS c ON s.Id = c.CommentId
						//		WHERE
						//			c.AggScore <= 40 
						//			AND s.State in (0,1,2,3)
						//			AND s.SchoolSectionId in @eids
						//UNION
						//		SELECT
						//			7 AS TotalType,
						//			COUNT(1) AS Total,
						//			null AS SchoolSectionId 
						//		FROM
						//			SchoolComments as c
						//		WHERE
						//			IsHaveImagers = 1 
						//			AND State in (0,1,2,3) 
						//			AND c.SchoolSectionId in @eids";
                var str_SQL = $@"SELECT
								q.Id,
								q.State,
								scs.IsAttend,
								scs.AggScore,
								q.ReplyCount,
								q.RumorRefuting,
								q.IsHaveImagers
							FROM
								SchoolComments as q
								LEFT JOIN SchoolCommentScores as scs on scs.CommentId = q.Id
							WHERE
								q.State IN (0,1,2,3) AND SchoolSectionId IN @eids";
                var finds = await _schoolProductDB.SlaveConnection.QueryAsync<(Guid ID, int State, int IsAttend, float AggScore, int ReplyCount, int RumorRefuting, int IsHaveImagers)>(str_SQL, new { eids });
                //var finds = await _schoolProductDB.SlaveConnection.QueryAsync<SchoolCommentTotalDTO>(str_SQL, new { eids });
                if (finds?.Any() == true)
                {
                    var resultDatas = new List<SchoolCommentTotalDTO>();
                    for (int i = 1; i < 8; i++)
                    {
                        var item = new SchoolCommentTotalDTO()
                        {
                            TotalType = (Common.Enum.QueryConditionType)i
                        };
                        switch (i)
                        {
                            case 1:
                                item.Total = finds.Count();
                                break;
                            case 2:
                                item.Total = finds.Count(p => p.State == 3 && p.ReplyCount > 10);
                                break;
                            case 3:
                                item.Total = finds.Count(p => p.IsAttend == 1);
                                break;
                            case 4:
                                item.Total = finds.Count(p => p.RumorRefuting == 1);
                                break;
                            case 5:
                                item.Total = finds.Count(p => p.AggScore >= 80);
                                break;
                            case 6:
                                item.Total = finds.Count(p => p.AggScore <= 40);
                                break;
                            case 7:
                                item.Total = finds.Count(p => p.IsHaveImagers == 1);
                                break;
                        }
                        resultDatas.Add(item);
                    }
                    return resultDatas;
                }
            }
            return default;
        }
    }
}
