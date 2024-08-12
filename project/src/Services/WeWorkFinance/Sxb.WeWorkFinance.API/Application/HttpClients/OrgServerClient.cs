using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sxb.WeWorkFinance.API.Application.Models;
using Newtonsoft.Json;
using System.Text;

namespace Sxb.WeWorkFinance.API.Application.HttpClients
{
    public class OrgServerClient
    {
        private readonly HttpClient _httpClient;
        public OrgServerClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// 获取邀新活动的商品课程
        /// api/mini/Courses/rwInviteActivity/courses
        /// </summary>
        /// <returns></returns>
        public async Task<List<InviteActivityCourseModel>> GetInviteActivityCourses(string cityCode)
        {

            string url = "/api/mini/Courses/rwInviteActivity/courses" + (string.IsNullOrWhiteSpace(cityCode) ? "" : "?city=" + cityCode);

            var resp = await _httpClient.GetAsync(url);

            if(resp.StatusCode == System.Net.HttpStatusCode.OK )
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OrgResultModel<InviteActivityCourseResultModel>>(resultString);
                if (result.Succeed)
                {
                    return result.Data.Courses;
                }
            }
            return new List<InviteActivityCourseModel>();
        }


        /// <summary>
        /// 获取邀新活动的用户订单
        /// api/mini/Courses/rwInviteActivity/courses
        /// </summary>
        /// <returns></returns>
        public async Task<List<InviteActivityOrderResultModel>> GetInviteActivityOrder(Guid userId,DateTime startTime,DateTime endTime)
        {

            string url = "/api/Orders/rw/ls/user/storestatus";

            var request = new
            {
                userId,
                startTime,
                endTime
            };
            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var resp = await _httpClient.PostAsync(url, content);

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resultString = await resp.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<OrgResultModel<List<InviteActivityOrderResultModel>>>(resultString);
                if (result.Succeed)
                {
                    return result.Data;
                }
            }
            return new List<InviteActivityOrderResultModel>();
        }
    }
}
