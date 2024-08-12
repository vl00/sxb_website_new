using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using Sxb.School.Query.SQL.DB;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Query.SQL.Repository
{
    public class SchoolRepository : ISchoolRepository
    {
        readonly SchoolDataDB _schoolDataDB;
        public SchoolRepository(SchoolDataDB schoolDataDB)
        {
            _schoolDataDB = schoolDataDB;
        }

        public async Task<SchoolExtensionDTO> GetExtensionDTO(Guid eid, long schoolNo)
        {
            var str_Where = "ext.id=@extId";
            if (eid == default && schoolNo > 0) str_Where = "ext.no = @schoolNo";

            var sql = @$"SELECT TOP 1 school.name+' - '+ext.name as Name,school.name as schoolname,ext.name as extname,
                        ext.No as SchoolNo,
                        school.name_e AS EName,ext.id as ExtId,ext.sid,
                        school.website,school.logo,Charge.tuition,Charge.applicationfee,
                        Charge.otherfee,ext.type,ext.discount,
                        ext.diglossia,ext.chinese,school.intro,
                        content.tel,school.website,content.address,content.lodging,content.sdextern,content.canteen,
                        content.meal,content.characteristic,
                        content.authentication,content.foreignTea,content.abroad,
                        content.openhours,content.calendar,content.range,
                        content.afterclass,content.counterpart,content.province,
                        content.city,content.area,content.longitude,content.latitude,content.Allocation,
                        ext.grade,content.studentcount,content.teachercount,content.studentcount,
                        content.tsPercent,recruit.age,recruit.maxage,
                        recruit.target,recruit.proportion,recruit.point,recruit.count,
                        course.courses,course.authentication AS CourseAuthentication,
                        course.characteristic as CourseCharacteristic,area.name AS areaname,city.name AS cityname,content.HasSchoolBus,school.EduSysType
                        ,overviewinfo.RecruitWay
                        ,overviewinfo.OAName,overviewinfo.OAAppID,overviewinfo.OAAccount
                        ,overviewinfo.MPName,overviewinfo.MPAppID,overviewinfo.MPAccount
                        ,overviewinfo.VANAme,overviewinfo.VAAppID,overviewinfo.VAAccount,
                        ext.nickname
                        FROM dbo.OnlineSchoolExtension AS ext
                        LEFT JOIN dbo.OnlineSchool AS school ON ext.sid=school.id
                        LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id=content.eid AND content.IsValid=1
                        LEFT JOIN dbo.OnlineSchoolExtCharge AS Charge ON ext.id=Charge.eid AND Charge.IsValid=1
                        LEFT JOIN dbo.OnlineSchoolExtRecruit AS recruit ON ext.id=recruit.eid AND recruit.IsValid=1
                        LEFT JOIN dbo.OnlineSchoolExtCourse AS course ON  ext.id=course.eid AND course.IsValid=1
                        LEFT JOIN dbo.KeyValue AS city ON city.id=content.city
                        LEFT JOIN dbo.KeyValue AS area ON area.id=content.area
                        LEFT JOIN SchoolOverViewInfo overviewinfo ON overviewinfo.EID = ext.id
                        WHERE {str_Where} AND school.IsValid=1 AND ext.IsValid=1  and school.status=@status";

            var data = await _schoolDataDB.SlaveConnection.QuerySingleAsync<SchoolExtensionDTO>(sql, new { extId = eid, status = 3, schoolNo });
            if (data != null)
            {
                StringBuilder achSQL = new StringBuilder();
                if (data.Grade == (byte)SchoolGradeType.SeniorMiddleSchool)
                {
                    //高中
                    achSQL.Append(@"SELECT ach.schoolId AS [key],college.name AS [value],ach.count AS [message],ach.year AS [data]  FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT JOIN dbo.College AS college ON ach.schoolId=college.id
                WHERE ach.IsValid=1 AND college.IsValid=1 AND ach.extId=@extId AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc) ORDER BY ach.year desc");
                }
                else if (data.Grade != (byte)SchoolGradeType.Kindergarten && data.Type != (byte)SchoolType.SAR)
                {
                    achSQL.Append(@"SELECT ach.schoolId AS [key],sch.name AS [value],ach.count AS [message],ach.year AS [data]
                FROM dbo.OnlineSchoolAchievement AS ach 
                LEFT  JOIN  dbo.OnlineSchoolExtension AS ext ON ach.schoolId =ext.id
                LEFT JOIN dbo.OnlineSchool AS sch ON sch.id=ext.sid
               WHERE ach.extId=@extId AND ach.IsValid=1  AND ext.IsValid=1  AND ach.YEAR=(select top 1 year from OnlineSchoolAchievement where extId=@extId order by year desc)
                 ORDER BY ach.year,ach.count DESC");
                }
                if (!new byte[] { (byte)SchoolGradeType.Kindergarten, (byte)SchoolGradeType.PrimarySchool }.Contains(data.Grade))
                {
                    //升学成绩
                    data.Achievement = _schoolDataDB.SlaveConnection.Query<KeyValueDTO<Guid, string, double, int>>(achSQL.ToString(), new { extId = eid.ToString() }).ToList();
                    if (data.Achievement != null && data.Achievement.Any())
                    {
                        data.AchYear = data.Achievement.FirstOrDefault().Data;
                    }
                    else
                    {
                        switch (data.Grade)
                        {
                            case (byte)SchoolGradeType.JuniorMiddleSchool:
                                data.AchYear = _schoolDataDB.SlaveConnection.Query<int>("select top 1 year from OnlineMiddleSchoolAchievement where extId=@extId and IsValid = 1 order by year desc;", new { extId = eid }).FirstOrDefault();
                                break;
                            case (byte)SchoolGradeType.SeniorMiddleSchool:
                                data.AchYear = _schoolDataDB.SlaveConnection.Query<int>("select top 1 year from OnlineHighSchoolAchievement where extId=@extId  and IsValid = 1 order by year desc;", new { extId = eid }).FirstOrDefault();
                                break;
                        }
                    }
                }

            }
            return data;
        }

        public async Task<Dictionary<Guid, SchoolGradeType>> GetExtensionGradesAsync(IEnumerable<Guid> eids)
        {
            if (eids == null | !eids.Any()) return null;
            var finds = await _schoolDataDB.SlaveConnection.QuerySet<OnlineSchoolExtensionInfo>().Where($"ID in {eids.ToSQLInString()}").Select(p => new { p.ID, p.Grade }).ToIEnumerableAsync();
            if (finds?.Any() == true) return finds.ToDictionary(k => k.ID, v => v.Grade);
            return null;
        }

        public async Task<IEnumerable<KeyValueDTO<Guid, Guid, string, string, int>>> GetRecommendExtension(byte type, byte grade, int city, Guid eid, int take)
        {
            if (eid == default) return null;
            if (take < 1) take = 1;
            var sql = $@"SELECT  TOP {++take} ext.sid AS [SID],ext.id AS [EID],sch.name AS [SchoolName],ext.name AS [ExtName], ext.no as [ExtNo],scores.score as [Score] FROM dbo.OnlineSchoolExtension AS ext
            LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
            LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
            LEFT JOIN ScoreTotal AS scores ON scores.eid=ext.id 
            WHERE content.city =@city and ext.IsValid=1 
            AND ext.type = @type AND ext.grade = @grade
            AND sch.status = @status ORDER BY scores.score DESC";

            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid SID, Guid EID, string SchoolName, string ExtName, int ExtNo, int Score)>(sql, new { city, type, grade, status = (byte)3, eid });
            if (finds?.Any() == true)
            {
                return finds.Where(p => p.EID != eid).OrderByDescending(p => p.Score).Select(p => new KeyValueDTO<Guid, Guid, string, string, int>()
                {
                    Key = p.SID,
                    Value = p.EID,
                    Message = p.SchoolName,
                    Data = p.ExtName,
                    Other = p.ExtNo
                });
            }
            return default;
        }

        public async Task<IEnumerable<KeyValueDTO<Guid, int, string, string>>> GetSchoolExtAddress(IEnumerable<Guid> eids)
        {
            Console.WriteLine(await _schoolDataDB.SlaveConnection.QuerySingleAsync<string>("Select @@serverName"));
            var str_SQL = @$"SELECT
	                            ose.id AS [Key],
	                            ose.[No] AS [Value],
	                            osec.address AS [Data],
	                            os.name + ' - ' + ose.name AS [Message] 
                            FROM
	                            OnlineSchoolExtContent AS osec
	                            LEFT JOIN OnlineSchoolExtension AS ose ON ose.id = osec.eid
	                            LEFT JOIN OnlineSchool AS os ON os.id = ose.sid 
                            WHERE
	                            osec.eid IN {eids.ToSQLInString()}";
            return await _schoolDataDB.SlaveConnection.QueryAsync<KeyValueDTO<Guid, int, string, string>>(str_SQL, new { });
        }

        public async Task<IEnumerable<SchoolImageDTO>> GetSchoolExtensionImages(Guid eid, long schoolNo, IEnumerable<SchoolImageType> types)
        {
            var str_SQL = @"SELECT
                                url,
	                            surl,
	                            imageDesc,
	                            type,
                                sort
                            FROM
                                [dbo].[OnlineSchoolImage]
                            WHERE
                                IsValid = 1
                                And eid = @eid";
            if (types?.Any() == true)
            {
                str_SQL += $" And type in ({string.Join(",", types.Where(p => p != 0).Select(p => (int)p))})";
            }
            return await _schoolDataDB.SlaveConnection.QueryAsync<SchoolImageDTO>(str_SQL, new { eid });
        }

        public async Task<IEnumerable<KeyValueDTO<Guid>>> GetSchoolExtensions(Guid sid)
        {
            var str_SQL = "SELECT name AS [key],id AS [value] , no as [year] FROM dbo.OnlineSchoolExtension WHERE sid=@sid AND IsValid=1";
            return await _schoolDataDB.SlaveConnection.QueryAsync<KeyValueDTO<Guid>>(str_SQL, new { sid }, null, null, null);
        }

        public async Task<IEnumerable<string>> GetSchoolFieldYears(Guid eid, string field)
        {
            if (eid == default || string.IsNullOrWhiteSpace(field)) return null;
            var str_SQL = "Select top 1 years from OnlineYearExtField WHERE field = @field and eid = @eid;";
            var find = await _schoolDataDB.SlaveConnection.QuerySingleAsync<string>(str_SQL, new { eid, field });
            if (!string.IsNullOrWhiteSpace(find)) return find.Split(',').OrderByDescending(p => p);
            return null;
        }

        public async Task<Dictionary<Guid, long>> GetSchoolNosAsync(IEnumerable<Guid> eids)
        {
            if (eids == null || !eids.Any()) return null;
            var str_SQL = "Select id,[No] from OnlineSchoolExtension WHERE ID in @eids";
            eids = eids.Distinct();
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid, long)>(str_SQL, new { eids });
            if (finds?.Any() == true)
            {
                return finds.ToDictionary(k => k.Item1, v => v.Item2);
            }
            return null;
        }

        public async Task<IEnumerable<KeyValueDTO<DateTime, string, byte, string, string>>> GetSchoolVideos(Guid extId, SchoolVideoType type = SchoolVideoType.Unknow)
        {
            var sql = @"SELECT
	                        CreateTime AS [key],
	                        videoUrl AS [value],
	                        type AS message ,
                            videoDesc as [Data],
                            cover as [Other]
                        FROM
	                        dbo.OnlineSchoolVideo 
                        WHERE
	                        eid = @extId 
	                        AND IsValid = 1";
            if (type != 0)
            {
                sql += $"and type={(int)type}";
            }
            return await _schoolDataDB.SlaveConnection.QueryAsync<KeyValueDTO<DateTime, string, byte, string, string>>(sql, new { extId });
        }

        public async Task<IEnumerable<OnlineSchoolYearFieldContentInfo>> GetSchoolYearFieldsAsync(Guid eid, string field, int year, bool ignoreIsValid = false)
        {
            if (eid == default || string.IsNullOrWhiteSpace(field) || year < 1800) return null;
            var str_Where = "and IsValid=1";
            if (ignoreIsValid) str_Where = string.Empty;
            var str_SQL = $"Select * From OnlineSchoolYearFieldContent_{year} where eid=@eid and field=@field {str_Where} order by Year desc";
            return await _schoolDataDB.SlaveConnection.QueryAsync<OnlineSchoolYearFieldContentInfo>(str_SQL, new { eid, field });
        }

        public async Task<IEnumerable<ExtensionCounterPartAllocationDTO>> GetExtensionCounterPartAllocationAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                var result = new List<ExtensionCounterPartAllocationDTO>();
                var index = 0;
                do
                {
                    var ids = eids.Skip(index).Take(500);
                    var finds = await _schoolDataDB.SlaveConnection.QuerySet<ExtensionCounterPartAllocationDTO>().Where($"EID in {ids.ToSQLInString()}").ToIEnumerableAsync();
                    if (finds?.Any() == true) result.AddRange(finds);
                    index += 500;
                } while (index < eids.Count());
                if (result.Any()) return result;
            }
            return null;
        }

        public async Task<IEnumerable<OnlineExtensionCounterPartAllocationDTO>> GetOnlineExtensionCounterPartAllocationAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                var result = new List<OnlineExtensionCounterPartAllocationDTO>();
                var index = 0;
                do
                {
                    var ids = eids.Skip(index).Take(500);
                    var finds = await _schoolDataDB.SlaveConnection.QuerySet<OnlineExtensionCounterPartAllocationDTO>().Where($"EID in {ids.ToSQLInString()}").ToIEnumerableAsync();
                    if (finds?.Any() == true) result.AddRange(finds);
                    index += 500;
                } while (index < eids.Count());
                if (result.Any()) return result;
            }
            return null;
        }

        public async Task<bool> UpdateCouterPartAndAllocationAsync(Guid eid, int year, string counterPartJSON, string allocationJSON)
        {
            var str_SQL = "Update [SchoolExtContent] Set [counterpart] = @counterPartJSON , [Allocation] = @allocationJSON Where [EID] = @eid";
            int effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { counterPartJSON, allocationJSON, eid });
            if (effectCount > 0)
            {
                str_SQL = str_SQL.Replace("[SchoolExtContent]", "[OnlineSchoolExtContent]");
                effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { counterPartJSON, allocationJSON, eid });
                if (effectCount > 0)
                {
                    if (!string.IsNullOrWhiteSpace(counterPartJSON))
                    {
                        var entity_FieldContent = (await GetSchoolYearFieldsAsync(eid, "Counterpart", year))?.FirstOrDefault();
                        if (entity_FieldContent?.ID != default)
                        {
                            entity_FieldContent.Content = counterPartJSON;
                        }
                        else
                        {
                            entity_FieldContent = new OnlineSchoolYearFieldContentInfo()
                            {
                                Content = counterPartJSON,
                                Field = "Counterpart",
                                IsValid = true,
                                Year = year,
                                EID = eid
                            };
                        }
                        await InsertOrUpdateExtFieldContentAsync(entity_FieldContent);
                    }
                    if (!string.IsNullOrWhiteSpace(allocationJSON))
                    {
                        var entity_FieldContent = (await GetSchoolYearFieldsAsync(eid, "Allocation", year))?.FirstOrDefault();
                        if (entity_FieldContent?.ID != default)
                        {
                            entity_FieldContent.Content = allocationJSON;
                        }
                        else
                        {
                            entity_FieldContent = new OnlineSchoolYearFieldContentInfo()
                            {
                                Content = allocationJSON,
                                Field = "Allocation",
                                IsValid = true,
                                Year = year,
                                EID = eid
                            };
                        }
                        await InsertOrUpdateExtFieldContentAsync(entity_FieldContent);
                    }
                }
            }
            return effectCount > 0;
        }

        public async Task<IEnumerable<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>> GetSchoolAndExtensionNameAsync(IEnumerable<Guid> eids)
        {
            if (eids?.Any() == true)
            {
                var str_SQL = "Select se.id as [EID],s.name as [SchoolName],se.name as [ExtName] from OnlineSchoolExtension as se LEFT JOIN OnlineSchool as s on s.id = se.sid WHERE se.id in @ids";
                var index = 0;
                IEnumerable<Guid> ids;
                var result = new List<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>();
                while ((ids = eids.Skip(index).Take(200))?.Any() == true)
                {
                    var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid EID, string SchoolName, string ExtName)>(str_SQL, new { ids });
                    if (finds?.Any() == true)
                    {
                        result.AddRange(finds.Select(p => new KeyValuePair<Guid, (string SchoolName, string ExtensionName)>(p.EID, (p.SchoolName, p.ExtName))));
                    }
                    index += 200;
                }
                if (result.Any()) return result;
            }
            return default;
        }

        public async Task<int> UpdateExtNicknameAsync(Guid eid, string json)
        {
            var effectCount = 0;
            if (eid == default || string.IsNullOrWhiteSpace(json)) return 0;
            effectCount = await _schoolDataDB.Connection.CommandSet<SchoolExtensionInfo>().Where(p => p.ID == eid).UpdateAsync(new SchoolExtensionInfo { NickName = json });
            if (effectCount > 0)
            {
                effectCount = await _schoolDataDB.Connection.CommandSet<OnlineSchoolExtensionInfo>().Where(p => p.ID == eid).UpdateAsync(new OnlineSchoolExtensionInfo { NickName = json });
            }
            return effectCount;
        }

        public async Task<int> InsertOrUpdateExtFieldYearAsync(YearExtFieldInfo entity)
        {
            var effectCount = 0;
            if (entity == default || entity.EID == default || entity.Field == default) return effectCount;
            if (entity.Latest != default)
            {
                var str_SQL = "Update [YearExtField] Set [Years] = @years , [Latest] = @latest Where [EID] = @eid AND [Field] = @field";
                effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { years = entity.Years, latest = entity.Latest, eid = entity.EID, field = entity.Field });
                if (effectCount > 0)
                {
                    str_SQL = str_SQL.Replace("YearExtField", "OnlineYearExtField");
                    effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { years = entity.Years, latest = entity.Latest, eid = entity.EID, field = entity.Field });
                }
            }
            else
            {
                var str_SQL = "Insert Into [YearExtField] ([EID],[Field],[Years],[Latest]) VALUES (@eid,@field,@years,@latest);";
                var latest = entity.Years_Obj?.Max();
                if (latest.HasValue)
                {
                    effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { eid = entity.EID, field = entity.Field, years = entity.Years, latest });
                    if (effectCount > 0)
                    {
                        str_SQL = str_SQL.Replace("YearExtField", "OnlineYearExtField");
                        effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { eid = entity.EID, field = entity.Field, years = entity.Years, latest });
                    }
                }
            }
            return effectCount;
        }

        public async Task<int> InsertOrUpdateExtFieldContentAsync(OnlineSchoolYearFieldContentInfo entity)
        {
            var effectCount = 0;
            if (entity == default || entity.EID == default || string.IsNullOrWhiteSpace(entity.Content) || entity.Year < 2000) return effectCount;
            if (entity.ID == default)
            {
                entity.ID = Guid.NewGuid();
                var str_SQL = $"Insert INTO [SchoolYearFieldContent_{entity.Year}] VALUES(@id,@year,@eid,@field,@content,1);";
                effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new
                {
                    id = entity.ID,
                    year = entity.Year,
                    eid = entity.EID,
                    field = entity.Field,
                    content = entity.Content
                });
                if (effectCount > 0)
                {
                    str_SQL = str_SQL.Replace("SchoolYearFieldContent_", "OnlineSchoolYearFieldContent_");
                    effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new
                    {
                        id = entity.ID,
                        year = entity.Year,
                        eid = entity.EID,
                        field = entity.Field,
                        content = entity.Content
                    });
                }
            }
            else
            {
                var str_SQL = $"Update [SchoolYearFieldContent_{entity.Year}] Set [Content] = @content Where [ID] = @id;";
                effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { content = entity.Content, id = entity.ID });
                if (effectCount > 0)
                {
                    str_SQL = str_SQL.Replace("SchoolYearFieldContent_", "OnlineSchoolYearFieldContent_");
                    effectCount = await _schoolDataDB.Connection.ExecuteAsync(str_SQL, new { content = entity.Content, id = entity.ID });
                }
            }
            return effectCount;
        }

        public async Task<IEnumerable<YearExtFieldInfo>> ListFieldYearsAsync(IEnumerable<Guid> eids, string field = null)
        {
            if (eids == default || !eids.Any()) return default;
            var index = 0;
            IEnumerable<Guid> ids;
            var result = new List<YearExtFieldInfo>();
            while ((ids = eids.Skip(index).Take(200))?.Any() == true)
            {
                var query = _schoolDataDB.SlaveConnection.QuerySet<YearExtFieldInfo>().Where($"EID IN {ids.ToSQLInString()}");
                if (!string.IsNullOrWhiteSpace(field)) query = query.Where(p => p.Field == field);
                result.AddRange(await query.ToIEnumerableAsync());
                index += 200;
            }
            if (result.Any()) return result;
            else return default;
        }

        public async Task<IEnumerable<ExtSimpleDTO>> GetExtSimpleInfoAsync(IEnumerable<Guid> eids)
        {
            if (eids == default || !eids.Any()) return default;
            var str_SQL = $@"
            SELECT
	            os.id AS [SID],
	            ose.id AS [EID],
	            os.name AS [SchoolName],
	            ose.name AS [ExtName],
	            ose.SchFtype,
	            osec.city AS [CityCode],
	            osec.area AS [AreaCode],
	            osec.province as [ProvinceCode],
                ose.grade,
                osec.longitude,
                osec.latitude,
                ose.no
            FROM
	            OnlineSchoolExtension AS ose
	            LEFT JOIN OnlineSchool AS os ON os.id= ose.sid
	            LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = ose.id
            WHERE
                ose.id IN {eids.ToSQLInString()}";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<ExtSimpleDTO>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<(Guid, Guid, string, string, int)>> ListRecommendSchoolAsync(Guid eid, int top = 8)
        {
            if (eid == default || top < 1) return default;
            var sql = $@"DECLARE @extCityCode INT;
                        DECLARE @extType INT;
                        DECLARE @extGrade INT;
                        SELECT
	                        @extGrade = ose.grade,@extType = ose.type,@extCityCode = osec.city 
                        FROM
	                        OnlineSchoolExtension AS ose
	                        LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = ose.id 
                        WHERE
	                        ose.id = @eid;
                        SELECT TOP {top}
	                        ext.sid AS [SID],
	                        ext.id AS [EID],
	                        sch.name AS [SchoolName],
	                        ext.name AS [ExtName],
	                        ext.no AS [ExtNo] 
                        FROM
	                        dbo.OnlineSchoolExtension AS ext
	                        LEFT JOIN dbo.OnlineSchool AS sch ON ext.sid = sch.id
	                        LEFT JOIN dbo.OnlineSchoolExtContent AS content ON ext.id = content.eid
	                        LEFT JOIN ScoreTotal AS scores ON scores.eid= ext.id 
                        WHERE
	                        content.city = @extCityCode 
	                        AND ext.IsValid = 1 
	                        AND sch.IsValid = 1 
	                        AND ext.type = @extType 
	                        AND ext.grade = @extGrade 
	                        AND ext.id != @eid 
	                        AND sch.status = 3 
	                        AND scores.status = 1 
                        ORDER BY
	                        scores.score DESC;";

            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(Guid SID, Guid EID, string SchoolName, string ExtName, int ExtNo)>(sql, new { eid });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<bool> InsertExtensionSubscribeIfNotExistAsync(Guid eid, Guid userID)
        {
            var exist = await _schoolDataDB.SlaveConnection.QuerySet<ExtensionSubscribeInfo>().Where(p => p.EID == eid && p.UserID == userID).Top(1).GetAsync(p => 1);
            if (exist < 1)
            {
                var entity = new ExtensionSubscribeInfo()
                {
                    UserID = userID,
                    CreateTime = DateTime.Now,
                    EID = eid,
                    ID = Guid.NewGuid()
                };
                return await _schoolDataDB.Connection.CommandSet<ExtensionSubscribeInfo>().InsertAsync(entity) > 0;
            }
            return default;
        }

        public async Task<IEnumerable<SchoolSurroundingPoiInfo>> ListSurroundInfoAsync(double longitude, double latitude, int distance = 2000, int take = 5)
        {
            var str_SQL = $@"declare @searchLocation geography;
            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
            select * 
            FROM
            	(
            	SELECT
            		*,
            		row_number() OVER(partition BY t.typeCode ORDER BY t.distance) AS take
            	FROM
            		(
            		SELECT
                        id
                        ,poi_id
                        ,typeCode
                        ,poi_province
                        ,poi_city
                        ,poi_area
                        ,poi_tags
                        ,poi_lat
                        ,poi_long
                        ,poi_name
                        ,poi_photo
                        ,createTime
                        ,poi_address
                        ,poi_price
            			,longlat.STDistance(@searchLocation) AS [Distance]
            		FROM
            			SchoolSurroundingPoi 
            		WHERE
            			longlat.Filter(@searchLocation.STBuffer({distance})) = 1 
            		) AS t 
            	) s 
            WHERE
            	s.take <= {take}
            	ORDER BY s.typeCode,s.distance";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<SchoolSurroundingPoiInfo>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<SchoolSurroundingPoiInfo>> PageSurroundInfoAsync(double longitude, double latitude, int distance = 2000, int pageIndex = 1, int pageSize = 10, int? typeCode = default)
        {
            var str_Where = string.Empty;
            if (typeCode.HasValue) str_Where = $" AND TypeCode = {typeCode.Value}";
            var str_SQL = $@"declare @searchLocation geography;
                            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
                            SELECT        
                                id
                                ,poi_id
                                ,typeCode
                                ,poi_province
                                ,poi_city
                                ,poi_area
                                ,poi_tags
                                ,poi_lat
                                ,poi_long
                                ,poi_name
                                ,poi_photo
                                ,createTime
                                ,poi_address
                                ,poi_price
            			        ,longlat.STDistance(@searchLocation) AS [Distance]
            		        FROM
            			        SchoolSurroundingPoi 
            		        WHERE
            			        longlat.Filter(@searchLocation.STBuffer({distance})) = 1
                                {str_Where}
                            ORDER BY
                                [Distance] OFFSET {--pageIndex * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<SchoolSurroundingPoiInfo>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingAsync(double longitude, double latitude, int distance = 2000, int pageIndex = 1, int pageSize = 10)
        {
            var str_SQL = $@"declare @searchLocation geography;
                            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
                            SELECT
	                            id
                                ,name
                                ,address
                                ,house_id
                                ,house_img
                                ,city
                                ,house_lat
                                ,house_lng
                                ,building_years
                                ,house_price
                                ,house_properties
                                ,house_developers
                                ,createTime
                                ,new_house_img
                                ,longlat.STDistance(@searchLocation) AS [Distance]
                            FROM
	                            SchoolSurroundingBuilding 
                            WHERE
	                            longlat.Filter(@searchLocation.STBuffer({distance})) = 1
                            ORDER BY
                                [Distance] OFFSET {--pageIndex * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY;";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync(str_SQL, new { });
            if (finds?.Any() == true)
            {
                return finds.Select(p => new SchoolSurroundingBuildingInfo()
                {
                    Address = p.address,
                    Building_Years = p.building_years,
                    City = p.city,
                    CreateTime = p.createtime,
                    Distance = p.Distance,
                    House_Developers = p.house_developers,
                    House_ID = p.house_id,
                    House_Img = p.house_img,
                    House_Lat = p.house_lat,
                    House_Lng = p.house_lng,
                    House_Price = p.house_price,
                    House_Properties = p.house_properties,
                    ID = p.id,
                    Name = p.name,
                    New_House_Img = p.new_house_img
                });
            }
            return default;
        }

        public async Task<int> GetSurroundBuildingCountAsync(double longitude, double latitude, int distance = 2000)
        {
            var str_SQL = $@"declare @searchLocation geography;
                            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
                            SELECT
	                            Count(1)
                            FROM
	                            SchoolSurroundingBuilding 
                            WHERE
	                            longlat.Filter(@searchLocation.STBuffer({distance})) = 1;";
            return await _schoolDataDB.SlaveConnection.QuerySingleAsync<int>(str_SQL, new { });
        }

        public async Task<IEnumerable<(double Distance, Guid EID)>> ListSurroundExtIDsAsync(double longitude, double latitude, IEnumerable<SchoolGradeType> grades, int distance = 3000, int take = 10)
        {
            var str_SQL = $@"declare @searchLocation geography;
                            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
                            SELECT 
                                b.Distance,
                                b.id
                            FROM
                                (SELECT 
                                    *,ROW_NUMBER() OVER(PARTITION BY a.grade ORDER BY a.Distance) as Take
                                FROM
                                    (SELECT
	                                    osec.LatLong.STDistance(@searchLocation) AS [Distance],
                                        ose.id,
                                        ose.grade
                                    FROM
	                                    OnlineSchoolExtContent AS osec
                                    LEFT JOIN OnlineSchoolExtension AS ose ON osec.eid = ose.id
                                    LEFT JOIN OnlineSchool AS os ON os.id = ose.sid
                                    WHERE
                                        ose.Grade IN @grades
	                                    AND osec.LatLong.Filter(@searchLocation.STBuffer({distance})) = 1
                                        AND ose.IsValid = 1
                                        AND os.status = 3 
                                        AND os.IsValid = 1
                                        AND osec.IsValid = 1
                                    ) AS a
                                ) AS b
                            WHERE
                                b.take <= {take};";
            //var grade = grades.Select(p => (int)p);
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(double Distance, Guid ID)>(str_SQL, new { grades });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<SchoolSurroundingBuildingInfo>> ListSurroundBuildingsAsync(IEnumerable<Guid> ids)
        {
            if (ids?.Any() == true)
            {
                var finds = await _schoolDataDB.SlaveConnection.QuerySet<SchoolSurroundingBuildingInfo>().Where($"ID in {ids.ToSQLInString()}").ToIEnumerableAsync();
                if (finds?.Any() == true) return finds;
            }
            return default;
        }

        public async Task<IEnumerable<ExtSimpleDTO>> ListExtSimpleInfosAsync(IEnumerable<Guid> eids)
        {
            if (eids == default || !eids.Any()) return default;
            var str_SQL = $@"
            SELECT
                os.name as [SchoolName],
	            ose.name as [ExtName],
	            ose.grade,
	            city.name as [CityName],
	            area.name as [AreaName],
	            osec.lodging,
                osec.sdextern,
	            osech.tuition,
	            st.score,
	            ose.id as [EID],
	            ose.[No],
	            osec.teachercount,
	            osec.studentcount,
                os.EduSysType,
                osec.authentication,
                ose.SchFType
            FROM
                OnlineSchoolExtension AS ose
                LEFT JOIN OnlineSchool AS os ON os.id = ose.sid
                LEFT JOIN OnlineSchoolExtContent AS osec ON osec.eid = ose.id
                LEFT JOIN OnlineSchoolExtCharge AS osech ON osech.eid = ose.id
                LEFT JOIN KeyValue as city on city.id = osec.city
                LEFT JOIN KeyValue as area on Area.id = osec.area
                LEFT JOIN ScoreTotal as st on st.eid = ose.id
            WHERE ose.id in {eids.ToSQLInString()}";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<ExtSimpleDTO>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return default;
        }

        public async Task<IEnumerable<(int TypeCode, int Count)>> ListSurroundCountAsync(double longitude, double latitude, int distance = 2000)
        {
            var str_SQL = $@"declare @searchLocation geography;
                            Set @searchLocation = geography::STPointFromText('POINT({longitude} {latitude})',4326);
                            SELECT
	                            typeCode,
								count(1) as [Count]
                            FROM
	                            SchoolSurroundingPoi 
                            WHERE
	                            longlat.Filter(@searchLocation.STBuffer({distance})) = 1
							GROUP BY 
                                typeCode;";
            var finds = await _schoolDataDB.SlaveConnection.QueryAsync<(int TypeCode, int Count)>(str_SQL, new { });
            if (finds?.Any() == true) return finds;
            return default;
        }


        public async Task<IEnumerable<SchoolIdAndNameDto>> GetSchoolsIdAndName(IEnumerable<Guid> eids = default, IEnumerable<long> nos = default)
        {
            var result = new List<SchoolIdAndNameDto>();
            if (eids?.Any() == true)
            {
                foreach (var arr in SplitArr(eids, 200))
                {
                    var sql = @"
                        select e.id as Eid, e.no as Eno, s.name as Schname, e.name as Extname, (s.IsValid & e.IsValid) as IsValid
                        from OnlineSchoolExtension e 
                        left join OnlineSchool s on e.sid=s.id
                        where e.id in @arr
                    ";
                    var ls = await _schoolDataDB.SlaveConnection.QueryAsync<SchoolIdAndNameDto>(sql, new { arr = arr.Select(_ => _.ToString()) });
                    result.AddRange(ls);
                }
            }
            if (nos?.Any() == true)
            {
                foreach (var arr in SplitArr(nos, 200))
                {
                    var sql = @"
                        select e.id as Eid, e.no as Eno, s.name as Schname, e.name as Extname, (s.IsValid & e.IsValid) as IsValid
                        from OnlineSchoolExtension e 
                        left join OnlineSchool s on e.sid=s.id
                        where e.no in @arr
                    ";
                    var ls = await _schoolDataDB.SlaveConnection.QueryAsync<SchoolIdAndNameDto>(sql, new { arr });
                    result.AddRange(ls);
                }
            }
            return result;

            static IEnumerable<T[]> SplitArr<T>(IEnumerable<T> collection, int c /* c > 0 */)
            {
                for (var arr = collection; arr.Any();)
                {
                    yield return arr.Take(c).ToArray();
                    arr = arr.Skip(c);
                }
            }
        }

        public async Task<IEnumerable<Guid>> ListValidEIDsAsync(Guid sid)
        {
            if (sid != default)
            {
                var finds = await _schoolDataDB.SlaveConnection.QueryAsync<Guid>("Select ID From OnlineSchoolExtension Where SID = @sid AND IsValid = 1;", new { sid });
                if (finds?.Any() == true) return finds;
            }
            return default;
        }
    }
}