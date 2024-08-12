using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Static.API.Application.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Sxb.Static.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly static object _lock = new object();

        ILogsQueries _logsQueries;
        public LogsController(ILogsQueries logsQueries)
        {
            _logsQueries = logsQueries;
        }

        [HttpGet]
        [Route("/{city}/{day}/{type}.json")]
        public IActionResult GetFromExpireDay(int city, int day, string type)
        {
            string[] types = new[] { "school", "article", "schoolrank", "topic", "talent", "comment", "question", "course", "evaluation", "live" };
            int typeIndex = Array.IndexOf(types, type.ToLower());
            if (typeIndex > -1)
            {
                type = types[typeIndex];
            }
            else
            {
                return NotFound();
            }
            var today = DateTime.Now.Date;
            string directory = Path.Combine("Data", type);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string filePath = Path.Combine(directory, $"{today.ToString("yy-MM-dd")}_{city}_{day}.json");
            if (!System.IO.File.Exists(filePath))
            {
                lock (_lock)
                {
                    IEnumerable<LogData> logDatas = Enumerable.Empty<LogData>();
                    if (!System.IO.File.Exists(filePath))
                    {
                        switch (type)
                        {
                            case "school":
                                logDatas = _logsQueries.QuerySchoolLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "article":
                                logDatas = _logsQueries.QueryArticleLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "schoolrank":
                                logDatas = _logsQueries.QuerySchoolRankLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "topic":
                                logDatas = _logsQueries.QueryTopicLogDatasAsync(today.AddDays(day * -1), today).Result;
                                break;
                            case "talent":
                                logDatas = _logsQueries.QueryTalentLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "comment":
                                logDatas = _logsQueries.QueryCommentLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "question":
                                logDatas = _logsQueries.QueryQuestionLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                            case "course":
                                logDatas = _logsQueries.QueryCourseLogDatasAsync(2, today.AddDays(day * -1), today).Result;
                                break;
                            case "evaluation":
                                logDatas = _logsQueries.QueryEvaluationLogDatasAsync(today.AddDays(day * -1), today).Result;
                                break;
                            case "live":
                                logDatas = _logsQueries.QueryLiveLogDatasAsync(city, today.AddDays(day * -1), today).Result;
                                break;
                        }
                        var json = JsonConvert.SerializeObject(logDatas);
                        System.IO.File.WriteAllText(filePath, json, Encoding.UTF8);
                    }
                }
            }
            var buffer = System.IO.File.ReadAllBytes(filePath);
            return File(buffer, "application/json");

        }
    }
}
