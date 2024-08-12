using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.IRepository
{
    public interface ISchoolRepository
    {
        /// <summary>
        /// 获取学部详情
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="schoolNo">学部No</param>
        /// <returns></returns>
        Task<SchoolExtensionDTO> GetExtensionDTO(Guid eid, long schoolNo);
        /// <summary>
        /// 获取学部图片
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="schoolNo">学部No</param>
        /// <param name="types">图片类型(复数)</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolImageDTO>> GetSchoolExtensionImages(Guid eid, long schoolNo, IEnumerable<SchoolImageType> types);
        /// <summary>
        /// 获取学部视频
        /// </summary>
        /// <param name="extId">学部ID</param>
        /// <param name="type">视频类型</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, SchoolVideoType type = 0);
        /// <summary>
        /// 批量获取学校分部的地址
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid, int, string, string>>> GetSchoolExtAddress(IEnumerable<Guid> eids);
        /// <summary>
        /// 根据学校ID获取学部
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid>>> GetSchoolExtensions(Guid sid);
        /// <summary>
        /// 获取学校的推荐学校
        /// </summary>
        /// <param name="type"></param>
        /// <param name="grade"></param>
        /// <param name="city"></param>
        /// <param name="eid"></param>
        /// <param name="take">获取条数</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValueDTO<Guid, Guid, string, string, int>>> GetRecommendExtension(byte type, byte grade, int city, Guid eid, int take);
        Task<IEnumerable<string>> GetSchoolFieldYears(Guid eid, string field);
        Task<Dictionary<Guid, long>> GetSchoolNosAsync(IEnumerable<Guid> eids);
        Task<IEnumerable<OnlineSchoolYearFieldContentInfo>> GetSchoolYearFieldsAsync(Guid eid, string field, int year, bool ignoreIsValid = false);
        Task<Dictionary<Guid, SchoolGradeType>> GetExtensionGradesAsync(IEnumerable<Guid> eids);

        Task<IEnumerable<ExtensionCounterPartAllocationDTO>> GetExtensionCounterPartAllocationAsync(IEnumerable<Guid> eids);
        Task<IEnumerable<OnlineExtensionCounterPartAllocationDTO>> GetOnlineExtensionCounterPartAllocationAsync(IEnumerable<Guid> eids);
        /// <summary>
        /// 更新学部的对口学校与派位学校
        /// </summary>
        /// <param name="eid">学部ID</param>
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
        /// <summary>
        /// 根据EID获取学校与学部名称
        /// </summary>
        /// <param name="eids">学部IDs</param>
        /// <returns></returns>
        Task<IEnumerable<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>> GetSchoolAndExtensionNameAsync(IEnumerable<Guid> eids);

        /// <summary>
        /// 更新学部别称
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="json">别称JSON
        /// <para>["a","b"]</para></param>
        /// <returns></returns>
        Task<int> UpdateExtNicknameAsync(Guid eid, string json);

        Task<int> InsertOrUpdateExtFieldYearAsync(YearExtFieldInfo entity);
        Task<int> InsertOrUpdateExtFieldContentAsync(OnlineSchoolYearFieldContentInfo entity);
        Task<IEnumerable<YearExtFieldInfo>> ListFieldYearsAsync(IEnumerable<Guid> eids, string field = default);

        Task<IEnumerable<ExtSimpleDTO>> GetExtSimpleInfoAsync(IEnumerable<Guid> eids);

        /// <summary>
        /// 获取推荐学部
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="top">获取条数(默认8条)</param>
        /// <returns></returns>
        Task<IEnumerable<(Guid, Guid, string, string, int)>> ListRecommendSchoolAsync(Guid eid, int top = 8);
        /// <summary>
        /// 新增用户订阅学部(如果没订阅过)
        /// </summary>
        /// <param name="eid">学部ID</param>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        Task<bool> InsertExtensionSubscribeIfNotExistAsync(Guid eid, Guid userID);


        Task<IEnumerable<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<Guid> eids = default, IEnumerable<long> nos = default);
        /// <summary>
        /// 获取有效学部
        /// </summary>
        /// <param name="sid">学校ID</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid);
        /// <summary>
        /// 获取周边信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingPoiInfo>> ListSurroundInfoAsync(double longitude, double latitude, int distance = 2000, int take = 5);
        /// <summary>
        /// 获取周边房产信息
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingAsync(double longitude, double latitude, int distance = 2000, int pageIndex = 1, int pageSize = 10);
        /// <summary>
        /// 获取周边学部ID
        /// </summary>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="distance">范围(米)</param>
        /// <param name="grades">学段s</param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<IEnumerable<(double Distance, Guid EID)>> ListSurroundExtIDsAsync(double longitude, double latitude, IEnumerable<SchoolGradeType> grades, int distance = 3000, int take = 10);
        /// <summary>
        /// 获取房产信息
        /// </summary>
        /// <param name="ids">房产信息ID</param>
        /// <returns></returns>
        Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingsAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<(int TypeCode, int Count)>> ListSurroundCountAsync(double longitude, double latitude, int distance = 2000);
        Task<IEnumerable<SchoolSurroundingPoiInfo>> PageSurroundInfoAsync(double longitude, double latitude, int distance = 2000, int pageIndex = 1, int pageSize = 10, int? typeCode = null);
        Task<int> GetSurroundBuildingCountAsync(double longitude, double latitude, int distance = 2000);
        Task<IEnumerable<ExtSimpleDTO>> ListExtSimpleInfosAsync(IEnumerable<Guid> eids);
    }
}
