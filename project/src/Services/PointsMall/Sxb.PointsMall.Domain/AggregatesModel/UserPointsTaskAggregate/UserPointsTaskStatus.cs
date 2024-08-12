namespace Sxb.PointsMall.Domain.AggregatesModel.UserPointsTaskAggregate
{
    /// <summary>
    /// 状态  0 为创建/未完成  1 已完成,未领取积分 2 已领取积分
    /// </summary>
    public enum UserPointsTaskStatus
    {
        UnFinish = 0,
        Finished = 1,
        PointsGranted = 2
    }
}