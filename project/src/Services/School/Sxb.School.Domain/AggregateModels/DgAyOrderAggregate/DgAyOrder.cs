using Sxb.Domain;
using Sxb.School.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.School.Domain.AggregateModels.DgAyOrderAggregate
{
    public class DgAyOrder : Entity<Guid>, IAggregateRoot
    {

        public string Num { get; private set; }

        public Guid? UserId { get; private set; }

        public DgAyOrderPayWay? PayWay { get; private set; }

        public decimal Amount { get; private set; }

        public decimal? Payment { get; private set; }
        public DgAyOrderState Status { get; private set; }

        public string Remark { get; private set; }

        public DateTime CreateTime { get; private set; }

        public DateTime UpdateTime { get; private set; }

        public DateTime ExpireTime { get; private set; }

        /// <summary>
        /// 终端类型 1->h5 2->pc 3->小程序
        /// </summary>
        public byte Termtyp { get; private set; }

        public IReadOnlyList<DgAyOrderDetail> OrderDetails => _orderDetails;

        private List<DgAyOrderDetail> _orderDetails = new List<DgAyOrderDetail>();

        public void AddOrderDetail(DgAyOrderDetail detail)
        {
            detail.SetOrderId(this.Id);
            this._orderDetails.Add(detail);
            this.Amount += detail.UnitPrice * detail.Number;
            this.UpdateTime = DateTime.Now;
        }

        public void Pay(DgAyOrderPayWay payWay)
        {

            if (this.Status == DgAyOrderState.WaitPay)
            {
                this.PayWay = payWay;
                if (payWay == DgAyOrderPayWay.SubscribeWPFree)
                {
                    this.Payment = 0;
                }
                else
                {
                    this.Payment = this.Amount;
                }
                this.Status = DgAyOrderState.PaySuccess;
                this.UpdateTime = DateTime.Now;
                this.AddDomainEvent(new DgAyOrderPaySuccessDomainEvent(){ Order = this });
            }
            else
                throw new Exception("当前订单状态不可支付。");
        }

        public void SetRemark(string remark)
        {
            this.Remark = remark;
        }

        public void SetStatus(DgAyOrderState state)
        {
            if (this.Status != DgAyOrderState.PaySuccess && this.Status != DgAyOrderState.PayFail)
            {
                this.Status = state;
                this.UpdateTime = DateTime.Now;
            }
            else throw new Exception("当前订单处于终结状态,不可修改。");
        }

        public bool SetUser(Guid? userId)
        {
            if (this.UserId != null)
                return false;
            this.UserId = userId;
            this.UpdateTime = DateTime.Now;
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="termtyp">终端类型 1->h5 2->pc 3->小程序</param>
        /// <returns></returns>
        public static DgAyOrder NewDraft(Guid? userId ,byte termtyp)
        {
            GenerateNo.ISxbGenerateNo sxbGenerateNo = new GenerateNo.SxbGenerateNo();
            DgAyOrder order = new DgAyOrder();
            order.Id = Guid.NewGuid();
            order.Num = $"DgAy{sxbGenerateNo.GetNumber()}";
            order.UserId = userId;
            order.Termtyp = termtyp;
            order.CreateTime = DateTime.Now;
            order.UpdateTime = DateTime.Now;
            order.ExpireTime = order.CreateTime.AddMinutes(30);
            order.Status = DgAyOrderState.WaitPay;
            return order;
        }



    }
}
