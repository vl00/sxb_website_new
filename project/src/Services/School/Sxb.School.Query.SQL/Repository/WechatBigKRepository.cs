using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class WechatBigKRepository : IWechatBigKRepository
    {
        readonly SchoolDataDB _schoolDataDB;
        public WechatBigKRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<IEnumerable<WeChatRecruitArticleInfo>> GetArticlesAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitArticleInfo>().Where($"ID in {ids.ToSQLInString()}").ToIEnumerableAsync();
        }

        public async Task<IEnumerable<WeChatRecruitAttachmentInfo>> GetAttachmentsAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitAttachmentInfo>().Where($"ID in {ids.ToSQLInString()}").ToIEnumerableAsync();
        }

        public async Task<IEnumerable<WeChatRecruitItemInfo>> GetItemsAsync(IEnumerable<Guid> recruitIDs, WeChatRecruitItemType type)
        {
            if (recruitIDs == null || !recruitIDs.Any()) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitItemInfo>().Where($"RecruitID IN {recruitIDs.ToSQLInString()}");
            if (type != WeChatRecruitItemType.Unknow) query.Where(p => p.Type == type);
            return await query.ToIEnumerableAsync();
        }

        public async Task<IEnumerable<WeChatRecruitScheduleInfo>> GetSchedulesAsync(IEnumerable<Guid> ids)
        {
            if (ids == null || !ids.Any()) return null;
            return await _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitScheduleInfo>().Where($"ID in {ids.ToSQLInString()}").ToIEnumerableAsync();
        }


        public async Task<IEnumerable<WeChatRecruitScheduleInfo>> GetAvaliableSchedulesAsync(DateTime date)
        {
            //DateTime date = DateTime.Now;
            return await _schoolDataDB.SlaveConnection
                .QuerySet<WeChatRecruitScheduleInfo>()
                .Where(s => s.MinDate >= date.Date && s.MaxDate <= date)
                .ToIEnumerableAsync();
        }

        public async Task<int> RemoveByParamsAsync(IEnumerable<string> deleteParams)
        {
            var effectCount = 0;
            var ids = new List<Guid>();
            if (deleteParams?.Any() == true)
            {
                var index = 0;
                IEnumerable<string> tmp_Params;
                while ((tmp_Params = deleteParams.Skip(index).Take(200))?.Any() == true)
                {
                    var finds = await _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitInfo>()
                        .Where($"ISNULL(CAST(Type as VARCHAR),'') + ISNULL(CAST(Grade as VARCHAR),'') + ISNULL(CAST(Year as VARCHAR),'') + ISNULL(CAST(AreaCode as VARCHAR),'') in {tmp_Params.ToSQLInString()}")
                        .Select(p => p.ID)
                        .ToIEnumerableAsync();
                    if (finds?.Any() == true) ids.AddRange(finds.Select(p => p.ID));
                    index += 200;
                }
            }

            if (ids.Any())
            {
                var relations = new List<WeChatRecruitItemInfo>();
                var index = 0;
                IEnumerable<Guid> tmp_IDs;
                while ((tmp_IDs = ids.Skip(index).Take(200))?.Any() == true)
                {
                    var finds = await _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitItemInfo>()
                        .Where($"RecruitID IN {tmp_IDs.ToSQLInString()}")
                        .ToIEnumerableAsync();
                    if (finds?.Any() == true) relations.AddRange(finds);
                    index += 200;
                }
                if (relations.Any())
                {
                    foreach (var relationType in relations.Select(p => p.Type).Distinct())
                    {
                        switch (relationType)
                        {
                            case WeChatRecruitItemType.Article:
                                await _schoolDataDB.Connection.CommandSet<WeChatRecruitArticleInfo>().Where($"ID in {relations.Where(p => p.Type == relationType).Select(p => p.ItemID).ToSQLInString()}").DeleteAsync();
                                break;
                            case WeChatRecruitItemType.Attachment:
                                await _schoolDataDB.Connection.CommandSet<WeChatRecruitAttachmentInfo>().Where($"ID in {relations.Where(p => p.Type == relationType).Select(p => p.ItemID).ToSQLInString()}").DeleteAsync();
                                break;
                            case WeChatRecruitItemType.Schedule:
                                await _schoolDataDB.Connection.CommandSet<WeChatRecruitScheduleInfo>().Where($"ID in {relations.Where(p => p.Type == relationType).Select(p => p.ItemID).ToSQLInString()}").DeleteAsync();
                                break;
                        }
                    }
                    await _schoolDataDB.Connection.CommandSet<WeChatRecruitItemInfo>().Where($"ID in {relations.Select(p => p.ID).ToSQLInString()}").DeleteAsync();
                }
                effectCount = await _schoolDataDB.Connection.CommandSet<WeChatRecruitInfo>().Where($"ID in {ids.ToSQLInString()}").DeleteAsync();
            }
            return effectCount;
        }

        public async Task<int> InsertAsync(WeChatRecruitInfo entity)
        {
            if (entity.Year == default || entity.Type == WeChatRecruitType.Unknow || entity.AreaCode == default || entity.Grade == SchoolGradeType.Defalut || string.IsNullOrWhiteSpace(entity.Title)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<WeChatRecruitInfo>().InsertAsync(entity);
        }

        public async Task<int> InsertAsync(WeChatRecruitArticleInfo entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Title) || string.IsNullOrWhiteSpace(entity.Content)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            if (entity.CreateTime == default) entity.CreateTime = DateTime.Now;
            return await _schoolDataDB.Connection.CommandSet<WeChatRecruitArticleInfo>().InsertAsync(entity);
        }

        public async Task<int> InsertAsync(WeChatRecruitScheduleInfo entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Title) || string.IsNullOrWhiteSpace(entity.DataJson)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<WeChatRecruitScheduleInfo>().InsertAsync(entity);
        }

        public async Task<int> InsertAsync(WeChatRecruitItemInfo entity)
        {
            if (entity.Type == WeChatRecruitItemType.Unknow || entity.RecruitID == default || entity.ItemID == default) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<WeChatRecruitItemInfo>().InsertAsync(entity);
        }

        public async Task<int> InsertAsync(WeChatRecruitAttachmentInfo entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Title)) return 0;
            if (entity.ID == default) entity.ID = Guid.NewGuid();
            return await _schoolDataDB.Connection.CommandSet<WeChatRecruitAttachmentInfo>().InsertAsync(entity);
        }

        public async Task<IEnumerable<WeChatRecruitInfo>> GetAsync(WeChatRecruitType type, int areaCode, SchoolGradeType? grade, int? year)
        {
            if (type == WeChatRecruitType.Unknow || areaCode < 100000) return default;
            var query = _schoolDataDB.SlaveConnection.QuerySet<WeChatRecruitInfo>().Where(p => p.Type == type && p.AreaCode == areaCode);
            if (grade != SchoolGradeType.Defalut) query.Where(p => p.Grade == grade);
            if (year.HasValue && year.Value > 1900)
            {
                query.Where(p => p.Year == year);
            }
            else
            {
                year = query.Max(p => p.Year);
                if (year.HasValue && year.Value > 1900) query.Where(p => p.Year == year);
            }
            var finds = await query.ToIEnumerableAsync();
            if (finds?.Any() == true)
            {
                return finds;
            }
            else { return default; }
        }

        public async Task<IEnumerable<int>> GetYearsAsync(int areaCode, WeChatRecruitType type, SchoolGradeType grade)
        {
            if (areaCode < 100000 || type == WeChatRecruitType.Unknow || grade == SchoolGradeType.Defalut) return default;
            var str_SQL = $"Select Distinct(Year) From {nameof(WeChatRecruitInfo)} Where AreaCode = @areaCode AND Grade = @grade AND Type = @type";
            return await _schoolDataDB.SlaveConnection.QueryAsync<int>(str_SQL, new { areaCode, type, grade });
        }
    }
}
