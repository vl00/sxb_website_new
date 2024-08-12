using Sxb.School.Domain.AggregateModels.ViewOrder;
using System;

namespace Sxb.School.API.Application.Queries.SchoolViewOrder
{
    public class ViewOrderStateSummary
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public ViewOrderState State { get; set; }
    }

    public class OrderDetail {
        public Guid Id { get; set; }
        public string Num { get; set; }
        public Guid? UserId { get; set; }

        public ViewOrderPayWay? PayWay { get; set; }

        public decimal Amount { get; set; }

        public decimal? Payment { get; set; }

        public ViewOrderState Status { get; set; }
        public string Remark { get; set; }

        public string GoodsInfo { get; set; }

        public DateTime ExpireTime { get; set; }

        public ViewSchoolGoodsInfo GetViewSchoolGoodsInfo()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ViewSchoolGoodsInfo>(this.GoodsInfo);
        }

    }

    public class SchoolInfo {

        public Guid Id { get; set; }

        public Guid EID { get; set; }

        public string Name { get; set; }
    }


    public class UserInfo
    {

        public Guid Id { get; set; }

        public string NickName { get; set; }
    }

}
