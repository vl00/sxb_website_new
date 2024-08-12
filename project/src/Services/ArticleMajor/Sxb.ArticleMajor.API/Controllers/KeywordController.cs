using Microsoft.AspNetCore.Mvc;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Framework.AspNetCoreHelper.Utils;
using Sxb.Framework.Foundation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class KeywordController : Controller
    {
        readonly IKeywordQuery _keywordQuery;
        public KeywordController(IKeywordQuery keywordQuery)
        {
            _keywordQuery = keywordQuery;
        }

        /// <summary>
        /// 获取关键字
        /// </summary>
        /// <param name="siteType">网站类型
        /// <para>1.幼儿教育</para>
        /// <para>2.小学教育</para>
        /// <para>3.中学教育</para>
        /// <para>4.中职教育</para>
        /// <para>5.高中教育</para>
        /// <para>6.素质教育</para>
        /// <para>7.国际教育</para>
        /// </param>
        /// <param name="cityCode">城市代码</param>
        /// <param name="pageType">页面类型
        /// <para>1.首页</para>
        /// <para>2.二级分类</para>
        /// <para>3.三级分类</para>
        /// <para>4.内容聚类</para>
        /// </param>
        /// <param name="positionType">位置
        /// <para>1.关键词导航栏</para>
        /// <para>2.时事热点</para>
        /// <para>3.关键词右侧栏</para>
        /// </param>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public async Task<ResponseResult> List(KeywordSiteType siteType, int cityCode, KeywordPageType pageType, KeywordPositionType? positionType)
        {
            var result = ResponseResult.DefaultFailed();
            if (siteType < KeywordSiteType.Unknow || pageType < KeywordPageType.Top) return result;
            if (cityCode < 100000) cityCode = Request.GetCity("440100");
            var finds = await _keywordQuery.ListAsync(siteType, cityCode, pageType, positionType);
            if (finds?.Any() == true)
            {
                var resultData = new List<KeyValuePair<int, dynamic>>();
                foreach (var find in finds)
                {
                    object items = null;
                    switch (find.PositionType)
                    {
                        case KeywordPositionType.Position1:
                            items = find.DataJson.FromJsonSafe<IEnumerable<KeyValuePair<string, IEnumerable<KeyValuePair<string, IEnumerable<Nav>>>>>>();
                            if (items != null) resultData.Add(new KeyValuePair<int, dynamic>(1, items));
                            break;
                        case KeywordPositionType.Position2:
                            items = find.DataJson.FromJsonSafe<IEnumerable<NavEx>>();
                            if (items != null) resultData.Add(new KeyValuePair<int, dynamic>(2, items));
                            break;
                        case KeywordPositionType.Position3:
                            items = find.DataJson.FromJsonSafe<IEnumerable<Nav>>();
                            if (items != null) resultData.Add(new KeyValuePair<int, dynamic>(3, items));
                            break;
                    }
                }
                if (resultData.Any()) result = ResponseResult.Success(resultData);
            }
            return result;
        }

        [HttpPost]
        [Route("UploadPosition1")]
        public ResponseResult UploadPosition1()
        {
            var result = ResponseResult.DefaultFailed();
            var file = Request.Form.Files[0];

            var rows = file.OpenReadStream().ExcelToData();


            var errors = new List<(int SheetIndex, int RowIndex, int CellIndex)>();
            var tagNames = new List<string>();
            var groupNames = new List<(string TagName, string GroupName)>();
            var keywords = new List<(string TagName, string GourpName, string Name, string Url)>();


            var currentTagName = string.Empty;
            var currentGroupName = string.Empty;
            var name = string.Empty;
            foreach (var row in rows)
            {
                foreach (var cell in row.Cells.Where(p => !string.IsNullOrWhiteSpace(p.CellValue)).OrderBy(p => p.CellIndex))
                {
                    if (errors.Any(p => p.Item1 == row.RowIndex)) break;
                    switch (cell.CellIndex)
                    {
                        case 0:
                            if (!tagNames.Contains(cell.CellValue.Trim()))
                            {
                                tagNames.Add(cell.CellValue);
                                currentTagName = cell.CellValue;
                            }
                            break;
                        case 1:
                            if (!groupNames.Contains((currentTagName, cell.CellValue.Trim())))
                            {
                                groupNames.Add((currentTagName, cell.CellValue));
                                currentGroupName = cell.CellValue;
                            }
                            break;
                        case 3:
                            name = cell.CellValue;
                            break;
                        case 4:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 5:
                            name = cell.CellValue;
                            break;
                        case 6:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 7:
                            name = cell.CellValue;
                            break;
                        case 8:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 9:
                            name = cell.CellValue;
                            break;
                        case 10:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 11:
                            name = cell.CellValue;
                            break;
                        case 12:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 13:
                            name = cell.CellValue;
                            break;
                        case 14:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 15:
                            name = cell.CellValue;
                            break;
                        case 16:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 17:
                            name = cell.CellValue;
                            break;
                        case 18:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 19:
                            name = cell.CellValue;
                            break;
                        case 20:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                        case 21:
                            name = cell.CellValue;
                            break;
                        case 22:
                            keywords.Add((currentTagName, currentGroupName, name, cell.CellValue));
                            break;
                    }
                }
            }

            var obj_Tags = new List<KeyValuePair<string, dynamic>>();
            foreach (var tagName in tagNames)
            {
                var obj_Groups = new List<KeyValuePair<string, dynamic>>();
                foreach (var groupName in groupNames)
                {
                    if (groupName.TagName != tagName) continue;
                    var obj_Keywords = new List<dynamic>();
                    foreach (var keyword in keywords)
                    {
                        if (keyword.GourpName != groupName.GroupName || keyword.TagName != tagName) continue;
                        obj_Keywords.Add(new
                        {
                            keyword.Name,
                            keyword.Url
                        });
                    }
                    obj_Groups.Add(new KeyValuePair<string, dynamic>(groupName.GroupName, obj_Keywords));
                }
                obj_Tags.Add(new KeyValuePair<string, dynamic>(tagName, obj_Groups));
            }
            System.Console.WriteLine(obj_Tags.ToJson());
            result = ResponseResult.Success(obj_Tags);
            return result;
        }
    }
}
