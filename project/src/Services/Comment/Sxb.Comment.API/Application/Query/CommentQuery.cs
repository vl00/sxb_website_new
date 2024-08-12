using Sxb.Comment.Common.DTO;
using Sxb.Comment.Common.Entity;
using Sxb.Comment.Common.OtherAPIClient.User;
using Sxb.Comment.Query.SQL.IRepository;
using Sxb.Framework.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Comment.API.Application.Query
{
    public class CommentQuery : ICommentQuery
    {
        ICommentRepository _commentRepository;
        ISchoolGiveLikeRepository _schoolGiveLikeRepository;
        ISchoolImageRepository _schoolImageRepository;
        IUserAPIClient _userAPIClient;
        public CommentQuery(ICommentRepository commentRepository, ISchoolGiveLikeRepository schoolGiveLikeRepository, ISchoolImageRepository schoolImageRepository, IUserAPIClient userAPIClient)
        {
            _commentRepository = commentRepository;
            _schoolGiveLikeRepository = schoolGiveLikeRepository;
            _schoolImageRepository = schoolImageRepository;
            _userAPIClient = userAPIClient;
        }

        public async Task<IEnumerable<SchoolCommentDTO>> GetCommentsByEID(Guid eid, Guid? userID, int take = 1)
        {
            if (eid == default) return null;
            var comments = await _commentRepository.GetSchoolSelectedComment(eid, 1, take);
            if (comments == default || !comments.Any()) return null;
            var userInfosTask = _userAPIClient.ListTalentDetails(comments.Where(p => p.CommentUserID != default).Select(p => p.CommentUserID).Distinct());
            var commentIDs = comments.Select(p => p.ID).Distinct();
            var commentScores = await _commentRepository.GetCommentScoresByCommentIDs(commentIDs);
            var images = await _schoolImageRepository.GetImageByDataSourceIDs(commentIDs, Common.Enum.ImageType.Comment);
            var tags = await _commentRepository.GetCommentTagsByCommentIDs(commentIDs);
            IEnumerable<GiveLikeInfo> likes = null;
            if (userID.HasValue && userID.Value != default)
            {
                likes = await _schoolGiveLikeRepository.CheckCurrentUserIsLikeBulk(userID.Value, commentIDs);
            }
            var userInfos = await userInfosTask;
            var result = new List<SchoolCommentDTO>();
            foreach (var comment in comments)
            {
                var dto = CommonHelper.MapperProperty<SchoolCommentInfo, SchoolCommentDTO>(comment);
                dto.UserID = comment.CommentUserID;
                dto.CreateTime = comment.AddTime;
                if (commentScores?.Any() == true)
                {
                    if (commentScores.Any(p => p.CommentID == comment.ID))
                    {
                        dto.StartTotal = GetCurrentSchoolstart(commentScores.First(p => p.CommentID == comment.ID).AggScore);
                    }
                    dto.Score = commentScores.FirstOrDefault(p => p.CommentID == comment.ID);
                }
                if (likes?.Any() == true) dto.IsLike = likes.Any(p => p.SourceID == comment.ID);
                if (images?.Any() == true) dto.Images = images.Where(p => p.DataSourcetID == comment.ID).Select(p => p.ImageUrl);
                if (tags?.Any() == true) dto.Tags = tags.Where(p => p.CommentID == comment.ID).Select(p => p.Content);
                if (comment.IsAnony)
                {
                    dto.NickName = "匿名用户";
                    dto.HeadImgUrl = "https://cdn.sxkid.com//images/AppComment/defaultUserHeadImage.png";
                }
                else
                {
                    var userInfo = userInfos?.FirstOrDefault(p => p.ID == comment.CommentUserID);
                    if (userInfo?.ID != default)
                    {
                        dto.NickName = userInfo.Nickname;
                        dto.HeadImgUrl = userInfo.HeadImgUrl;
                        dto.IsTalent = userInfo.IsAuth;
                        dto.TalentType = userInfo.Role;
                    }
                }
                result.Add(dto);
            }
            return result;
        }

        public async Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalBySID(Guid sid)
        {
            if (sid == default) return null;
            return await _commentRepository.GetCommentTotalBySID(sid);
        }

        public int GetCurrentSchoolstart(decimal scoreTemp)
        {
            if (scoreTemp > 20) scoreTemp /= 20;
            int start = 0, score = (int)Math.Round(scoreTemp);
            if (score <= 5) score *= 20;
            if (score >= 1 && score <= 20) start = 1;
            else if (score >= 21 && score <= 40) start = 2;
            else if (score >= 41 && score <= 60) start = 3;
            else if (score >= 61 && score <= 80) start = 4;
            else if (score >= 81 && score <= 100) start = 5;
            return start;
        }

        public async Task<IEnumerable<SchoolScoreCommentCountDto>> GetSchoolScoreCommentCountByEids(IEnumerable<Guid> eids)
        {
            if ((eids?.Count() ?? 0) < 1) return null;
            return await _commentRepository.GetSchoolScoreCommentCountByEids(eids);
        }

        public async Task<IEnumerable<SchoolCommentTotalDTO>> GetCommentTotalByEIDsAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true) return await _commentRepository.GetCommentTotalByEIDsAsync(eids);
            return default;
        }
    }
}
