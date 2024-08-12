using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public class ArticleRepository : RepositoryBase<Article>, IArticleRepository
    {
        ProductManagement.Framework.MongoDb.UoW.IMongoDbUnitOfWork _unitOfWork;
        public ArticleRepository(IMongoService mongo, ProductManagement.Framework.MongoDb.UoW.IMongoDbUnitOfWork unitOfWork) : base(mongo)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteAsync(string[] ids)
        {
            await _collection.DeleteManyAsync(s => ids.Contains(s.Id));
        }

        public async Task<Article> GetAsync(string code)
        {
            return (await _collection
                .FindAsync(s => s.Code == code && s.IsValid)).FirstOrDefault();
        }

        public async Task<List<ArticleFromUrl>> GetCodesByFromUrlAsync(string[] fromUrl)
        {
            return await _collection
                .AsQueryable()
                .Where(s => fromUrl.Contains(s.FromUrl) && s.IsValid)
                .Select(s => new ArticleFromUrl(s.FromUrl, s.Code))
                .ToListAsync();
        }

        public async Task<IEnumerable<Article>> GetLastestArticlesAsync(ArticlePlatform platform, int? cityId, IEnumerable<int> categoryIds, int top, bool mustCover = false)
        {
            var query = _collection
                .AsQueryable()
                .Where(s => s.IsValid)
                .Where(s => s.CategoryId > 0)
                ;
            if (platform != ArticlePlatform.Master)
            {
                query = query.Where(s => s.Platform == platform);
            }
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(s => categoryIds.Contains(s.CategoryId));
            }
            if (cityId != null)
            {
                //0全国文章显示在每个城市中
                query = query.Where(s => s.CityId == cityId.Value || s.CityId == 0);
            }
            if (mustCover)
            {
                query = query.Where(s => s.Covers != null && s.Covers.Any());
            }

            return await query
                .OrderByDescending(s => s.CityId)//优先显示有城市的, 后显示全国
                .ThenByDescending(s => s.PublishTime)
                .Take(top)
                .ToListAsync();
        }


        public async Task<(IEnumerable<Article> data, long total)> GetArticlesAsync(ArticlePlatform platform, int? cityId, IEnumerable<int> categoryIds, int pageIndex, int pageSize)
        {
            var query = _collection
                .AsQueryable()
                .Where(s => s.Platform == platform && s.IsValid && categoryIds.Contains(s.CategoryId));

            if (cityId != null)
            {
                //0全国文章显示在每个城市中
                query = query.Where(s => s.CityId == cityId.Value || s.CityId == 0);
            }

            var dataQuery = query
                .OrderByDescending(s => s.CityId)//优先显示有城市的, 后显示全国
                .ThenByDescending(s => s.PublishTime)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                ;

            var data = await dataQuery.ToListAsync();

            return (data, await query.CountAsync());
        }

        public async Task AddAsync(IEnumerable<Article> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task AddAsync(Article entitys)
        {
            if (_unitOfWork.SessionHandle != null)
            {
                //必须开启事务, SessionHandle=null 会报错
                await _collection.InsertOneAsync(_unitOfWork.SessionHandle, entitys);
            }
            else
            {
                await _collection.InsertOneAsync(entitys);
            }
        }


        public async Task<List<CityCategoriesHaveData>> GetHaveDataCategoryIds(ArticlePlatform platform)
        {
            var query = _collection
                .AsQueryable()
                .Where(s => s.IsValid)
                .Where(s => s.Platform == platform)
                .GroupBy(s => new { s.CityId, s.CategoryId })
                .GroupBy(s => s.Key.CityId)
                .Select(s => new CityCategoriesHaveData
                {
                    CityId = s.Key,
                    Categories = s.Select(x => x.Key.CategoryId).ToList()
                })
            ;
            return await query.ToListAsync();
        }

        public async Task<List<string>> ExistsIdsAsync(string[] ids)
        {
            var query = _collection
                .AsQueryable()
                .Where(s => ids.Contains(s.Id))
                .Select(s => s.Id)
            ;
            return await query.ToListAsync();
        }


        public List<ArticleDuplication> GetArticleDuplications(int top = 0)
        {
            List<string> pipelineJson = new List<string>()
            {
                @"{
                    $group: {
                        _id: '$FromUrl',
                        count: {
                            $sum: 1
                        },
                        dups: {
                            $addToSet: '$_id'
                        }
                    }
                 }",
                @"{
                    $match: {
                        count: {
                            $gt: 1
                        }
                    }
                }"
            };

            if (top > 0) pipelineJson.Add($"{{ $limit: {top} }}");

            var data = GetAggregate(pipelineJson, new AggregateOptions() { AllowDiskUse = true });
            return data.Select(s =>
            {
                return new ArticleDuplication()
                {
                    FromUrl = s["_id"].AsString,
                    Count = s["count"].AsInt32,
                    Ids = s["dups"].AsBsonArray.Select(s => s.AsBsonValue.ToString()).ToArray(),
                };
            }).ToList();
        }
    }
}
