using Sxb.Framework.Foundation;
using Sxb.School.Common;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolExtensionQuery : ISchoolExtensionQuery
    {
        ISchoolRepository _schoolRepository;

        public SchoolExtensionQuery(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }

        public async Task<IEnumerable<KeyValueDTO<Guid, int, string, string>>> GetExtAddresses(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                return await _schoolRepository.GetSchoolExtAddress(eids);
            }
            return null;
        }

        public async Task<IEnumerable<KeyValueDTO<DateTime, string, byte, string, string>>> GetExtensionVideos(Guid eid, SchoolVideoType type = SchoolVideoType.Unknow)
        {
            return await _schoolRepository.GetSchoolVideos(eid, type);
        }

        public async Task<SchoolExtensionDTO> GetSchoolExtensionDetails(Guid eid, long schoolNo)
        {
            var extension = await _schoolRepository.GetExtensionDTO(eid, schoolNo);

            if (extension == null) return null;

            extension.SchoolImages = await _schoolRepository.GetSchoolExtensionImages(extension.ExtId, schoolNo, Enum.GetValues(typeof(SchoolImageType)).OfType<SchoolImageType>().ToArray());

            var schtype = new SchFType0(extension.Grade, extension.Type, extension.Discount, extension.Diglossia, extension.Chinese);
            extension.SchFType = schtype;
            extension.SchoolImageUrl = extension.HardwareImages.FirstOrDefault()?.Url ?? ConstantValue.DefaultSchoolHeadImg;
            CommonHelper.PropertyStringDef(extension, "暂未收录", "[]");
            return extension;
        }

        public async Task<IEnumerable<SchoolImageDTO>> GetSchoolExtensionImages(Guid eid, long schoolNo, IEnumerable<SchoolImageType> types)
        {
            if (eid == default && schoolNo < 0) return null;
            return await _schoolRepository.GetSchoolExtensionImages(eid, schoolNo, types);
        }

        public async Task<IEnumerable<KeyValueDTO<Guid>>> GetSchoolExtensionNames(Guid sid)
        {
            if (sid != default)
            {
                var finds = await _schoolRepository.GetSchoolExtensions(sid);
                if (finds?.Any() == true)
                {
                    foreach (var item in finds)
                    {
                        if (long.TryParse(item.Year, out long schoolNo))
                        {
                            item.Year = UrlShortIdUtil.Long2Base32(schoolNo);
                        }
                    }
                    return finds;
                }
            }
            return null;
        }

        public async Task<IEnumerable<KeyValueDTO<Guid, Guid, string, string, int>>> GetRecommendExtension(byte type, byte grade, int city, Guid eid, int take = 2)
        {
            return await _schoolRepository.GetRecommendExtension(type, grade, city, eid, take);
        }

        public async Task<IEnumerable<string>> GetSchoolFieldYears(Guid eid, string field)
        {
            return await _schoolRepository.GetSchoolFieldYears(eid, field);
        }

        public async Task<IEnumerable<(Guid EID, long SchoolNo, string Base32String)>> GetSchoolNosAsync(IEnumerable<Guid> eids)
        {
            if (eids == null || !eids.Any()) return null;
            var finds = await _schoolRepository.GetSchoolNosAsync(eids);
            if (finds?.Any() == true)
            {
                return finds.Select(p => (p.Key, p.Value, UrlShortIdUtil.Long2Base32(p.Value)));
            }
            return null;
        }

        public async Task<string> GetSchoolFieldContentAsync(Guid eid, string field, int year)
        {
            var finds = await _schoolRepository.GetSchoolYearFieldsAsync(eid, field, year);
            if (finds?.Any(p => p.EID == eid && p.Year == year) == true)
            {
                return finds.FirstOrDefault(p => p.EID == eid && p.Year == year)?.Content;
            }
            return null;
        }

        public async Task<Dictionary<Guid, SchoolGradeType>> GetExtensionGradesAsync(IEnumerable<Guid> eids)
        {
            if (eids == null || !eids.Any()) return null;
            return await _schoolRepository.GetExtensionGradesAsync(eids);
        }

        public async Task<IEnumerable<ExtensionCounterPartAllocationDTO>> GetCouterPartAndAllocationAsync(IEnumerable<Guid> eids)
        {
            return await _schoolRepository.GetExtensionCounterPartAllocationAsync(eids) ?? new List<ExtensionCounterPartAllocationDTO>();
        }

        public async Task<IEnumerable<OnlineExtensionCounterPartAllocationDTO>> GetOnlineCouterPartAndAllocationAsync(IEnumerable<Guid> eids)
        {
            return await _schoolRepository.GetOnlineExtensionCounterPartAllocationAsync(eids) ?? new List<OnlineExtensionCounterPartAllocationDTO>();
        }

        public async Task<bool> UpdateCouterPartAndAllocationAsync(Guid eid, int year, string counterPartJSON, string allocationJSON)
        {
            return await _schoolRepository.UpdateCouterPartAndAllocationAsync(eid, year, counterPartJSON, allocationJSON);
        }

        public async Task<IEnumerable<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>> GetSchoolAndExtensionNameAsync(IEnumerable<Guid> eids)
        {
            return await _schoolRepository.GetSchoolAndExtensionNameAsync(eids);
        }

        public async Task<int> UpdateExtNicknameAsync(Guid eid, string json)
        {
            return await _schoolRepository.UpdateExtNicknameAsync(eid, json);
        }

        public async Task<IEnumerable<YearExtFieldInfo>> ListFieldYearsAsync(IEnumerable<Guid> eids, string field = default)
        {
            return await _schoolRepository.ListFieldYearsAsync(eids, field);
        }

        public async Task<int> InsertOrUpdateExtFieldYearAsync(YearExtFieldInfo entity)
        {
            return await _schoolRepository.InsertOrUpdateExtFieldYearAsync(entity);
        }

        public async Task<IEnumerable<ExtSimpleDTO>> GetExtSimpleInfoAsync(IEnumerable<Guid> eids)
        {
            return await _schoolRepository.GetExtSimpleInfoAsync(eids);
        }

        public async Task<IEnumerable<(Guid SID, Guid EID, string SchoolName, string ExtName, int ExtNo)>> ListRecommendSchoolAsync(Guid eid, int top = 8)
        {
            return await _schoolRepository.ListRecommendSchoolAsync(eid, top);
        }

        public async Task<bool> SubscribeAsync(Guid eid, Guid userID)
        {
            if (eid == default || userID == default) return default;
            return await _schoolRepository.InsertExtensionSubscribeIfNotExistAsync(eid, userID);
        }

        public async Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid)
        {
            return await _schoolRepository.ListValidEIDsAsync(sid);
        }

        public async Task<IEnumerable<SchoolSurroundingPoiInfo>> ListExtSurroundInfosAsync(double longitude, double latitude, int distance = 1950)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.ListSurroundInfoAsync(longitude, latitude, distance);
        }
        public async Task<IEnumerable<SchoolSurroundingPoiInfo>> PageExtSurroundInfosAsync(double longitude, double latitude, int distance = 1950, int pageIndex = 2, int pageSize = 5, int? typeCode = default)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.PageSurroundInfoAsync(longitude, latitude, distance, pageIndex, pageSize, typeCode);
        }

        public async Task<IEnumerable<SchoolSurroundingBuildingInfo>> PageExtSurroundBuildingsAsync(double longitude, double latitude, int distance = 1950, int pageIndex = 1, int pageSize = 5)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.ListSurroundBuildingAsync(longitude, latitude, distance, pageIndex, pageSize);
        }

        public async Task<int> GetSurroundBuildingCountAsync(double longitude, double latitude, int distance = 1950)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.GetSurroundBuildingCountAsync(longitude, latitude, distance);
        }

        public async Task<IEnumerable<(double Distance, Guid EID)>> ListSurroundExtIDsAsync(double longitude, double latitude, IEnumerable<SchoolGradeType> grades, int distance = 2750, int take = 10)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.ListSurroundExtIDsAsync(longitude, latitude, grades, distance, take);
        }

        public async Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingsAsync(IEnumerable<Guid> ids)
        {
            if (ids?.Any() == true) return await _schoolRepository.ListSurroundBuildingsAsync(ids);
            return default;
        }

        public async Task<IEnumerable<(int TypeCode, int Count)>> ListSurroundCountAsync(double longitude, double latitude, int distance = 1950)
        {
            if (longitude < 0 || latitude < 0 || distance < 1) return default;
            return await _schoolRepository.ListSurroundCountAsync(longitude, latitude, distance);
        }

        public async Task<IEnumerable<ExtSimpleDTO>> ListExtInfoForSurroundAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true) return await _schoolRepository.ListExtSimpleInfosAsync(eids);
            return default;
        }
    }
}
