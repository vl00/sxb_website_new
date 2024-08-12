using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Static.BackgroundTask
{
   public  class Config
    {
        public static readonly string SqlServerConstr_School = "Server=10.1.0.199;database=iSchoolData;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";
        public static readonly string SqlServerConstr_Article = "Server=10.1.0.199;database=iSchoolArticle;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";
        public static readonly string SqlServerConstr_Product = "Server=10.1.0.199;database=iSchoolProduct;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";
        public static readonly string SqlServerConstr_User= "Server=10.1.0.199;database=iSchoolUser;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";
        public static readonly string SqlServerConstr_Live = "Server=10.1.0.199;database=iSchoolLive;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";
        public static readonly string SqlServerConstr_Org = "Server=10.1.0.199;database=Organization;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;Max Pool Size = 10000;";

        public static readonly string MongoConstr = "mongodb://172.16.0.5:27014,10.1.0.12:27014,10.1.0.17:27014/?readPreference=primary&appname=sxbrecommend&ssl=false";
        public static readonly ConsumerConfig KafkaConfig = new ConsumerConfig
        {
            BootstrapServers = " 10.1.0.2:9092,10.1.0.6:9092,10.1.0.11:9092",
            GroupId = "Sxb.Static.BackgroundTask",
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
    }
}
