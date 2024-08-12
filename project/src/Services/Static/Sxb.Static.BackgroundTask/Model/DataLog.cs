using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask.Model
{
    public record DataLog
    {
        public ObjectId Id { get; init; }
        public Guid DataId { get; init; }
        public string? UserId { get; init; }
        public string? DeviceId { get; init; }
        public DateTime CreateTime { get; init; }
    }
}
