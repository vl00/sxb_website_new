using System;
using System.Collections.Generic;
using Kogel.Dapper.Extension.Attributes;

namespace Sxb.SignActivity.Common.Entity
{
	/// <summary> 
	///  
	/// </summary> 
	[Serializable]
	[Display(Rename = "[Order]")]
	public partial class Order
	{
		/// <summary> 
		/// </summary> 
		[Display("id")] 
		public Guid Id {get;set;}

		/// <summary> 
		/// 订单号 
		/// </summary> 
		[Display("code")] 
		public string Code {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("courseid")] 
		public Guid? Courseid {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("userid")] 
		public Guid Userid {get;set;}

		/// <summary> 
		/// 收货人手机号 
		/// </summary> 
		[Display("mobile")] 
		public string Mobile {get;set;}

		/// <summary> 
		/// 收货地址 
		/// </summary> 
		[Display("address")] 
		public string Address {get;set;}

		/// <summary> 
		/// 订单状态(v2) 
		/// 101=待付款 
		/// 103=已付款|待发货 
		/// 302=商家已发货用户待收货 
		/// 333=已收货|已完成 
		/// 202=退款中 
		/// 203=已退款 
		/// </summary> 
		[Display("status")] 
		public int Status {get;set;}

		/// <summary> 
		/// 是否是部分退款 
		/// </summary> 
		public bool IsPartialRefund {get;set;}

		/// <summary> 
		/// 订单类型  1.留资|认证课程购买 
		/// 2.微信方式购买课程 
		/// </summary> 
		[Display("type")] 
		public byte Type {get;set;}

		/// <summary> 
		/// 商品价格 
		/// </summary> 
		[Display("payment")] 
		public decimal Payment {get;set;}

		/// <summary> 
		/// 运费 
		/// </summary> 
		[Display("freight")] 
		public decimal Freight {get;set;}

		/// <summary> 
		/// 支付总价格 
		/// </summary> 
		[Display("totalpayment")] 
		public decimal Totalpayment {get;set;}

		/// <summary> 
		/// 支付类型  1.微信 
		/// </summary> 
		[Display("paymenttype")] 
		public int? Paymenttype {get;set;}

		/// <summary> 
		/// 支付时间 
		/// </summary> 
		[Display("paymenttime")] 
		public DateTime? Paymenttime {get;set;}

		/// <summary> 
		/// 创建订单时间 
		/// </summary> 
		public DateTime? CreateTime {get;set;}

		/// <summary> 
		/// </summary> 
		public Guid? Creator {get;set;}

		/// <summary> 
		/// </summary> 
		public DateTime? ModifyDateTime {get;set;}

		/// <summary> 
		/// </summary> 
		public Guid? Modifier {get;set;}

		/// <summary> 
		/// 1=有效 
		/// 0=假删除 
		/// </summary> 
		public bool IsValid {get;set;}

		/// <summary> 
		/// type=1，用于留资 
		/// type=2，for CustomerRemark 
		/// </summary> 
		public string Remark {get;set;}

		/// <summary> 
		/// 机构反馈(多条反馈之间用) 
		/// </summary> 
		public string SystemRemark {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("recvUsername")] 
		public string RecvUsername {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("recvPostalcode")] 
		public string RecvPostalcode {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("recvProvince")] 
		public string RecvProvince {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("recvCity")] 
		public string RecvCity {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("recvArea")] 
		public string RecvArea {get;set;}

		/// <summary> 
		/// </summary> 
		[Display("age")] 
		public int? Age {get;set;}

		/// <summary> 
		/// 约课状态 
		/// 1=待排课 
		/// 2=排课中 
		/// 3=已排课 
		/// 4=完课 
		/// 99=无 
		/// </summary> 
		[Display("appointmentStatus")] 
		public int? AppointmentStatus {get;set;}

		/// <summary> 
		/// 上课电话 
		/// </summary> 
		public string BeginClassMobile {get;set;}

		/// <summary> 
		/// 是否是多个物流 
		/// </summary> 
		public bool? IsMultipleExpress {get;set;}

		/// <summary> 
		/// 快递公司编码 
		/// </summary> 
		[Display("expressType")] 
		public string ExpressType {get;set;}

		/// <summary> 
		/// 快递号 
		/// </summary> 
		[Display("expressCode")] 
		public string ExpressCode {get;set;}

		/// <summary> 
		/// 发货时间（物流） 
		/// </summary> 
		public DateTime? SendExpressTime {get;set;}

		/// <summary> 
		/// 退款时间 
		/// </summary> 
		public DateTime? RefundTime {get;set;}

		/// <summary> 
		/// 退款人id (一般为后台人员) 
		/// </summary> 
		public Guid? RefundUserId {get;set;}

		/// <summary> 
		/// 孩子归档ids [] 
		/// </summary> 
		public string ChildArchivesIds {get;set;}

		/// <summary> 
		/// 订单状态变成待收货时的时间 
		/// </summary> 
		public DateTime? ShippingTime {get;set;}

		/// <summary> 
		/// 订单来源类型   1微信合作院校库 
		/// </summary> 
		public byte SourceType {get;set;}

		/// <summary> 
		/// 来源的id（例如学部id） 
		/// </summary> 
		public Guid? SourceId {get;set;}

		/// <summary> 
		/// 来源的扩展字段 
		/// </summary> 
		public string SourceExtend {get;set;}

		/// <summary> 
		/// 预订单号 
		/// </summary> 
		public string AdvanceOrderNo {get;set;}

		/// <summary> 
		/// 预订单id 
		/// </summary> 
		public Guid? AdvanceOrderId {get;set;}


	}
}