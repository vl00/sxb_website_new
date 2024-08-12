using Sxb.Domain;
using Sxb.School.Domain.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.School.Domain.AggregateModels.ViewOrder
{

    public class SchoolViewOrder : Entity<Guid>, IAggregateRoot
    {
        public string Num { get; private set; }

        public Guid? UserId { get; private set; }

        public ViewOrderPayWay? PayWay { get; private set; }

        public decimal Amount { get; private set; }

        public decimal? Payment { get; private set; }
        public ViewOrderState Status { get; private set; }

        public string Remark { get; private set; }

        public string GoodsInfo { get; private set; }

        public DateTime CreateTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public DateTime ExpireTime { get; private set; }

        public void SetRemark(string remark)
        {
            this.Remark = remark;
            this.UpdateTime = DateTime.Now;
        }
        public void SetAmount(decimal amount) {
            if (this.Status != ViewOrderState.WaitPay)
                throw new Exception("当前订单状态不能修改金额。");
            this.Amount = amount;
            this.UpdateTime = DateTime.Now;
        }
        public void SetGoodsInfo(ViewSchoolGoodsInfo goodsInfo)
        {
            this.Amount = goodsInfo.Price;
            this.GoodsInfo = Newtonsoft.Json.JsonConvert.SerializeObject(goodsInfo);
            this.UpdateTime = DateTime.Now;
        }

        public ViewSchoolGoodsInfo GetViewSchoolGoodsInfo()
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ViewSchoolGoodsInfo>(this.GoodsInfo);
        }
        public bool SetUser(Guid? userId)
        {
            if (this.UserId != null)
                return false;
            this.UserId = userId;
            this.UpdateTime = DateTime.Now;
            return true;
        }

        public void Pay(ViewOrderPayWay payWay, decimal payment)
        {
            this.PayWay = payWay;
            this.Payment = payment;
            this.Status = ViewOrderState.WaitPayResult;
            this.UpdateTime = DateTime.Now;
        }

        public void PaySuccess(ViewOrderPayWay payWay, decimal payment)
        {
            if (this.Status == ViewOrderState.WaitPay || this.Status == ViewOrderState.WaitPayResult)
            {
                this.PayWay = payWay;
                this.Payment = payment;
                this.Status = ViewOrderState.PaySuccess;
                this.UpdateTime = DateTime.Now;
                this.AddDomainEvent(new ViewOrderPaySuccessDomainEvent()
                {
                    OrderId = this.Id,
                    OrderNo = this.Num,
                    UserId = this.UserId.Value,
                    ViewSchoolGoodsInfo = this.GetViewSchoolGoodsInfo()
                });
            }
            else
                throw new Exception("当前订单状态不可支付。");

        }
        public void PayFail(ViewOrderPayWay payWay)
        {
            if (this.Status == ViewOrderState.WaitPay || this.Status == ViewOrderState.WaitPayResult)
            {
                this.PayWay = payWay;
                this.Status = ViewOrderState.PayFail;
                this.UpdateTime = DateTime.Now;
            }
            else
                throw new Exception("当前订单状态不可支付。");

        }



        public SchoolViewOrder(Guid id, string num, Guid userId, ViewOrderPayWay? payWay
            , decimal amount, decimal? payment, ViewOrderState status
            , string remark, string goodsInfo, DateTime createTime, DateTime updateTime, DateTime expireTime)
        {
            this.Id = id;
            this.Num = num;
            this.UserId = userId;
            this.PayWay = payWay;
            this.Amount = amount;
            this.Payment = payment;
            this.Status = status;
            this.Remark = remark;
            this.GoodsInfo = goodsInfo;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.ExpireTime = expireTime;

        }


        public SchoolViewOrder(Guid id, string num, Guid? userId, int? payWay
    , decimal amount, decimal? payment, int status
    , string remark, string goodsInfo, DateTime createTime, DateTime updateTime, DateTime expireTime)
        {
            this.Id = id;
            this.Num = num;
            this.UserId = userId;
            this.PayWay = (ViewOrderPayWay)payWay;
            this.Amount = amount;
            this.Payment = payment;
            this.Status = (ViewOrderState)status;
            this.Remark = remark;
            this.GoodsInfo = goodsInfo;
            this.CreateTime = createTime;
            this.UpdateTime = updateTime;
            this.ExpireTime = expireTime;

        }

        private SchoolViewOrder()
        {

        }

        public static SchoolViewOrder NewDraft(Guid? userId, ViewSchoolGoodsInfo viewSchoolGoodsInfo)
        {

            GenerateNo.ISxbGenerateNo sxbGenerateNo = new GenerateNo.SxbGenerateNo();
            var schoolViewOrder = new SchoolViewOrder()
            {
                Id = Guid.NewGuid(),
                Num = $"vsch{sxbGenerateNo.GetNumber()}",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Status = ViewOrderState.WaitPay,
                ExpireTime = DateTime.Now.AddHours(1),
            };
            schoolViewOrder.SetGoodsInfo(viewSchoolGoodsInfo);
            schoolViewOrder.SetUser(userId);
            return schoolViewOrder;
        }

    }
}
