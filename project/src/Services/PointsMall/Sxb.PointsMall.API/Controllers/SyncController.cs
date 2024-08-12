using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sxb.Framework.AspNetCoreHelper;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.PointsMall.API.Application.Commands;
using Sxb.PointsMall.API.Application.Queries.AccountPoints;
using Sxb.PointsMall.API.Application.Queries.UserPointsTask;
using Sxb.PointsMall.API.Application.Queries.UserSignInInfo;
using Sxb.PointsMall.API.Models;
using Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate;
using Sxb.PointsMall.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using System.ComponentModel;
using Sxb.PointsMall.API.Config;
using Sxb.Framework.Cache.Redis;
using Microsoft.AspNetCore.Mvc.Filters;
using Sxb.PointsMall.API.Infrastructure.Filter;
using Sxb.PointsMall.Domain.Events;
using Sxb.PointsMall.API.Application.IntegrationEvents;
using DotNetCore.CAP;
using Sxb.PointsMall.API.Infrastructure.Services;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace Sxb.PointsMall.API.Controllers
{

    /// <summary>
    /// 同步中心
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ICapPublisher _capPublisher;
        private readonly IConfiguration _configuration;

        public SyncController(ICapPublisher capPublisher, IConfiguration configuration)
        {
            _capPublisher = capPublisher;
            _configuration = configuration;
        }

        public IDbConnection Db
        {
            get
            {
                string constr = _configuration.GetConnectionString("iSchoolPointsMall.Read");
                return new SqlConnection(constr);
            }
        }

        /// <summary>
        /// 把已添加的孩子旧数据, 自动完成运营孩子任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("Org/ChildInfo")]
        public async Task<ResponseResult> SyncOrgChildInfoAsync()
        {
            var sql = @"
-- 已添加孩子, 未完成任务的用户
SELECT
	UserId,
	MIN(CreateTime) AS CreateTime
FROM
	Organization.dbo.ChildArchives
WHERE
	IsValid = 1
	AND UserId NOT IN (
		SELECT UserId FROM ISchoolPointsMall.dbo.UserPointsTasks
		WHERE IsValid = 1 AND Status > 0 AND PointsTaskId = 6
	)
GROUP BY
	UserId
";
            var evts = await Db.QueryAsync<AddChildIntegrationEvent>(sql);
            if (evts.Any())
            {
                foreach (var evt in evts)
                {
                    evt.Id = Guid.Empty;
                    evt.Name = "system sync";
                    evt.CreateTime = DateTime.Now;
                    await _capPublisher.PublishAsync(nameof(AddChildIntegrationEvent), evt);
                }
                return ResponseResult.Success("同步完成");
            }
            return ResponseResult.Failed("已全部同步, 无需再次同步");
        }
    }
}
