﻿using Sxb.School.Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolExtensionLevelQuery
    {
        /// <summary>
        /// 根据城市代码与学校类型删除
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<OnlineSchoolExtLevelInfo>> GetByCityCodeSchFType(int cityCode, string schFType);

        /// <summary>
        /// 根据 城市代码 与 学校类型 删除
        /// </summary>
        /// <param name="deleteParams">
        /// <para>
        /// Key -> CityCode
        /// </para>
        /// <para>
        /// Value-> SchFType
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<int> RemoveByParamsAsync(IEnumerable<KeyValuePair<int, string>> deleteParams);
        Task<bool> InsertAsync(OnlineSchoolExtLevelInfo entity);
    }
}
