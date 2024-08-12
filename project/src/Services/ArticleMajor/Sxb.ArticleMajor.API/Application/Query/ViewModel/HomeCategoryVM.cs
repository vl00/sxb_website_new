using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using System.Collections.Generic;
using System.Linq;

namespace Sxb.ArticleMajor.API.Application.Query.ViewModel
{
    public class HomeCategoryVM
    {
        public HomeCategoryVM()
        {
        }

        public HomeCategoryVM(string name, string url)
        {
            Name = name;
            Url = url;
        }

        /// <summary>
        /// 名称 e.g.中考备考
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Url { get; set; }

        public IEnumerable<HomeCategoryVM> Children { get; set; }

    }

    public class CategorySidebarVM
    {
        public static CategoryQueryDto GetSchoolSidebar(ArticlePlatform platform)
        {
            List<CategoryQueryDto> children =null;
            switch (platform)
            {
                case ArticlePlatform.Master:
                    break;
                case ArticlePlatform.YouEr:
                    children = new List<CategoryQueryDto>()
                    {
                        new CategoryQueryDto("公办幼儿园", "https://www.sxkid.com/school/sh=lx110"),
                        new CategoryQueryDto("普通民办幼儿园", "https://www.sxkid.com/school/sh=lx120"),
                        new CategoryQueryDto("民办普惠幼儿园", "https://www.sxkid.com/school/sh=lx121"),
                        new CategoryQueryDto("国际幼儿园", "https://www.sxkid.com/school/sh=lx130"),
                    };
                    break;
                case ArticlePlatform.XiaoXue:
                    children = new List<CategoryQueryDto>()
                    {
                        new CategoryQueryDto("公办小学", "https://www.sxkid.com/school/sh=lxlx210"),
                        new CategoryQueryDto("普通民办小学", "https://www.sxkid.com/school/sh=lx220"),
                        new CategoryQueryDto("双语小学", "https://www.sxkid.com/school/sh=lx231"),
                        new CategoryQueryDto("外国人小学", "https://www.sxkid.com/school/sh=lx230"),
                    };
                    break;
                case ArticlePlatform.ZhongXue:
                    children = new List<CategoryQueryDto>()
                    {
                        new CategoryQueryDto("公办初中", "https://www.sxkid.com/school/sh=lx310"),
                        new CategoryQueryDto("普通民办初中", "https://www.sxkid.com/school/sh=lx320"),
                        new CategoryQueryDto("双语初中", "https://www.sxkid.com/school/sh=lx331"),
                        new CategoryQueryDto("外国人初中", "https://www.sxkid.com/school/sh=lx330"),
                    };
                    break;
                case ArticlePlatform.ZhongZhi:
                    break;
                case ArticlePlatform.GaoZhong:
                    children = new List<CategoryQueryDto>()
                    {
                        new CategoryQueryDto("公办高中", "https://www.sxkid.com/school/sh=lx410"),
                        new CategoryQueryDto("普通民办高中", "https://www.sxkid.com/school/sh=lx420"),
                        new CategoryQueryDto("国际高中", "https://www.sxkid.com/school/sh=lx432"),
                        new CategoryQueryDto("外国人高中", "https://www.sxkid.com/school/sh=lx430"),
                    };
                    break;
                case ArticlePlatform.SuZhi:
                    break;
                case ArticlePlatform.GuoJi:
                    break;
                default:
                    break;
            }
            return new CategoryQueryDto("院校库", "https://www.sxkid.com/school/") { Children = children };
        }
    }
}
