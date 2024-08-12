using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface IDegreeAnalyzeRepository
    {
        Task<IEnumerable<DgAyQuestionDto>> FindQuestions(bool useReadConn = true);

        Task<IEnumerable<DgAyQuestionOptionDto>> FindQuestionOptions(bool useReadConn = true);

        Task<IEnumerable<(long City, string CityName, long Area, string AreaName)>> FindAddrAreas(long city);
        // 录入时房产地址必须对应一个eid
        Task<IEnumerable<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>> FindAddresses(long? area = null);
        Task<PagedList<(long City, string CityName, long Area, string AreaName, string Address, int Sort)>> FindAddresses(long area, string kw, int pageIndex, int pageSize);
        Task<bool> IsExistsAddresse(long area, string address);

        Task<PagedList<(Guid Id, string Title, DateTime Time, int i)>> GetMyQaResultList(Guid userid, int pageIndex, int pageSize);

        /// <summary>
        /// 报告与结果
        /// </summary>
        Task<DgAyUserQaPaperDto> GetQaPaperAndResult(Guid id);
        /// <summary>
        /// 答题内容
        /// </summary>
        Task<DgAyUserQaContent[]> GetQaContents(Guid id);

        Task<DgAyUserQaPaperIsUnlockedDto> GetQaIsUnlocked(Guid id);

        Task<IEnumerable<DgAySchoolItemDto>> GetSchoolItems(IEnumerable<Guid> eids);

        Task<IEnumerable<(Guid Eid, double Score)>> GetSchoolJfScoreLine(IEnumerable<Guid> eids, int year);

        /// <summary>
        /// 查地区编码和地区名
        /// </summary>
        /// <param name="v">地区编码或地区名</param>
        /// <returns>(地区编码, 地区名)</returns>
        Task<(long Code, string Name)> GetCityAreaByCodeOrName(string v, long? parentCode = null);

        /// <summary>获取城市的地区s</summary>
        /// <returns>地区s</returns>
        Task<IEnumerable<(long Code, string Name)>> GetCityAreas(long code);

        /// <summary>保存内容</summary>
        Task SaveQpaper(DgAyUserQaPaper q, List<DgAyUserQaContent> qaContents);

        Task UpQpaperAndAnalyzedResult(DgAyUserQaPaper qa, DgAyUserQaResultContent resultContent);

        /// <summary>
        /// 根据房产地址查对口小学eid
        /// </summary>
        /// <param name="area"></param>
        /// <param name="address"></param>
        /// <returns>小学学部eid</returns>
        Task<Guid> GetDgAyPrimarySchoolByAreaAddress(long area, string address);

        /// <summary>
        /// 查政策文件（数据库中最新的年份）
        /// </summary>
        /// <param name="area"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task<DgAySchPcyFileDto> GetDgAySchPcyFile(long? area, DgAySchPcyFileTypeEnum type);

        /// <summary>
        /// （录取分数线—用户计算积分=0 ~ -30区间内的学校中）录取分数线最高的5所学
        /// </summary>
        Task<Guid[]> FindTop5JfPriSchoolEids(int year, long area, double totalPoint);
        /// <summary>
        /// 直径范围3公里内的公办小学 
        /// </summary>
        Task<Guid[]> Find3kmOvPriSchoolEids(double lng, double lat, long? area);
        /// <summary>
        /// 查小学的 对口直升 电脑派位
        /// </summary>
        /// <param name="eid"></param>
        /// <param name="year">null查最新数据</param>
        /// <returns></returns>
        Task<(Guid[] Counterpart, Guid[] Allocation)> FindCpPcAssignAndHeliSchoolEids(Guid eid, int? year);
        /// <summary>
        /// 找民办小学s
        /// </summary>
        Task<Guid[]> FindMbPriSchoolEids(IEnumerable<(string FindField, string FindFieldFw, string FindFieldFwJx)> conditions);
    }
}
