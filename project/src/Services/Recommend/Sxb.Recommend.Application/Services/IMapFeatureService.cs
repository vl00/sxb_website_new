using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IMapFeatureService
    {
        /// <summary>
        /// 获取某个类型的映射特性
        /// </summary>
        /// <param name="type">1.学校 2.文章</param>
        /// <returns></returns>
        Task<MapFeature> GetAsync(string id);


        /// <summary>
        /// 获取某个类型的映射特性
        /// </summary>
        /// <param name="type">1.学校 2.文章</param>
        /// <returns></returns>
        Task<IEnumerable<MapFeature>> GetAsync(int type);

        Task InitialFeatures();

        Task<bool> Update(MapFeature mapFeature);


    }
}
