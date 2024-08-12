using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Common.QueryDto
{
    public partial class CategoryQueryDto
    {
        public CategoryQueryDto()
        {

        }

        public CategoryQueryDto(CategoryQueryDto categoryQueryDto, bool copyChild = false)
        {
            Id = categoryQueryDto.Id;
            Name = categoryQueryDto.Name;
            ShortName = categoryQueryDto.ShortName;
            if (copyChild)
            {
                Children = categoryQueryDto.Children;
            }
        }

        public CategoryQueryDto(string name, string shortName)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ShortName = shortName ?? throw new ArgumentNullException(nameof(shortName));
        }

        public int Id { get; set; }

        /// <summary> 
        /// 节点名称 
        /// </summary> 
        public string Name { get; set; }

        /// <summary> 
        /// 自定义短链接,否则为节点名称全拼 
        /// </summary> 
        public string ShortName { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public IEnumerable<CategoryQueryDto> Children { get; set; }

        /// <summary>
        /// 归属平台
        /// </summary>
        public ArticlePlatform Platform { get; set; }

        public static IEnumerable<CategoryQueryDto> GetChildren(IEnumerable<Category> all, int id)
        {
            if (id >= 0)
            {
                List<CategoryQueryDto> list = new List<CategoryQueryDto>();
                foreach (var s in all.Where(s => s.ParentId == id))
                {
                    var children = GetChildren(all, s.Id);
                    if (s.IsLeaf || children.Any())
                    {
                        list.Add(new CategoryQueryDto()
                        {
                            Id = s.Id,
                            Name = s.Name,
                            ShortName = s.ShortName,
                            Platform = s.Platform,
                            Children = children
                        });
                    }
                }
                return list;
                //return from s in all.Where(s => s.ParentId == id)
                //       let children = GetChildren(all, s.Id)
                //       //非叶子节点, 必须要有子级
                //       where s.IsLeaf || children.Any()
                //       select new CategoryQueryDto()
                //       {
                //           Id = s.Id,
                //           Name = s.Name,
                //           ShortName = s.ShortName,
                //           Platform = s.Platform,
                //           Children = children
                //       }
                //       ;

                //return all.Where(s => s.ParentId == id)
                //    //非叶子节点, 必须要有子级
                //    .Where(s => !s.IsLeaf && GetChildren(all, s.Id).Any())
                //    .Select(s => new CategoryQueryDto()
                //    {
                //        Id = s.Id,
                //        Name = s.Name,
                //        ShortName = s.ShortName,
                //        Platform = s.Platform,
                //        Children = GetChildren(all, s.Id)
                //    });
            }
            return Enumerable.Empty<CategoryQueryDto>();
        }

        public static IEnumerable<Category> GetChildrenFlat(IEnumerable<Category> all, int id)
        {
            if (id >= 0)
            {
                List<Category> dto = new();

                //tolist, 延迟计算会导致死循环
                var children = all.Where(s => s.ParentId == id).ToList();
                while (children.Any())
                {
                    dto.AddRange(children);

                    var childrenIds = children.Select(s => s.Id).ToList();
                    children = all.Where(s => childrenIds.Contains(s.ParentId)).ToList();
                }
                return dto;
            }
            return Enumerable.Empty<Category>();
        }

        public static IEnumerable<Category> GetChildrenFlat(IEnumerable<Category> all, List<int> ids)
        {
            List<Category> dto = new();
            foreach (var id in ids)
            {
                if (id >= 0)
                {
                    var children = all.Where(s => s.ParentId == id).ToList();
                    while (children.Any())
                    {
                        dto.AddRange(children);

                        var childrenIds = children.Select(s => s.Id).ToList();
                        children = all.Where(s => childrenIds.Contains(s.ParentId)).ToList();
                    }
                }
            }
            return dto;
        }
    }


    public class Recursion<Source, Dest>
    {
        public Recursion(IEnumerable<Source> all, Func<Source, IEnumerable<Dest>, Dest> createFunc)
        {
            All = all ?? throw new ArgumentNullException(nameof(all));
            CreateFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
        }

        public IEnumerable<Source> All { get; set; }
        public Func<Source, IEnumerable<Dest>, Dest> CreateFunc { get; set; }


        private Type SourceType => typeof(Source);
        private PropertyInfo IdProp => SourceType.GetProperty("Id");
        private PropertyInfo ParentIdProp => SourceType.GetProperty("ParentId");

        public IEnumerable<Dest> GetChildren(object id)
        {
            if (id != null)
            {
                return All.Where(s => ParentIdProp.GetValue(s).Equals(id))
                    .Select(s => CreateFunc(s, GetChildren(IdProp.GetValue(s))));
            }
            return Enumerable.Empty<Dest>();
        }

    }
}
