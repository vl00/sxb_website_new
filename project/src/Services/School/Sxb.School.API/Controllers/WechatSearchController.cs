using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Query;
using Sxb.School.Common.Entity;
using Sxb.School.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WechatSearchController : ControllerBase
    {
        readonly ISchoolFractionQuery _schoolFractionQuery;
        readonly ISchoolAchievementQuery _schoolAchievementQuery;
        readonly ISchoolExtensionQuery _schoolExtensionQuery;
        readonly IEasyRedisClient _easyRedisClient;
        readonly ISchoolOverViewQuery _schoolOverViewQuery;
        readonly ISchoolQuotaQuery _schoolQuotaQuery;
        readonly ISchoolRecruitQuery _schoolRecruitQuery;
        readonly IAreaRecruitPlanQuery _areaRecruitPlanQuery;
        readonly ISchoolExtensionLevelQuery _schoolExtensionLevelQuery;
        readonly ISchoolProjectQuery _schoolProjectQuery;
        readonly IDataModifyLogQuery _dataModifyLogQuery;

        public WechatSearchController(ISchoolFractionQuery schoolFractionQuery, ISchoolAchievementQuery schoolAchievementQuery, ISchoolExtensionQuery schoolExtensionQuery, IEasyRedisClient easyRedisClient
            , ISchoolOverViewQuery schoolOverViewQuery, ISchoolQuotaQuery schoolQuotaQuery, ISchoolRecruitQuery schoolRecruitQuery, IAreaRecruitPlanQuery areaRecruitPlanQuery
            , ISchoolExtensionLevelQuery schoolExtensionLevelQuery, ISchoolProjectQuery schoolProjectQuery, IDataModifyLogQuery dataModifyLogQuery)
        {
            _schoolProjectQuery = schoolProjectQuery;
            _schoolExtensionLevelQuery = schoolExtensionLevelQuery;
            _areaRecruitPlanQuery = areaRecruitPlanQuery;
            _schoolRecruitQuery = schoolRecruitQuery;
            _schoolQuotaQuery = schoolQuotaQuery;
            _schoolOverViewQuery = schoolOverViewQuery;
            _easyRedisClient = easyRedisClient;
            _schoolExtensionQuery = schoolExtensionQuery;
            _schoolAchievementQuery = schoolAchievementQuery;
            _schoolFractionQuery = schoolFractionQuery;
            _dataModifyLogQuery = dataModifyLogQuery;
        }

        bool CheckUploadPermission(ref ResponseResult result)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() != "development")
            {
                var savedToken = _easyRedisClient.GetStringAsync("UploadDataToken").Result;
                if (string.IsNullOrWhiteSpace(savedToken) || !Request.Form.ContainsKey("uploaddatatoken") || Request.Form["uploaddatatoken"].ToString().ToLower() != savedToken.ToLower().Trim('"'))
                {
                    result.Msg = "Token error";
                    return false;
                }
            }
            return true;
        }

        void SaveModifyLog(IEnumerable<WechatModifyLogInfo> existedDatas, IEnumerable<(bool IsModify, string Description)> diffs, Guid eid, int? year = null, string headTitle = "")
        {
            if (diffs?.Any() == true)
            {
                if (!string.IsNullOrWhiteSpace(headTitle)) headTitle += "-";
                if (diffs.Count() == 1 && diffs.First().Description == "9527" && eid != default)
                {
                    var entity_ModifyLog = existedDatas?.FirstOrDefault(p => p.Yaer == year && p.EID == eid && p.IsModify == false);
                    if (entity_ModifyLog == default)
                    {
                        entity_ModifyLog = new WechatModifyLogInfo()
                        {
                            EID = eid,
                            IsModify = false,
                            CreateDate = DateTime.Now.Date,
                            Yaer = year
                        };

                    }
                    if (entity_ModifyLog.Attrs_Obj?.Any() == true)
                    {
                        entity_ModifyLog.Attrs = new string[] { headTitle + "-全部" }.Concat(entity_ModifyLog.Attrs_Obj).Distinct().ToJson();
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(headTitle)) entity_ModifyLog.Attrs = new string[] { headTitle + "-全部" }.ToJson();
                    }

                    _ = _dataModifyLogQuery.SaveAsync(entity_ModifyLog).Result;
                }
                else
                {
                    foreach (var tmp_IS in diffs.Select(p => p.IsModify).Distinct())
                    {
                        var entity_ModifyLog = existedDatas?.FirstOrDefault(p => p.Yaer == year && p.EID == eid && p.IsModify == tmp_IS);
                        if (entity_ModifyLog == default)
                        {
                            entity_ModifyLog = new WechatModifyLogInfo()
                            {
                                EID = eid,
                                Attrs = diffs.Where(p => p.IsModify == tmp_IS).Select(p => headTitle + p.Description).ToJson(),
                                IsModify = tmp_IS,
                                CreateDate = DateTime.Now.Date,
                                Yaer = year
                            };
                        }
                        else
                        {
                            if (entity_ModifyLog.Attrs_Obj?.Any() == true)
                            {
                                entity_ModifyLog.Attrs = diffs.Where(p => p.IsModify == tmp_IS).Select(p => headTitle + p.Description).Concat(entity_ModifyLog.Attrs_Obj).Distinct().ToJson();
                            }
                        }
                        _ = _dataModifyLogQuery.SaveAsync(entity_ModifyLog).Result;
                    }
                }
            }
        }

        /// <summary>
        /// 上传 对口学校和派位学校
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadCounterPartAndAllocation()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;
            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<(Guid EID, int Year, Guid[] CounterPart, Guid[] Allocation)>();
            foreach (var row in rows)
            {
                (Guid EID, int Year, Guid[] CouterPart, Guid[] Allocations) cellValues = (default, default, default, default);
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.Item1 == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (Guid.TryParse(cell.CellValue, out Guid eid)) { cellValues.EID = eid; }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            if (int.TryParse(cell.CellValue, out int year))
                            {
                                cellValues.Year = year;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 2:
                            var counterPart = cell.CellValue.FromJsonSafe<Guid[]>();
                            if (counterPart != default)
                            {
                                cellValues.CouterPart = counterPart;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 3:
                            var allocations = cell.CellValue.FromJsonSafe<Guid[]>();
                            if (allocations != default)
                            {
                                cellValues.Allocations = allocations;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                    }
                }
                if (!errors.Any(p => p.Item1 == row.RowIndex) && cellValues.Year != default && cellValues.EID != default) insertDatas.Add(cellValues);
            }
            var insertCount = 0;
            var errorEIDs = new List<Guid>();
            if (insertDatas.Any())
            {
                var eids = insertDatas.Select(p => p.EID).Distinct();
                var targetEIDs = new List<Guid>();
                foreach (var item in insertDatas)
                {
                    if (item.CounterPart?.Any() == true) targetEIDs.AddRange(item.CounterPart);
                    if (item.Allocation?.Any() == true) targetEIDs.AddRange(item.Allocation);
                }
                var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(eids);

                var schoolNames = await _schoolExtensionQuery.GetSchoolAndExtensionNameAsync(targetEIDs.Distinct()) ?? new List<KeyValuePair<Guid, (string SchoolName, string ExtensionName)>>();
                var items = await _schoolExtensionQuery.GetCouterPartAndAllocationAsync(eids);
                var onlineItems = await _schoolExtensionQuery.GetCouterPartAndAllocationAsync(eids);
                var counterPartFieldYears = await _schoolExtensionQuery.ListFieldYearsAsync(eids, "Counterpart");
                var allocationFieldYears = await _schoolExtensionQuery.ListFieldYearsAsync(eids, "Allocation");
                foreach (var item in insertDatas)
                {
                    if (!items.Any(p => p.EID == item.EID || !onlineItems.Any(p => p.EID == item.EID)))
                    {
                        errorEIDs.Add(item.EID);
                        continue;
                    }
                    string counterPartJson = null;
                    string allocationJson = null;
                    if (item.CounterPart?.Any() == true)
                    {
                        var entity_CounterPartYear = counterPartFieldYears?.FirstOrDefault(p => p.EID == item.EID);
                        if (entity_CounterPartYear == default)
                        {
                            entity_CounterPartYear = new YearExtFieldInfo()
                            {
                                EID = item.EID,
                                Field = "Counterpart",
                                Years = item.Year.ToString()
                            };
                        }
                        else
                        {
                            if (entity_CounterPartYear.Years_Obj == default || !entity_CounterPartYear.Years_Obj.Contains(item.Year))
                            {
                                entity_CounterPartYear.Years = $"{entity_CounterPartYear.Years},{item.Year}";
                                entity_CounterPartYear.Latest = entity_CounterPartYear.Years_Obj.Max();
                            }
                        }
                        if (await _schoolExtensionQuery.InsertOrUpdateExtFieldYearAsync(entity_CounterPartYear) < 1)
                        {
                            errorEIDs.Add(item.EID);
                            continue;
                        }
                        counterPartJson = item.CounterPart.Select(p =>
                        {
                            var schoolName = "未收录";
                            if (schoolNames.Any(k => k.Key == p))
                            {
                                var find_SchoolName = schoolNames.FirstOrDefault(k => k.Key == p);
                                schoolName = $"{find_SchoolName.Value.SchoolName}_{find_SchoolName.Value.ExtensionName}";
                            }
                            return new KeyValuePair<string, Guid>(schoolName, p);
                        })?.ToJson();
                    }
                    if (item.Allocation?.Any() == true)
                    {
                        var entity_AllocationYear = allocationFieldYears?.FirstOrDefault(p => p.EID == item.EID);
                        if (entity_AllocationYear == default)
                        {
                            entity_AllocationYear = new YearExtFieldInfo()
                            {
                                EID = item.EID,
                                Field = "Allocation",
                                Years = item.Year.ToString()
                            };
                        }
                        else
                        {
                            if (entity_AllocationYear.Years_Obj == default || !entity_AllocationYear.Years_Obj.Contains(item.Year))
                            {
                                entity_AllocationYear.Years = $"{entity_AllocationYear.Years},{item.Year}";
                                entity_AllocationYear.Latest = entity_AllocationYear.Years_Obj.Max();
                            }
                        }
                        if (await _schoolExtensionQuery.InsertOrUpdateExtFieldYearAsync(entity_AllocationYear) < 1)
                        {
                            errorEIDs.Add(item.EID);
                            continue;
                        }

                        allocationJson = item.Allocation.Select(p =>
                        {
                            var schoolName = "未收录";
                            if (schoolNames.Any(k => k.Key == p))
                            {
                                var find_SchoolName = schoolNames.FirstOrDefault(k => k.Key == p);
                                schoolName = $"{find_SchoolName.Value.SchoolName}_{find_SchoolName.Value.ExtensionName}";
                            }
                            return new KeyValuePair<string, Guid>(schoolName, p);
                        })?.ToJson();
                    }
                    if (await _schoolExtensionQuery.UpdateCouterPartAndAllocationAsync(item.EID, item.Year, counterPartJson, allocationJson)) { insertCount++; } else { errorEIDs.Add(item.EID); }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count(), insertCount, errors, errorEIDs });
            result.Data = new { dataCount = insertDatas.Count(), insertCount, errors, errorEIDs };
            return result;
        }

        /// <summary>
        /// 上传 学校其他信息
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadSchoolOverView()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<SchoolOverViewInfo>();
            foreach (var row in rows)
            {
                var entity = new SchoolOverViewInfo();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.Item1 == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (Guid.TryParse(cell.CellValue, out Guid eid))
                            {
                                entity.EID = eid;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            var recruitWay = cell.CellValue.FromJsonSafe<string[]>();
                            if (recruitWay?.Any() == true) entity.RecruitWay = cell.CellValue;
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 2:
                            entity.OAName = cell.CellValue;
                            break;
                        case 3:
                            entity.OAAppID = cell.CellValue;
                            break;
                        case 4:
                            entity.MPName = cell.CellValue;
                            break;
                        case 5:
                            entity.MPAppID = cell.CellValue;
                            break;
                        case 6:
                            entity.VAName = cell.CellValue;
                            break;
                        case 7:
                            entity.VAAppID = cell.CellValue;
                            break;
                        case 8:
                            entity.OAAccount = cell.CellValue;
                            break;
                        case 9:
                            entity.MPAccount = cell.CellValue;
                            break;
                        case 10:
                            entity.VAAccount = cell.CellValue;
                            break;
                        case 11:
                            var certifications = cell.CellValue.FromJsonSafe<string[]>();
                            if (certifications?.Any(p => !string.IsNullOrWhiteSpace(p)) == true) entity.Certifications = certifications.Where(p => !string.IsNullOrWhiteSpace(p)).ToJson();
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)) insertDatas.Add(entity);
            }
            var insertCount = 0;
            if (insertDatas.Any())
            {
                //await _schoolOverViewQuery.RemoveByEIDsAsync(insertDatas.Select(p => p.EID).Distinct());
                var exist_Logs = await _dataModifyLogQuery.ListByEIDAsync(insertDatas.Select(p => p.EID).Distinct());
                foreach (var item in insertDatas)
                {
                    var entity = await _schoolOverViewQuery.GetByEID(item.EID);
                    if (entity?.ID != default) item.ID = entity.ID;
                    if (await _schoolOverViewQuery.SaveAsync(item))
                    {
                        insertCount++;
                        SaveModifyLog(exist_Logs, entity.CompareObjFieldValue(item), item.EID, headTitle: "学校其他信息");
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 学部分数线
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadExtensionFraction()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData(1);
            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<ExtensionFractionInfo>();
            foreach (var row in rows)
            {
                var entity = new ExtensionFractionInfo();
                var items = new List<KeyValuePair<string, string>>();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    if (cell.CellIndex < 3)
                    {
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (Guid.TryParse(cell.CellValue, out Guid eid))
                                {
                                    entity.EID = eid;
                                }
                                else
                                {
                                    errors.Add((1, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                if (int.TryParse(cell.CellValue, out int fractionType))
                                {
                                    entity.Type = (ExtensionFractionType)fractionType;
                                }
                                else
                                {
                                    errors.Add((1, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 2:
                                if (int.TryParse(cell.CellValue, out int year))
                                {
                                    entity.Year = year;
                                }
                                else
                                {
                                    errors.Add((1, row.RowIndex, cell.CellIndex));
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                        {
                            items.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                        }
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex) && entity.EID != default && entity.Year > 1900 && items.Any())
                {
                    entity.Items = items.ToJson();
                    insertDatas.Add(entity);
                }
            }

            #region Tables
            rows = file.OpenReadStream().ExcelToData(2);
            foreach (var row in rows)
            {
                var entity = new ExtensionFractionInfo();
                var tables = new List<KeyValuePair<string, string>>();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    if (cell.CellIndex < 3)
                    {
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (Guid.TryParse(cell.CellValue, out Guid eid))
                                {
                                    entity.EID = eid;
                                }
                                else
                                {
                                    errors.Add((2, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                if (int.TryParse(cell.CellValue, out int fractionType))
                                {
                                    entity.Type = (ExtensionFractionType)fractionType;
                                }
                                else
                                {
                                    errors.Add((2, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 2:
                                if (int.TryParse(cell.CellValue, out int year))
                                {
                                    entity.Year = year;
                                }
                                else
                                {
                                    errors.Add((2, row.RowIndex, cell.CellIndex));
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                        {
                            tables.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                        }
                    }
                }
                if (!errors.Any(p => p.Item1 == row.RowIndex) && entity.EID != default && entity.Year > 1900 && tables.Any())
                {
                    if (insertDatas.Any(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year))
                    {
                        var find = insertDatas.FirstOrDefault(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year);
                        find.Tables = tables.ToJson();
                    }
                    else
                    {
                        entity.Tables = tables.ToJson();
                        insertDatas.Add(entity);
                    }
                }
            }
            #endregion

            var insertCount = 0;
            if (insertDatas.Any())
            {
                //await _schoolFractionQuery.RemoveByEIDsAsync(insertDatas.Select(p => p.EID).Distinct());
                var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(insertDatas.Select(p => p.EID).Distinct());
                foreach (var item in insertDatas)
                {
                    var entity = await _schoolFractionQuery.GetAysnc(item.EID, item.Year, (int)item.Type);
                    if (entity?.ID != default) item.ID = entity.ID;
                    if (await _schoolFractionQuery.SaveAsync(item))
                    {
                        insertCount++;
                        SaveModifyLog(modifyLogs, entity.CompareObjFieldValue(item), item.EID, item.Year, "学部分数线");
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 学部升学成绩
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadExtensionAchievement()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();

            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<ExtensionAchievementInfo>();

            #region 字段
            foreach (var row in rows)
            {
                if (!row.Cells.Any(p => !string.IsNullOrWhiteSpace(p.CellValue))) continue;
                var entity = new ExtensionAchievementInfo();
                var items = new List<KeyValuePair<string, string>>();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.Item1 == row.RowIndex)) break;
                    if (cell.CellIndex < 2)
                    {
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (Guid.TryParse(cell.CellValue, out Guid eid))
                                {
                                    entity.EID = eid;
                                }
                                else
                                {
                                    errors.Add((0, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                if (int.TryParse(cell.CellValue, out int year))
                                {
                                    entity.Year = year;
                                }
                                else
                                {
                                    errors.Add((0, row.RowIndex, cell.CellIndex));
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                        {
                            items.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                        }
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex) && entity.EID != default && entity.Year > 1900)
                {
                    if (items.Any()) entity.Items = items.ToJson();
                    insertDatas.Add(entity);
                }
            }
            #endregion

            #region 表格
            rows = file.OpenReadStream().ExcelToData(sheetIndex: 1);
            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new ExtensionAchievementInfo();
                    var tables = new List<KeyValuePair<string, string>>();
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                        if (cell.CellIndex < 2)
                        {
                            switch (cell.CellIndex)
                            {
                                case 0:
                                    if (Guid.TryParse(cell.CellValue, out Guid eid))
                                    {
                                        entity.EID = eid;
                                    }
                                    else
                                    {
                                        errors.Add((1, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                                case 1:
                                    if (int.TryParse(cell.CellValue, out int year))
                                    {
                                        entity.Year = year;
                                    }
                                    else
                                    {
                                        errors.Add((1, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                tables.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                            }
                        }
                    }

                    if (!errors.Any(p => p.RowIndex == row.RowIndex) && entity.EID != default && entity.Year > 1900)
                    {
                        if (insertDatas.Any(p => p.EID == entity.EID && p.Year == entity.Year))
                        {
                            var find = insertDatas.FirstOrDefault(p => p.EID == entity.EID && p.Year == entity.Year);
                            find.Tables = tables.ToJson();
                        }
                        else
                        {
                            entity.Tables = tables.ToJson();
                            insertDatas.Add(entity);
                        }
                    }
                }
            }
            #endregion

            var insertCount = 0;
            if (insertDatas.Any())
            {
                //await _schoolAchievementQuery.RemoveByEIDsAsync(insertDatas.Select(p => p.EID).Distinct());
                var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(insertDatas.Select(p => p.EID).Distinct());
                foreach (var item in insertDatas)
                {
                    var entity = await _schoolAchievementQuery.GetAsync(item.ID, item.Year);
                    if (entity?.ID != default) item.ID = entity.ID;
                    if (await _schoolAchievementQuery.SaveAsync(item))
                    {
                        insertCount++;
                        SaveModifyLog(modifyLogs, entity.CompareObjFieldValue(item), item.EID, item.Year, "学部升学成绩");
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 指标分配
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadExtensionQuota()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();

            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<OnlineSchoolQuotaInfo>();
            foreach (var row in rows)
            {
                var entity = new OnlineSchoolQuotaInfo();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (Guid.TryParse(cell.CellValue, out Guid eid))
                            {
                                entity.EID = eid;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            if (int.TryParse(cell.CellValue, out int quotaType))
                            {
                                entity.Type = quotaType;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 2:
                            if (int.TryParse(cell.CellValue, out int year))
                            {
                                entity.Year = year;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 3:
                            if (int.TryParse(cell.CellValue, out int examineeQuantity))
                            {
                                entity.ExamineeQuantity = examineeQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 4:
                            if (int.TryParse(cell.CellValue, out int provinceCityQuantity))
                            {
                                entity.ProvinceCityQuantity = provinceCityQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 5:
                            if (int.TryParse(cell.CellValue, out int areaQuantity))
                            {
                                entity.AreaQuantity = areaQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 6:
                            entity.CoreSchool = cell.CellValue;
                            break;
                        case 7:
                            if (int.TryParse(cell.CellValue, out int coreQuantity))
                            {
                                entity.CoreQuantity = coreQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 8:
                            var schoolData = cell.CellValue.FromJsonSafe<IEnumerable<string[]>>();
                            if (schoolData?.Any(p => p?.Length == 3) == true)
                            {
                                entity.SchoolData = schoolData.Where(p => p?.Length == 3).ToJson();
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)) insertDatas.Add(entity);
            }

            #region 额外信息
            rows = file.OpenReadStream().ExcelToData(sheetIndex: 2);
            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new OnlineSchoolQuotaInfo();
                    var otherItems = new List<KeyValuePair<string, string>>();
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                        if (cell.CellIndex < 3)
                        {
                            switch (cell.CellIndex)
                            {
                                case 0:
                                    if (Guid.TryParse(cell.CellValue, out Guid eid))
                                    {
                                        entity.EID = eid;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                                case 1:
                                    if (int.TryParse(cell.CellValue, out int quotaType))
                                    {
                                        entity.Type = quotaType;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                                case 2:
                                    if (int.TryParse(cell.CellValue, out int year))
                                    {
                                        entity.Year = year;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                otherItems.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                            }
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex) && entity.EID != default && entity.Year > 1900 && otherItems.Any())
                    {
                        if (insertDatas.Any(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year))
                        {
                            var find = insertDatas.FirstOrDefault(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year);
                            find.ExtraItems = otherItems.ToJson();
                        }
                        else
                        {
                            entity.ExtraItems = otherItems.ToJson();
                            insertDatas.Add(entity);
                        }
                    }
                }
            }
            #endregion

            var insertCount = 0;
            if (insertDatas.Any())
            {
                //await _schoolQuotaQuery.RemoveByEIDsAsync(insertDatas.Select(p => p.EID).Distinct());
                var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(insertDatas.Select(p => p.EID).Distinct());
                foreach (var item in insertDatas)
                {
                    var entity = await _schoolQuotaQuery.GetAsync(item.EID, item.Year, item.Type);
                    if (entity?.ID != default) item.ID = entity.ID;
                    if (await _schoolQuotaQuery.SaveAsync(item))
                    {
                        insertCount++;
                        SaveModifyLog(modifyLogs, entity.CompareObjFieldValue(item), item.EID, item.Year, "指标分配");
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 招生信息
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadRecruits()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();

            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<OnlineSchoolRecruitInfo>();
            var extNickNames = new Dictionary<Guid, string>();
            foreach (var row in rows)
            {
                var entity = new OnlineSchoolRecruitInfo();
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (Guid.TryParse(cell.CellValue, out Guid eid))
                            {
                                entity.EID = eid;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            if (int.TryParse(cell.CellValue, out int recruitType))
                            {
                                entity.Type = recruitType;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 2:
                            if (int.TryParse(cell.CellValue, out int year))
                            {
                                entity.Year = year;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 3:
                            if (!extNickNames.ContainsKey(entity.EID)) extNickNames.Add(entity.EID, cell.CellValue);
                            break;
                        case 4:
                            entity.MinAge = cell.CellValue;
                            break;
                        case 5:
                            entity.MaxAge = cell.CellValue;
                            break;
                        case 6:
                            if (int.TryParse(cell.CellValue, out int quantity))
                            {
                                entity.Quantity = quantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 7:
                            if (int.TryParse(cell.CellValue, out int planQuantity))
                            {
                                entity.PlanQuantity = planQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 8:
                            if (int.TryParse(cell.CellValue, out int quotaPlanQuantity))
                            {
                                entity.QuotaPlanQuantity = quotaPlanQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 9:
                            entity.Target = cell.CellValue;
                            break;
                        case 10:
                            entity.Score = cell.CellValue;
                            break;
                        case 11:
                            entity.Scale = cell.CellValue;
                            break;
                        case 12:
                            entity.Brief = cell.CellValue;
                            break;
                        case 13:
                            entity.CounterpartScore = cell.CellValue;
                            break;
                        case 14:
                            entity.AllocationScore = cell.CellValue;
                            break;
                        case 15:
                            entity.IntegralImgUrl = cell.CellValue;
                            break;
                        case 16:
                            if (decimal.TryParse(cell.CellValue, out decimal integralPassLevel))
                            {
                                entity.IntegralPassLevel = integralPassLevel;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 17:
                            if (decimal.TryParse(cell.CellValue, out decimal integralAdmitLevel))
                            {
                                entity.IntegralAdmitLevel = integralAdmitLevel;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 18:
                            entity.ApplyCost = cell.CellValue;
                            break;
                        case 19:
                            entity.Tuition = cell.CellValue;
                            break;
                        case 20:
                            var otherCosts = cell.CellValue.FromJsonSafe<IEnumerable<string[]>>();
                            if (otherCosts?.Any() == true)
                            {
                                entity.OtherCost = cell.CellValue;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 21:
                            var requirements = cell.CellValue.FromJsonSafe<string[]>();
                            if (requirements?.Any() == true)
                            {
                                entity.Requirement = cell.CellValue;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 22:
                            if (int.TryParse(cell.CellValue, out int classQuantity))
                            {
                                entity.ClassQuantity = classQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 23:
                            entity.Material = cell.CellValue;
                            break;
                        case 24:
                            entity.SchoolScope = cell.CellValue;
                            break;
                        case 25:
                            if (int.TryParse(cell.CellValue, out int tzQuantity))
                            {
                                entity.TZQuantity = tzQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 26:
                            if (int.TryParse(cell.CellValue, out int tjQuantity))
                            {
                                entity.TJQuantity = tjQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 27:
                            entity.ProjectClassName = cell.CellValue;
                            break;
                        case 28:
                            if (int.TryParse(cell.CellValue, out int pzQuantity))
                            {
                                entity.PZQuantity = pzQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 29:
                            if (int.TryParse(cell.CellValue, out int lzQuantity))
                            {
                                entity.LZQuantity = lzQuantity;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 30:
                            if (decimal.TryParse(cell.CellValue, out decimal zkfskzx))
                            {
                                entity.ZKFSKZX = zkfskzx;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 31:
                            var allocationPrimaryEIDs = cell.CellValue.FromJsonSafe<Guid[]>();
                            if (allocationPrimaryEIDs?.Any() == true)
                            {
                                entity.AllocationPrimaryEIDs = cell.CellValue;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 32:
                            var counterpartPrimaryEIDs = cell.CellValue.FromJsonSafe<Guid[]>();
                            if (counterpartPrimaryEIDs?.Any() == true)
                            {
                                entity.CounterpartPrimaryEIDs = cell.CellValue;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                        case 33:
                            entity.ScribingScope = cell.CellValue;
                            break;
                        case 34:
                            var scribingScopeEIDs = cell.CellValue.FromJsonSafe<Guid[]>();
                            if (scribingScopeEIDs?.Any() == true)
                            {
                                entity.ScribingScopeEIDs = cell.CellValue;
                            }
                            else { errors.Add((0, row.RowIndex, cell.CellIndex)); }
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)) insertDatas.Add(entity);
            }

            #region 学部别称
            var nicknameCount = extNickNames.Count;
            var nicknameUpdateCount = 0;
            if (extNickNames?.Any() == true)
            {
                foreach (var dic_Nickname in extNickNames)
                {
                    var array_Nickname = dic_Nickname.Value?.FromJsonSafe<string[]>();
                    if (array_Nickname?.Any(p => !string.IsNullOrWhiteSpace(p)) == true)
                    {
                        nicknameUpdateCount += await _schoolExtensionQuery.UpdateExtNicknameAsync(dic_Nickname.Key, array_Nickname.Where(p => !string.IsNullOrWhiteSpace(p)).ToJson());
                    }
                }
            }
            #endregion

            #region 额外信息
            rows = file.OpenReadStream().ExcelToData(sheetIndex: 2);
            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new OnlineSchoolRecruitInfo();
                    var otherItems = new List<KeyValuePair<string, string>>();
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                        if (cell.CellIndex < 3)
                        {
                            switch (cell.CellIndex)
                            {
                                case 0:
                                    if (Guid.TryParse(cell.CellValue, out Guid eid))
                                    {
                                        entity.EID = eid;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                                case 1:
                                    if (int.TryParse(cell.CellValue, out int recruitType))
                                    {
                                        entity.Type = recruitType;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                                case 2:
                                    if (int.TryParse(cell.CellValue, out int year))
                                    {
                                        entity.Year = year;
                                    }
                                    else
                                    {
                                        errors.Add((2, row.RowIndex, cell.CellIndex));
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(cell.CellName) && !string.IsNullOrWhiteSpace(cell.CellValue))
                            {
                                otherItems.Add(new KeyValuePair<string, string>(cell.CellName, cell.CellValue));
                            }
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex) && entity.EID != default && entity.Year > 1900 && otherItems.Any())
                    {
                        if (insertDatas.Any(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year))
                        {
                            var find = insertDatas.FirstOrDefault(p => p.EID == entity.EID && p.Type == entity.Type && p.Year == entity.Year);
                            find.ExtraItems = otherItems.ToJson();
                        }
                        else
                        {
                            entity.ExtraItems = otherItems.ToJson();
                            insertDatas.Add(entity);
                        }
                    }
                }
            }
            #endregion

            var insertCount = 0;
            if (insertDatas.Any())
            {
                var eids = insertDatas.Select(p => p.EID).Distinct();
                var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(eids);
                foreach (var item in insertDatas)
                {
                    var entity = (await _schoolRecruitQuery.GetByEID(item.EID, item.Year, item.Type))?.FirstOrDefault();
                    if (entity != default) item.ID = entity.ID;
                    if (await _schoolRecruitQuery.SaveAsync(item))
                    {
                        insertCount++;
                        SaveModifyLog(modifyLogs, entity.CompareObjFieldValue(item), item.EID, item.Year, "招生信息");
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors, nicknameCount, nicknameUpdateCount });

            return result;
        }

        /// <summary>
        /// 上传 招生日程
        /// <para>done</para>
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadRecruitSchedule()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new List<RecruitScheduleInfo>();
            foreach (var row in rows)
            {
                var entity = new RecruitScheduleInfo()
                {
                    ID = Guid.NewGuid()
                };
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (int.TryParse(cell.CellValue, out int cityCode))
                            {
                                entity.CityCode = cityCode;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            entity.SchFType = cell.CellValue;
                            break;
                        case 2:
                            if (int.TryParse(cell.CellValue, out int areaCode))
                            {
                                entity.AreaCode = areaCode;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 3:
                            if (int.TryParse(cell.CellValue, out int recruitType))
                            {
                                entity.RecruitType = recruitType;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 4:
                            entity.StrDate = cell.CellValue.Trim('"');
                            break;
                        case 5:
                            entity.Content = cell.CellValue;
                            break;
                        case 6:
                            if (int.TryParse(cell.CellValue, out int index))
                            {
                                entity.Index = index;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 7:
                            if (int.TryParse(cell.CellValue, out int year))
                            {
                                entity.Year = year;
                            }
                            else
                            {
                                errors.Add((0, row.RowIndex, cell.CellIndex));
                            }
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)) insertDatas.Add(entity);
            }
            var insertCount = 0;
            if (insertDatas.Any())
            {
                await _schoolRecruitQuery.RemoveRecruitSchedulesAsync(insertDatas.Select(p => $"{p.AreaCode}{p.SchFType}{p.RecruitType}{p.CityCode}").Distinct());
                foreach (var item in insertDatas)
                {
                    if (await _schoolRecruitQuery.InsertRecruitScheduleAsync(item) > 0) insertCount++;
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 区域招生政策
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadAreaRecruitPlan()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];
            var sheetIndex = 0;
            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int ColIndex)>();
            var insertDatas = new List<AreaRecruitPlanInfo>();
            foreach (var row in rows)
            {
                var entity = new AreaRecruitPlanInfo()
                {
                    ID = Guid.NewGuid()
                };
                string articleTitle = null, articleUrl = null;
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            entity.SchFType = cell.CellValue;
                            break;
                        case 1:
                            entity.AreaCode = cell.CellValue;
                            break;
                        case 2:
                            if (int.TryParse(cell.CellValue, out int year)) entity.Year = year;
                            else errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            break;
                        case 3:
                            articleTitle = cell.CellValue;
                            break;
                        case 4:
                            articleUrl = cell.CellValue;
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)
                    || string.IsNullOrWhiteSpace(entity.SchFType)
                    || string.IsNullOrWhiteSpace(entity.AreaCode)
                    || string.IsNullOrWhiteSpace(articleUrl)
                    || string.IsNullOrWhiteSpace(articleTitle)
                    || entity.Year < 1900)
                {
                    if (insertDatas.Any(p => p.SchFType == entity.SchFType && p.Year == entity.Year && p.AreaCode == entity.AreaCode))
                    {
                        var find = insertDatas.FirstOrDefault(p => p.SchFType == entity.SchFType && p.Year == entity.Year && p.AreaCode == entity.AreaCode);
                        var list_UrlDatas = find.UrlData_Obj?.ToList() ?? new List<string[]>();
                        if (!list_UrlDatas.Contains(new string[] { articleTitle, articleUrl })) list_UrlDatas.Add(new string[] { articleTitle, articleUrl });
                        find.UrlData = list_UrlDatas.ToJson();
                    }
                    else
                    {
                        entity.UrlData = $"[{new string[] { articleTitle, articleUrl }.ToJson()}]";
                        insertDatas.Add(entity);
                    }
                }
            }


            //积分办法
            sheetIndex = 1;
            rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);

            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new AreaRecruitPlanInfo()
                    {
                        ID = Guid.NewGuid()
                    };
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 0:
                                entity.SchFType = cell.CellValue;
                                break;
                            case 1:
                                entity.AreaCode = cell.CellValue;
                                break;
                            case 2:
                                if (int.TryParse(cell.CellValue, out int year)) entity.Year = year;
                                else errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                break;
                            case 3:
                                entity.PointMethod = cell.CellValue;
                                break;
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex)
                        || string.IsNullOrWhiteSpace(entity.SchFType)
                        || string.IsNullOrWhiteSpace(entity.AreaCode)
                        || string.IsNullOrWhiteSpace(entity.PointMethod)
                        || entity.Year < 1900)
                    {
                        if (insertDatas.Any(p => p.SchFType == entity.SchFType && p.Year == entity.Year && p.AreaCode == entity.AreaCode))
                        {
                            var find = insertDatas.FirstOrDefault(p => p.SchFType == entity.SchFType && p.Year == entity.Year && p.AreaCode == entity.AreaCode);
                            find.PointMethod = entity.PointMethod;
                        }
                        else
                        {
                            insertDatas.Add(entity);
                        }
                    }
                }
            }

            var insertCount = 0;
            if (insertDatas.Any())
            {
                await _areaRecruitPlanQuery.RemoveByParamsAsync(insertDatas.Select(p => $"{p.AreaCode}{p.SchFType}{p.Year}"));
                foreach (var item in insertDatas)
                {
                    if (await _areaRecruitPlanQuery.InsertAsync(item)) insertCount++;
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 等级标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadExtLevel()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];
            var sheetIndex = 0;
            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int ColIndex)>();
            var insertDatas = new List<OnlineSchoolExtLevelInfo>();
            foreach (var row in rows)
            {
                var entity = new OnlineSchoolExtLevelInfo()
                {
                    ID = Guid.NewGuid()
                };
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (int.TryParse(cell.CellValue, out int cityCode))
                            {
                                entity.CityCode = cityCode;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            entity.SchFType = cell.CellValue;
                            break;
                        case 2:
                            entity.ReplaceSource = cell.CellValue;
                            break;
                        case 3:
                            entity.LevelName = cell.CellValue;
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex)
                    && !string.IsNullOrWhiteSpace(entity.SchFType)
                    && !string.IsNullOrWhiteSpace(entity.ReplaceSource)
                    && !string.IsNullOrWhiteSpace(entity.LevelName)
                    && entity.CityCode.HasValue)
                {
                    insertDatas.Add(entity);
                }
            }

            var insertCount = 0;
            if (insertDatas.Any())
            {
                await _schoolExtensionLevelQuery.RemoveByParamsAsync(insertDatas.Select(p => new KeyValuePair<int, string>(p.CityCode.Value, p.SchFType)));
                foreach (var item in insertDatas)
                {
                    if (await _schoolExtensionLevelQuery.InsertAsync(item)) insertCount++;
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }

        /// <summary>
        /// 上传 学部开设的课程
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseResult> UploadExtensionProject()
        {
            var result = ResponseResult.Failed();
            if (!CheckUploadPermission(ref result)) return result;

            var file = Request.Form.Files[0];
            var sheetIndex = 0;
            var rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);
            var insertCount = 0;
            var errors = new List<(int SheetIndex, int RowIndex, int ColIndex)>();
            var insertDatas = new List<OnlineSchoolProjectInfo>();
            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new OnlineSchoolProjectInfo();
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 1:
                                if (Guid.TryParse(cell.CellValue, out Guid eid))
                                {
                                    entity.EID = eid;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 2:
                                entity.Name = cell.CellValue;
                                break;
                            case 3:
                                var array_Item = $"[{cell.CellValue.Replace("“", "\"").Replace("”", "\"").Replace("，", ",")}]".FromJsonSafe<string[]>();
                                if (array_Item?.Any(p => !string.IsNullOrWhiteSpace(p)) == true)
                                {
                                    entity.ItemJson = array_Item.Where(p => !string.IsNullOrWhiteSpace(p)).ToJson();
                                }
                                break;
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex)) insertDatas.Add(entity);
                }

                if (insertDatas.Any())
                {
                    //await _schoolProjectQuery.RemoveByEIDsAsync(insertDatas.Select(p => p.EID));
                    var modifyLogs = await _dataModifyLogQuery.ListByEIDAsync(insertDatas.Select(p => p.EID).Distinct());
                    foreach (var item in insertDatas)
                    {
                        var entity = await _schoolProjectQuery.GetAsync(item.EID);
                        if (entity?.ID != default) item.ID = entity.ID;
                        if (await _schoolProjectQuery.SaveAsync(item))
                        {
                            insertCount++;
                            SaveModifyLog(modifyLogs, entity.CompareObjFieldValue(item), item.EID, headTitle: "学部开设的课程");
                        }
                    }

                }
            }

            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });

            return result;
        }
    }
}