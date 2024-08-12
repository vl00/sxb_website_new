using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyOrderAggregate
{
    public enum DgAyProductType
    {

        /// <summary>
        /// 学位分析结果
        /// </summary>
        DgAyResult = 1,

        /// <summary>
        /// 查阅A级学校权限
        /// </summary>
        ViewALevelSchoolPermission = 2
    }
}
