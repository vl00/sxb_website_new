using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public interface ISchoolExtensionQuery
    {
        /// <summary>
        /// 获取学部详情
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="schoolNo">学部No</param>
        /// <returns></returns>
        Task<SchoolExtensionDTO> GetSchoolExtensionDetails(Guid eid, long schoolNo);
        /// <summary>
        /// 获取学部视频
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="type">视频类型</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<DateTime, string, byte, string, string>>> GetExtensionVideos(Guid eid, SchoolVideoType type = SchoolVideoType.Unknow);
        /// <summary>
        /// 获取学校的地址
        /// <para>Key -> ExtID</para>
        /// <para>Value -> SchoolNo</para>
        /// <para>Message -> SchoolName - ExtName</para>
        /// <para>Data -> SchoolAddress</para>
        /// </summary>
        /// <param name="eids">学部IDs</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid, int, string, string>>> GetExtAddresses(IEnumerable<Guid> eids);
        /// <summary>
        /// 根据学校id获取分部姓名
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid>>> GetSchoolExtensionNames(Guid sid);
        /// <summary>
        /// 获取学部图片
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="schoolNo">学部No</param>
        /// <param name="types">图片类型s</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolImageDTO>> GetSchoolExtensionImages(Guid eid, long schoolNo, IEnumerable<SchoolImageType> types);
        /// <summary>
        /// 获取推荐学部
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid, Guid, string, string, int>>> GetRecommendExtension(byte type, byte grade, int city, Guid eid, int take = 2);
        Task<IEnumerable<string>> GetSchoolFieldYears(Guid eid, string field);
        /// <summary>
        /// 换取学部短ID
        /// </summary>
        /// <param name="eids">学部ID</param>
        /// <returns></returns>
        Task<IEnumerable<(Guid EID, long SchoolNo, string Base32String)>> GetSchoolNosAsync(IEnumerable<Guid> eids);
        Task<string> GetSchoolFieldContentAsync(Guid eid, string field, int year);
        /// <summary>
        /// 根据学部ID获取学部学段
        /// </summary>
        /// <param name="eids">学部IDs</param>
        /// <returns></returns>
        Task<Dictionary<Guid, SchoolGradeType>> GetExtensionGradesAsync(IEnumerable<Guid> eids);

        Task<IEnumerable<ExtensionCounterPartAllocationDTO>> GetCouterPartAndAllocationAsync(IEnumerable<Guid> eids);
        Task<IEnumerable<OnlineExtensionCounterPartAllocationDTO>> GetOnlineCouterPartAndAllocationAsync(IEnumerable<Guid> eids);

        /// <summary>
        /// 更新学部的对口学校与派位学校
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="year">年份</param>
        /// <param name="counterPartJSON">对口学校JSON
        /// <para>
        /// [{"key":"schoolName_extName", "value":"eid"}]
        /// </para>
        /// </param>
        /// <param name="allocationJSON">派位学校JSON
        /// <para>
        /// [{"key":"schoolName_extName", "value":"eid"}]
        /// </para>
        /// </param>
        /// <returns></returns>
        Task<bool> UpdateCouterPartAndAllocationAsync(Guid eid, int year, string counterPartJSON, string allocationJSON);
        Task<IEnumerable<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>> GetSchoolAndExtensionNameAsync(IEnumerable<Guid> eids);

        /// <summary>
        /// 更新学部别称
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="json">别称JSON
        /// <para>["a","b"]</para></param>
        /// <returns></returns>
        Task<int> UpdateExtNicknameAsync(Guid eid, string json);

        Task<IEnumerable<YearExtFieldInfo>> ListFieldYearsAsync(IEnumerable<Guid> eids, string field = default);

        /// <summary>
        /// 更新获取新增字段年份
        /// </summary>
        /// <param name="entity">当Latest为default时插入</param>
        /// <returns></returns>
        Task<int> InsertOrUpdateExtFieldYearAsync(YearExtFieldInfo entity);

        Task<IEnumerable<ExtSimpleDTO>> GetExtSimpleInfoAsync(IEnumerable<Guid> eids);

        /// <summary>
        /// 获取推荐学部
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="top">获取条数(默认8条)</param>
        /// <returns></returns>
        Task<IEnumerable<(Guid SID, Guid EID, string SchoolName, string ExtName, int ExtNo)>> ListRecommendSchoolAsync(Guid eid, int top = 8);
        /// <summary>
        /// 订阅学部
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<bool> SubscribeAsync(Guid eid, Guid userID);
        /// <summary>
        /// 获取周边房产信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingBuildingInfo>> PageExtSurroundBuildingsAsync(double longitude, double latitude, int distance = 1950, int pageIndex = 1, int pageSize = 5);
        /// <summary>
        /// 获取周边信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingPoiInfo>> ListExtSurroundInfosAsync(double longitude, double latitude, int distance = 1950);
        /// <summary>
        /// 获取周边学部ID
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="grade">学段s</param>
        /// <param name="distance">范围(米)</param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<IEnumerable<(double Distance, Guid EID)>> ListSurroundExtIDsAsync(double longitude, double latitude, IEnumerable<SchoolGradeType> grade, int distance = 2750, int take = 10);
        /// <summary>
        /// 获取房产信息
        /// </summary>
        /// <param name="ids">房产信息ID</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingsAsync(IEnumerable<Guid> ids);
        /// <summary>
        /// 获取周边信息统计
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <returns></returns>
        Task<IEnumerable<(int TypeCode, int Count)>> ListSurroundCountAsync(double longitude, double latitude, int distance = 1950);
        Task<IEnumerable<SchoolSurroundingPoiInfo>> PageExtSurroundInfosAsync(double longitude, double latitude, int distance = 1950, int pageIndex = 2, int pageSize = 5, int? typeCode = null);
        Task<int> GetSurroundBuildingCountAsync(double longitude, double latitude, int distance = 1950);
        Task<IEnumerable<ExtSimpleDTO>> ListExtInfoForSurroundAsync(IEnumerable<Guid> eids);
        Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid);
    }
}
