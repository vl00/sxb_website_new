using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using Sxb.Recommend.API.Models;
using Sxb.Recommend.Application.Services;
using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Recommend.API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class MapFeatureController : ControllerBase
    {
        IMapFeatureService _mapFeatureService;
        public MapFeatureController(IMapFeatureService mapFeatureService)
        {
            _mapFeatureService = mapFeatureService;
        }
        [HttpGet]
        [Description("获取推荐匹配的特征信息详细")]
        public async Task<ResponseResult> Get(int type = 1)
        {
            var features = await _mapFeatureService.GetAsync(type);
            return ResponseResult.Success(features);
        }

        [HttpPost]
        public async Task<ResponseResult> Edit(MapFetureViewModel model)
        {
            var mapFeature = await _mapFeatureService.GetAsync(model.Id);
            mapFeature.Update(model.Score, model.Weight, model.Alias);
            var flag = await _mapFeatureService.Update(mapFeature);
            return ResponseResult.Success(flag,"ok");
        }
    }
}
