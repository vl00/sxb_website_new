using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using ProductManagement.Framework.MongoDb;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Query.Mongodb
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(IMongoService mongo) : base(mongo)
        {
        }

        public async Task AddAsync(IEnumerable<Category> entities)
        {
            await _collection.InsertManyAsync(entities);
        }

        public async Task UpdateShortNameAsync(IEnumerable<Category> entities)
        {
            //foreach (var item in entities)
            //{
            //    await _collection.UpdateOneAsync(
            //        s =>s.Id == item.Id, 
            //        Builders<Category>.Update.Set(u => u.ShortName, item.ShortName)
            //    );
            //}

            var models = new List<WriteModel<Category>>(entities.Count());
            foreach (var item in entities)
            {
                FilterDefinition<Category> p1 = (Expression<Func<Category, bool>>)(s => s.Id == item.Id);
                UpdateDefinition<Category> p2 = Builders<Category>.Update.Set(u => u.ShortName, item.ShortName);
                models.Add(new UpdateOneModel<Category>(p1,p2));
            }
            await _collection.BulkWriteAsync(models);
        }

        public async Task<Category> GetAsync(ArticlePlatform platform, string shortName = null)
        {
            if (string.IsNullOrWhiteSpace(shortName))
            {
                return (await _collection.FindAsync(s => s.IsValid && s.Platform == platform)).FirstOrDefault();
            }
            return (await _collection.FindAsync(s => s.IsValid && s.Platform == platform && s.ShortName == shortName)).FirstOrDefault();
        }

        public async Task<Category> GetByNameAsync(ArticlePlatform platform, string name)
        {
            return (await _collection.FindAsync(s => s.IsValid && s.Platform == platform && s.Name == name)).FirstOrDefault();
        }

        public async Task<IEnumerable<Category>> GetListAsync()
        {
            return (await _collection.FindAsync(s => s.IsValid)).ToList();//.ToEnumerable();
        }

        public async Task<IEnumerable<Category>> GetListAsync(ArticlePlatform platform)
        {
            //An IAsyncCursor can only be enumerated once. so ToList
            return (await _collection.FindAsync(s => s.IsValid && s.Platform == platform)).ToList();//.ToEnumerable();
        }

        public async Task<IEnumerable<Category>> GetListAsync(ArticlePlatform platform, IEnumerable<string> shortNames)
        {
            return (await _collection.FindAsync(s => s.IsValid && s.Platform == platform && shortNames.Contains(s.ShortName))).ToList();
        }

        public async Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync()
        {
            var all = (await _collection.FindAsync(s => s.IsValid)).ToList();//.ToEnumerable();
            return CategoryQueryDto.GetChildren(all, 0);
        }

        public async Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync(ArticlePlatform platform)
        {
            return await GetChildrenAsync(platform, 0);
        }

        public async Task<IEnumerable<CategoryQueryDto>> GetChildrenAsync(ArticlePlatform platform, int parentId, List<int> includeLeafs = null)
        {
            //An IAsyncCursor can only be enumerated once. so ToList
            var all = (await GetListAsync(platform)).ToList();
            if (includeLeafs?.Any() == true)
            {
                all = all.Where(s => !s.IsLeaf || includeLeafs.Contains(s.Id)).ToList();
            }

            //return new Recursion<Category, CategoryQueryDto>(all.ToList(),
            //    (s, children) => new CategoryQueryDto()
            //    {
            //        Id = s.Id,
            //        Name = s.Name,
            //        ShortName = s.ShortName,
            //        Children = children
            //    }).GetChildren(parentId);
            return CategoryQueryDto.GetChildren(all, parentId);
        }

        public async Task<IEnumerable<Category>> GetChildrenFlatAsync(ArticlePlatform platform, int parentId)
        {
            var all = await GetListAsync(platform);
            return CategoryQueryDto.GetChildrenFlat(all, parentId);
        }

        public async Task<IEnumerable<CategoryQueryDto>> GetParentsAsync(ArticlePlatform platform, int childId, bool withself = false)
        {
            var all = (await GetListAsync(platform)).ToList();
            List<Category> parents = new List<Category>();

            Category current = all.FirstOrDefault(s => s.Id == childId);
            if (current != null)
            {
                if (withself) parents.Add(current);
                while (true)
                {
                    current = all.FirstOrDefault(s => s.Id == current.ParentId);
                    if (current == null) break;
                    parents.Add(current);
                }
            }
            return parents.Select(s => new CategoryQueryDto()
            {
                Id = s.Id,
                Name = s.Name,
                ShortName = s.ShortName,
            }).Reverse();
        }


        public async Task<IEnumerable<Category>> GetChildrenFlatWithselfAsync(ArticlePlatform platform, string shortName)
        {
            var category = await GetAsync(platform, shortName);
            if (category == null)
            {
                return new List<Category>();
            }
            var categories = (await GetChildrenFlatAsync(platform, category.Id)).ToList();
            categories.Add(category);
            return categories;
        }

        public async Task<IEnumerable<Category>> GetChildrenFlatWithselfByNameAsync(ArticlePlatform platform, string name)
        {
            var category = await GetByNameAsync(platform, name);
            if (category == null)
            {
                return new List<Category>();
            }
            var categories = (await GetChildrenFlatAsync(platform, category.Id)).ToList();
            categories.Add(category);
            return categories;
        }

        public async Task<IEnumerable<Category>> GetChildrenFlatAsync(ArticlePlatform platform, List<int> parentIds, bool withself = false)
        {
            var all = await GetListAsync(platform);
            var categories = CategoryQueryDto.GetChildrenFlat(all, parentIds);
            return !withself ? categories.ToList()
                    : all.Where(s => parentIds.Contains(s.Id)).Concat(categories).ToList();
        }


    }
}
