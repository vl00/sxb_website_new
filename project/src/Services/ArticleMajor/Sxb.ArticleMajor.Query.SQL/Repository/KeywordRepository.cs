using Kogel.Dapper.Extension;
using Kogel.Dapper.Extension.MsSql;
using Sxb.ArticleMajor.Common.Entity;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.SQL.Repository
{
    public class KeywordRepository : IKeywordRepository
    {
        readonly ArticleMajorDB _db;

        public KeywordRepository(ArticleMajorDB db)
        {
            _db = db;
        }

        public async Task<IEnumerable<KeywordInfo>> ListAsync(KeywordSiteType? siteType, int? cityCode, KeywordPageType? pageType, KeywordPositionType? positionType)
        {
            var query = _db.SlaveConnection.QuerySet<KeywordInfo>();
            if (siteType.HasValue) query.Where(p => p.SiteType == siteType);
            else query.Where(p => p.SiteType.IsNull());
            if (cityCode.HasValue) query.Where(p => p.CityCode == cityCode);
            if (pageType.HasValue) query.Where(p => p.PageType == pageType);
            if (positionType.HasValue) query.Where(p => p.PositionType == positionType);
            var finds = await query.ToIEnumerableAsync();
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<PageList<KeywordInfo>> PageAsync(int pageIndex = 1, int pageSize = 10)
        {
            return await _db.SlaveConnection.QuerySet<KeywordInfo>().OrderByDescing(p => p.CreateTime).PageListAsync(pageIndex, pageSize);
        }

        public async Task<KeywordInfo> GetAsync(Guid id)
        {
            if (id == default) return default;
            var find = await _db.SlaveConnection.QuerySet<KeywordInfo>().Where(p => p.ID == id).GetAsync();
            if (find?.ID != default) return find;
            return default;
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            if (id != default) return await _db.Connection.CommandSet<KeywordInfo>().Where(p => p.ID == id).DeleteAsync();
            return default;
        }

        public async Task<int> InsertAsync(KeywordInfo entity)
        {
            if (entity == default) return default;
            entity.ID = Guid.NewGuid();
            entity.CreateTime = entity.ModifyTime = DateTime.Now;
            return await _db.Connection.CommandSet<KeywordInfo>().InsertAsync(entity);
        }

        public async Task<int> UpdateAsync(KeywordInfo entity)
        {
            if (entity == default || entity.ID == default) return default;
            entity.ModifyTime = DateTime.Now;
            return await _db.Connection.CommandSet<KeywordInfo>().UpdateAsync(entity);
        }
    }
}
