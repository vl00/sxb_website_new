using Dapper;
using Sxb.SignActivity.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sxb.SignActivity.Common.Entity;
using System.Threading.Tasks;
using Sxb.Framework.Foundation;
using Kogel.Dapper.Extension.MsSql;
using Sxb.SignActivity.Common.DTO;

namespace Sxb.SignActivity.Query.SQL.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrgDB _orgDB;

        public OrderRepository(OrgDB orgDB)
        {
            _orgDB = orgDB ?? throw new ArgumentNullException(nameof(orgDB));
        }

        public async Task<Order> GetAsync(Guid orderId)
        {
            var sql = $@" 
SELECT 
	 *
FROM 
	[Order] O
WHERE 
	O.IsValid = 1 
    AND O.Id = @orderId
";
            var order = await _orgDB.Connection.QueryFirstOrDefaultAsync<Order>(sql, new { orderId });
            return order;
        }

        public async Task<IEnumerable<Order>> GetListAsync(DateTime startTime, DateTime endTime, int? status = null)
        {
			var statusSql = status == null ? "AND O.Status >= 103 " : $"AND O.Status = {status}";
			var sql = $@" 
SELECT 
	 *
FROM 
	[Order] O
WHERE
	IsValid = 1 
	AND Type >= 2
	{statusSql}
	AND paymenttime between @startTime AND @endTime
";
            var orders = await _orgDB.Connection.QueryAsync<Order>(sql, new { startTime, endTime });
            return orders;
        }

        public async Task<IEnumerable<OrderDayPayDTO>> GetOrderDayPaysAsync(Guid? userId, DateTime startTime, DateTime endTime, int? status = null)
        {
            //var statusSql = status == null ? "AND (O.Status IN(103,202,203) OR O.Status >= 300)" : $"AND O.Status = {status}";
            var statusSql = status == null ? "AND O.Status >= 103 " : $"AND O.Status = {status}";
            var userSql = userId == null ? " " : $"AND O.UserId = @userId";
            var sql = $@"
SELECT 
	P.UserId,
	P.TotalPayment,
	isnull(RF.Refund,0)  AS TotalRefund,
	isnull(RF.RefundFreight,0) AS TotalRefundFreight
FROM
-- 订单支付 - 排除隐形上架
(	
	SELECT 
		O.UserId,
	   ISNULL(SUM(OD.Price * OD.Number), 0) AS TotalPayment
	FROM 
		[Order] O
		INNER JOIN dbo.OrderDetial OD ON OD.OrderId = O.Id
		INNER JOIN dbo.Course C ON C.Id = OD.courseid AND C.IsInvisibleOnline = 0
	WHERE 
		O.IsValid = 1 
	    AND O.Type >= 2
	    {userSql}
		{statusSql}
		AND O.paymenttime between @startTime AND @endTime
	GROUP BY
		O.UserId
) AS P
-- 订单退运费 - 排除隐形上架
LEFT JOIN (
	SELECT
		O.UserId,
		SUM(R.Price) AS Refund,
		SUM(CASE WHEN R.IsContainFreight = 1 THEN O.freight ELSE 0 END) AS RefundFreight
	FROM 
		[Order] O
		INNER JOIN dbo.OrderDetial OD ON OD.OrderId = O.Id
		INNER JOIN dbo.Course C ON C.Id = OD.courseid AND C.IsInvisibleOnline = 0
		INNER JOIN dbo.OrderRefunds R ON R.OrderId = OD.OrderId AND R.OrderDetailId = OD.Id 
			AND R.IsValid = 1 
			AND (R.Type in (3,4) or R.Status in (5,17))
	WHERE 
		O.IsValid = 1
	    AND O.Type >= 2
	    {userSql}
		{statusSql}
		AND O.paymenttime between @startTime AND @endTime
	GROUP BY
		O.UserId
) AS RF ON RF.UserId = P.UserId

";
            var orders = await _orgDB.Connection.QueryAsync<OrderDayPayDTO>(sql, new { userId, startTime, endTime });
            return orders;
        }

        /**
         * 订单状态(v2)
         * 101=待付款
         * 103=已付款|待发货
         * 302=商家已发货用户待收货 333=已收货|已完成
         * 202=退款中 203=已退款
         * */
        public async Task<decimal> GetShippedTotalAmountAsync(Guid userId, DateTime signDate)
        {
            //已收货
            int status = 333;
            var startTime = signDate;
            return await GetTotalAmountAsync(userId, startTime, startTime.AddDays(1), status);
        }

        public async Task<decimal> GetTotalAmountAsync(Guid userId, DateTime startTime, DateTime endTime, int? status = null)
        {
            //var pay = await GetTotalOrderPayAsync(userId, startTime, endTime, status);
            //var refund = await GetTotalOrderRefundAsync(userId, startTime, endTime);

            var data = await GetOrderDayPaysAsync(userId, startTime, endTime, status);
            return data.FirstOrDefault()?.RealPayment ?? 0;
        }

        public async Task<decimal> GetTotalOrderPayAsync(Guid userId, DateTime startTime, DateTime endTime, int? status = null)
        {
            var statusSql = status == null ? "AND (Status IN(103,202,203) OR Status >= 300)" : $"AND Status = {status}";

            var sql = $@" 
SELECT 
	ISNULL(SUM(payment), 0) 
FROM 
	[Order] 
WHERE 
	IsValid = 1 
	AND Type >= 2
	{statusSql}
	AND UserId = @userId 
	AND paymenttime between @startTime AND @endTime

";
            return await _orgDB.Connection.QueryFirstOrDefaultAsync<decimal>(sql, new { userId, startTime, endTime });
        }

        public async Task<decimal> GetTotalOrderRefundAsync(Guid userId, DateTime startTime, DateTime endTime)
        {
            var sql = $@" 
SELECT 
	ISNULL(SUM(R.Price), 0) 
FROM 
	[Order] O
	INNER JOIN dbo.OrderRefunds R ON R.OrderId = O.Id 
		AND R.IsValid = 1 
		AND (R.Type in (3,4) or R.Status in (5,17))
WHERE 
	O.IsValid = 1 
    AND O.Type >= 2
	AND UserId = @userId 
	AND paymenttime between @startTime AND @endTime
";
            return await _orgDB.Connection.QueryFirstOrDefaultAsync<decimal>(sql, new { userId, startTime, endTime });
        }
    }
}
