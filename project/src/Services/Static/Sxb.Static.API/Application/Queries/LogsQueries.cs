using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.Static.API.Application.Queries
{
    public class LogsQueries : ILogsQueries
    {
        IMongoClient _mongoClient;
        public LogsQueries(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }

        public async Task<IEnumerable<LogData>> QueryArticleLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("DeployAreaInfo",
                            new BsonDocument("$elemMatch",
                            new BsonDocument("City", city))),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("ArticleLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryCommentLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
                     {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("City", city),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
                     };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("CommentLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryCourseLogDatasAsync(int type, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("Type", type),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("CourseLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryEvaluationLogDatasAsync(DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("EvaluationLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryLiveLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("City", city),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("LiveLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryQuestionLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
                     {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("City", city),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
                     };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("QuestionLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QuerySchoolLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("City", city),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("SchoolLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QuerySchoolRankLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
            {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("Citys",
                            new BsonDocument("$elemMatch",
                            new BsonDocument("$eq", city))),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
            };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("SchoolRankLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryTalentLogDatasAsync(int? city, DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
 {
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("City", city),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
 };
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("TalentLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }

        public async Task<IEnumerable<LogData>> QueryTopicLogDatasAsync(DateTime st, DateTime et)
        {
            var pipline = new BsonDocument[]
{
                new BsonDocument("$match",
                new BsonDocument("$and",
                new BsonArray
                        {
                            new BsonDocument("CreateTime",
                            new BsonDocument("$gte",st)),
                            new BsonDocument("CreateTime",
                            new BsonDocument("$lt",et))
                        })),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "DataId", "$DataId" },
                        { "UserId",
                new BsonDocument("$cond",
                new BsonArray
                            {
                                new BsonDocument("$or",
                                new BsonArray
                                    {
                                        new BsonDocument("UserId", ""),
                                        new BsonDocument("UserId", BsonNull.Value)
                                    }),
                                "$DeviceId",
                                "$UserId"
                            }) }
                    }),
                new BsonDocument("$group",
                new BsonDocument
                    {
                        { "_id",
                new BsonDocument
                        {
                            { "UserId", "$UserId" },
                            { "DataId", "$DataId" }
                        } },
                        { "count",
                new BsonDocument("$sum", 1) }
                    }),
                new BsonDocument("$project",
                new BsonDocument
                    {
                        { "_id", false },
                        { "DataId", "$_id.DataId" },
                        { "UV", "$count" }
                    })
};
            var collection = _mongoClient.GetDatabase("SxbStatic").GetCollection<BsonDocument>("TopicLogs");
            var result = (await collection.Aggregate<LogData>(pipline).ToListAsync());
            return result;
        }
    }


}
