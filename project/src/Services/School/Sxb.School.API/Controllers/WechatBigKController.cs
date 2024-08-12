using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.Cache.Redis;
using Sxb.Framework.Foundation;
using Sxb.School.API.Application.Commands;
using Sxb.School.API.Application.Query;
using Sxb.School.API.RequestContact.WechatBigK;
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
    public class WechatBigKController : ControllerBase
    {
        readonly IEasyRedisClient _easyRedisClient;
        readonly IMediator _mediator;
        readonly IWechatBigKQuery _wechatBigKQuery;
        readonly IUserQuery _userQuery;
        public WechatBigKController(IMediator mediator, IEasyRedisClient easyRedisClient, IWechatBigKQuery wechatBigKQuery, IUserQuery userQuery)
        {
            _wechatBigKQuery = wechatBigKQuery;
            _mediator = mediator;
            _easyRedisClient = easyRedisClient;
            _userQuery = userQuery;
        }

        [HttpGet]
        public async Task<ResponseResult> Get([FromQuery] GetRequest request)
        {
            var result = ResponseResult.DefaultFailed();
            if (request == null || request.AreaCode < 1 || request.Type < WeChatRecruitType.Policy) return result;
            var items = await _wechatBigKQuery.GetAsync(request.AreaCode, request.Type, request.Grade, request.Year);
            if (items?.Any() == true)
            {
                var relations = await _wechatBigKQuery.GetRalations(items.Select(p => p.ID));
                if (relations?.Any() == true)
                {
                    var resultDatas = new List<GetResponse>();

                    foreach (var item in items)
                    {
                        var item_Relations = relations.Where(p => p.RecruitID == item.ID);
                        if (item_Relations?.Any() == true)
                        {
                            var resultData = new GetResponse()
                            {
                                Year = item.Year,
                                SubTitle = item.SubTitle,
                                Title = item.Title,
                                Type = item.Type,
                                CityCode = item.CityCode,
                                AreaCode = item.AreaCode
                            };
                            var relationItems = new List<(string TypeName, dynamic Items)>();
                            foreach (var item_RelationType in item_Relations.Select(p => p.Type).Distinct())
                            {
                                switch (item_RelationType)
                                {
                                    case WeChatRecruitItemType.Article:
                                        relationItems.Add(("Article",
                                            (await _wechatBigKQuery.GetArticles(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Article).Select(p => p.ItemID)))
                                            ?.OrderBy(p => Array.IndexOf(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Article).OrderBy(x => x.Index).Select(x => x.ItemID).ToArray(), p.ID))
                                            ));
                                        break;
                                    case WeChatRecruitItemType.Attachment:
                                        relationItems.Add(("Attachment",
                                            (await _wechatBigKQuery.GetAttachments(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Attachment).Select(p => p.ItemID)))
                                            ?.OrderBy(p => Array.IndexOf(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Attachment).OrderBy(x => x.Index).Select(x => x.ItemID).ToArray(), p.ID))
                                            ));
                                        break;
                                    case WeChatRecruitItemType.Schedule:
                                        relationItems.Add(("Schedule",
                                            (await _wechatBigKQuery.GetSchedules(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Schedule).Select(p => p.ItemID)))
                                            ?.OrderBy(p => Array.IndexOf(item_Relations.Where(p => p.Type == WeChatRecruitItemType.Schedule).OrderBy(x => x.Index).Select(x => x.ItemID).ToArray(), p.ID))
                                            ));
                                        break;
                                }
                            }
                            if (relationItems?.Any() == true)
                            {
                                resultData.Items = relationItems.Select(p => new
                                {
                                    p.TypeName,
                                    p.Items
                                });
                                resultDatas.Add(resultData);
                                resultData.Years = await _wechatBigKQuery.GetYearsAsync(item.AreaCode, item.Type, item.Grade);
                            }
                        }
                    }
                    if (resultDatas.Any())
                    {
                        result = ResponseResult.Success(resultDatas);
                    }
                }
            }
            return result;
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

        [HttpPost]
        public async Task<ResponseResult> UploadData()
        {
            var result = ResponseResult.DefaultFailed();
            if (!CheckUploadPermission(ref result)) return result;

            if (!Request.Form.Files.Any(p => p.Name == "uploadFile")) return result;
            var file = Request.Form.Files.GetFile("uploadFile");

            var sheetIndex = 0;
            var rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);

            if (rows.Count == 0) return result;

            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var insertDatas = new Dictionary<int, WeChatRecruitInfo>();
            var insertDatas_Relation = new List<WeChatRecruitItemInfo>();

            #region 主表
            foreach (var row in rows)
            {
                var entity = new WeChatRecruitInfo()
                {
                    ID = Guid.NewGuid()
                };
                var entityIndex = 0;
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (!int.TryParse(cell.CellValue, out entityIndex))
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 1:
                            if (Enum.TryParse(cell.CellValue, out WeChatRecruitType recruitType))
                            {
                                entity.Type = recruitType;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 2:
                            entity.Title = cell.CellValue;
                            break;
                        case 3:
                            entity.SubTitle = cell.CellValue;
                            break;
                        case 4:
                            if (int.TryParse(cell.CellValue, out int year))
                            {
                                entity.Year = year;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 5:
                            if (Enum.TryParse(cell.CellValue, out SchoolGradeType gradeType))
                            {
                                entity.Grade = gradeType;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 6:
                            if (int.TryParse(cell.CellValue, out int cityCode))
                            {
                                entity.CityCode = cityCode;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                        case 7:
                            if (int.TryParse(cell.CellValue, out int areaCode))
                            {
                                entity.AreaCode = areaCode;
                            }
                            else
                            {
                                errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                            }
                            break;
                    }
                }
                if (!errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)
                && entityIndex > 0
                && !insertDatas.ContainsKey(entityIndex)
                && entity.AreaCode > 0
                && entity.CityCode > 0
                && entity.Year > 0
                && !string.IsNullOrWhiteSpace(entity.Title)
                && entity.Type > 0) insertDatas.Add(entityIndex, entity);
            }
            #endregion

            #region 文章
            sheetIndex = 2;
            rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);
            var insertData_Article = new List<(int MainID, WeChatRecruitArticleInfo Entity)>();
            if (rows?.Any() == true)
            {
                var dic_TxtFile = new Dictionary<string, string>();
                if (Request.Form.Files.Any(p => p.Name == "zipFile"))
                {
                    var zipFileStream = Request.Form.Files.GetFile("zipFile").OpenReadStream();
                    dic_TxtFile = ZipHelper.UnZip(zipFileStream);
                }

                foreach (var row in rows)
                {
                    var entity = new WeChatRecruitArticleInfo()
                    {
                        ID = Guid.NewGuid()
                    };
                    var relation = new WeChatRecruitItemInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = WeChatRecruitItemType.Article,
                        ItemID = entity.ID
                    };
                    var entityIndex = 0;
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (int.TryParse(cell.CellValue, out entityIndex) && insertDatas.ContainsKey(entityIndex))
                                {
                                    relation.RecruitID = insertDatas[entityIndex].ID;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                entity.Title = cell.CellValue;
                                break;
                            case 2:
                                var subTitltes = $"[{cell.CellValue}]".FromJsonSafe<IEnumerable<string[]>>();
                                if (subTitltes?.Any(p => p != default && p.Length == 2 && p.First() != default) == true)
                                {
                                    var list_SubTitles = new List<WeChatRecruitArticleSubTitleItem>();
                                    foreach (var item in subTitltes.Where(p => p.Length == 2))
                                    {
                                        if (Enum.TryParse(item.Last(), out SubTitleTextAlignType subTitleTextAlignType) && !string.IsNullOrWhiteSpace(item.First()))
                                        {
                                            list_SubTitles.Add(new WeChatRecruitArticleSubTitleItem()
                                            {
                                                Align = subTitleTextAlignType,
                                                Content = item.First()
                                            });
                                        }
                                    }
                                    if (list_SubTitles.Any()) entity.SubTitle = list_SubTitles.ToJson();
                                }
                                break;
                            case 3:
                                entity.SourceText = cell.CellValue;
                                break;
                            case 4:
                                entity.SourceUrl = cell.CellValue;
                                break;
                            case 5:
                                if (int.TryParse(cell.CellValue, out int index))
                                {
                                    relation.Index = index;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 6:
                                if (cell.CellValue?.ToLower() == "##file##")
                                {
                                    if (dic_TxtFile.ContainsKey(entity.Title))
                                    {
                                        if (System.IO.File.Exists(dic_TxtFile[entity.Title])) entity.Content = System.IO.File.ReadAllText(dic_TxtFile[entity.Title]);
                                    }
                                }
                                else
                                {
                                    entity.Content = cell.CellValue;
                                }
                                break;
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex))
                    {
                        if (!string.IsNullOrWhiteSpace(entity.Content) && entity.SubTitle?.Any() == true)
                        {
                            var existSubTitles = new List<string>();
                            foreach (var subTitle in entity.SubTitle_Obj)
                            {
                                if (entity.Content.Contains($"##{subTitle.Content}##"))
                                {
                                    existSubTitles.Add(subTitle.Content);
                                    var textAlign = "left";
                                    switch (subTitle.Align)
                                    {
                                        case SubTitleTextAlignType.Center:
                                            textAlign = "center";
                                            break;
                                        case SubTitleTextAlignType.Right:
                                            textAlign = "right";
                                            break;
                                    }
                                    entity.Content = entity.Content.Replace("##" + subTitle.Content + "##", $"<div style=\"text-align:{textAlign};\" class=\"subTitle\" data-id=\"{subTitle.ID}\">{subTitle.Content}</div>");
                                }
                            }
                            if (existSubTitles.Any())
                            {
                                entity.SubTitle = entity.SubTitle_Obj.Where(p => existSubTitles.Contains(p.Content)).ToJson();
                            }
                            else
                            {
                                entity.SubTitle = default;
                            }
                        }
                        insertData_Article.Add((entityIndex, entity));
                        insertDatas_Relation.Add(relation);
                    }
                }

                if (dic_TxtFile.Any())
                {
                    var dir_Path = new System.IO.FileInfo(dic_TxtFile.FirstOrDefault().Value).DirectoryName;
                    try
                    {
                        System.IO.Directory.Delete(dir_Path, true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
            #endregion

            #region 附件
            sheetIndex = 3;
            rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);
            var insertData_Attachment = new List<(int MainID, WeChatRecruitAttachmentInfo Entity)>();
            if (rows?.Any() == true)
            {
                foreach (var row in rows)
                {
                    var entity = new WeChatRecruitAttachmentInfo()
                    {
                        ID = Guid.NewGuid()
                    };
                    var relation = new WeChatRecruitItemInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = WeChatRecruitItemType.Attachment,
                        ItemID = entity.ID
                    };
                    var entityIndex = 0;
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (int.TryParse(cell.CellValue, out entityIndex) && insertDatas.ContainsKey(entityIndex))
                                {
                                    relation.RecruitID = insertDatas[entityIndex].ID;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                entity.Title = cell.CellValue;
                                break;
                            case 2:
                                entity.Url = cell.CellValue;
                                break;
                            case 3:
                                if (int.TryParse(cell.CellValue, out int index))
                                {
                                    relation.Index = index;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                        }
                    }
                    if (!errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex))
                    {
                        insertData_Attachment.Add((entityIndex, entity));
                        insertDatas_Relation.Add(relation);
                    }
                }
            }
            #endregion

            #region 日程
            sheetIndex = 4;
            rows = file.OpenReadStream().ExcelToData(sheetIndex: sheetIndex);
            var insertData_Schedule = new List<(int MainID, WeChatRecruitScheduleInfo Entity)>();
            var schedules = new List<(int MainID, WeChatRecruitScheduleItem Entity)>();
            if (rows?.Any() == true)
            {
                var mainID = -1;
                foreach (var row in rows)
                {
                    var entity = new WeChatRecruitScheduleItem();
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (!int.TryParse(cell.CellValue, out mainID)) errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                break;
                            case 3:
                                if (int.TryParse(cell.CellValue, out int entityIndex))
                                {
                                    entity.Index = entityIndex;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 4:
                                if (DateTime.TryParse(cell.CellValue, out DateTime startDate))
                                {
                                    entity.StartDate = startDate;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 5:
                                if (DateTime.TryParse(cell.CellValue, out DateTime endDate))
                                {
                                    entity.EndDate = endDate;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 6:
                                entity.Content = cell.CellValue;
                                break;
                        }
                    }
                    if (entity.Index == 0 || entity.StartDate == DateTime.MinValue || entity.EndDate == DateTime.MinValue || string.IsNullOrWhiteSpace(entity.Content)) continue;
                    schedules.Add((mainID, entity));
                }

                foreach (var row in rows.Where(p => p.Cells.Any(x => x.CellIndex == 0 && !string.IsNullOrWhiteSpace(x.CellValue))))
                {
                    var entity = new WeChatRecruitScheduleInfo()
                    {
                        ID = Guid.NewGuid()
                    };
                    var relation = new WeChatRecruitItemInfo()
                    {
                        ID = Guid.NewGuid(),
                        Type = WeChatRecruitItemType.Schedule,
                        ItemID = entity.ID
                    };
                    var entityIndex = 0;
                    foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                    {
                        if (errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex)) break;
                        switch (cell.CellIndex)
                        {
                            case 0:
                                if (int.TryParse(cell.CellValue, out entityIndex) && insertDatas.ContainsKey(entityIndex))
                                {
                                    relation.RecruitID = insertDatas[entityIndex].ID;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 1:
                                entity.Title = cell.CellValue;
                                break;
                            case 2:
                                var subTitltes = $"[{cell.CellValue}]".FromJsonSafe<IEnumerable<string[]>>();
                                if (subTitltes?.Any(p => p != default && p.Length == 2 && p.First() != default) == true)
                                {
                                    var list_SubTitles = new List<WeChatRecruitArticleSubTitleItem>();
                                    foreach (var item in subTitltes.Where(p => p.Length == 2))
                                    {
                                        if (Enum.TryParse(item.Last(), out SubTitleTextAlignType subTitleTextAlignType) && !string.IsNullOrWhiteSpace(item.First()))
                                        {
                                            list_SubTitles.Add(new WeChatRecruitArticleSubTitleItem()
                                            {
                                                Align = subTitleTextAlignType,
                                                Content = item.First()
                                            });
                                        }
                                    }
                                    if (list_SubTitles.Any()) entity.SubTitle = list_SubTitles.ToJson();
                                }
                                break;
                            case 3:
                                if (int.TryParse(cell.CellValue, out int index))
                                {
                                    relation.Index = index;
                                }
                                else
                                {
                                    errors.Add((sheetIndex, row.RowIndex, cell.CellIndex));
                                }
                                break;
                            case 7:
                                entity.SourceText = cell.CellValue;
                                break;
                            case 8:
                                entity.SourceUrl = cell.CellValue;
                                break;
                            case 9:
                                entity.Content = cell.CellValue;
                                break;
                        }
                    }

                    if (!errors.Any(p => p.RowIndex == row.RowIndex && p.SheetIndex == sheetIndex))
                    {
                        if (schedules.Any(p => p.MainID == entityIndex && p.Entity.Index == relation.Index))
                        {
                            var index = 0;
                            var entitySchedules = schedules.Where(p => p.MainID == entityIndex && p.Entity.Index == relation.Index).Select(p => p.Entity);
                            entity.DataJson = entitySchedules.OrderBy(p => p.StartDate).Select(p => new WeChatRecruitScheduleItem()
                            {
                                Content = p.Content,
                                EndDate = p.EndDate,
                                StartDate = p.StartDate,
                                Index = ++index
                            }).ToJson();
                            entity.MinDate = entitySchedules.Min(p => p.StartDate);
                            entity.MaxDate = entitySchedules.Max(p => p.EndDate);
                        }
                        if (!string.IsNullOrWhiteSpace(entity.Content) && entity.SubTitle?.Any() == true)
                        {
                            foreach (var subTitle in entity.SubTitle_Obj)
                            {
                                var textAlign = "left";
                                switch (subTitle.Align)
                                {
                                    case SubTitleTextAlignType.Center:
                                        textAlign = "center";
                                        break;
                                    case SubTitleTextAlignType.Right:
                                        textAlign = "right";
                                        break;
                                }
                                entity.Content = entity.Content.Replace("##" + subTitle.Content + "##", $"<div style=\"text-align:{textAlign};\" class=\"subTitle\" data-id=\"{subTitle.ID}\">{subTitle.Content}</div>");
                            }
                        }
                        insertData_Schedule.Add((entityIndex, entity));
                        insertDatas_Relation.Add(relation);
                    }
                }
            }
            #endregion

            var insertCount = 0;
            if (insertDatas.Any())
            {
                await _wechatBigKQuery.RemoveManyAsync(insertDatas.Values);

                foreach (var item in insertDatas)
                {
                    if (await _wechatBigKQuery.InsertAsync(item.Value))
                    {
                        insertCount++;

                        foreach (var relation in insertDatas_Relation.Where(p => p.RecruitID == item.Value.ID))
                        {
                            if (await _wechatBigKQuery.InsertAsync(relation))
                            {
                                switch (relation.Type)
                                {
                                    case WeChatRecruitItemType.Article:
                                        var article = insertData_Article.FirstOrDefault(p => p.Entity.ID == relation.ItemID).Entity;
                                        if (article?.ID != default) await _wechatBigKQuery.InsertAsync(article);
                                        break;
                                    case WeChatRecruitItemType.Attachment:
                                        var attachment = insertData_Attachment.FirstOrDefault(p => p.Entity.ID == relation.ItemID).Entity;
                                        if (attachment?.ID != default) await _wechatBigKQuery.InsertAsync(attachment);
                                        break;
                                    case WeChatRecruitItemType.Schedule:
                                        var schedule = insertData_Schedule.FirstOrDefault(p => p.Entity.ID == relation.ItemID).Entity;
                                        if (schedule?.ID != default) await _wechatBigKQuery.InsertAsync(schedule);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (insertCount > 0 || errors.Any()) result = ResponseResult.Success(new { dataCount = insertDatas.Count, insertCount, errors });
            return result;
        }


        /// <summary>
        /// 每天晚上10点,轮询发送提醒
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public Task<ResponseResult> SendReminds()
        {
            return _mediator.Send(new WechatBigKRemindCommand());
        }

        /// <summary>
        /// 查询是否订阅政策大卡提醒
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ResponseResult> CheckIsSubscribe(Guid subjectId)
        {
            var groupCode = "BigK";
            var userId = User.Identity.GetID();

            var isSubscribe = userId != Guid.Empty 
                && await _userQuery.CheckIsSubscribe(groupCode, subjectId, userId);

            return ResponseResult.Success(new
            {
                IsSubscribe = isSubscribe
            });
        }
    }
}
