using MongoDB.Bson;
using Sxb.Domain;
using Sxb.Recommend.Domain.Event;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class MapFeature:Entity<ObjectId>,IAggregateRoot
    {

        /// <summary>
        /// 分值
        /// </summary>
        public double Score { get; private set; }

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; private set; }

        public int Type { get; private set; }

        public List<FrequencySchoolRateValue> FrequencyRateValues { get; private set; }

        /// <summary>
        /// 计分规则
        /// </summary>
        public string  ComputeRuleName { get; private set; }


        /// <summary>
        /// 特性别称
        /// </summary>
        public string Alias { get; private set; }


        public void Update(double score, double weight, string alias)
        {
            if (this.Score != score)
            {
                this.AddDomainEvent(new MapFeatureChangeEvent(this));
            }
            this.Score = score;
            this.Weight = weight;
            this.Alias = alias;

        }

        public MapFeature(string computeRuleName, double score, double weight, int type, string alias, List<FrequencySchoolRateValue> frequencyRateValues)
        {
            this.Id = ObjectId.GenerateNewId();
            this.Score = score;
            this.Weight = weight;
            this.ComputeRuleName = computeRuleName;
            this.Type = type;
            this.Alias = alias;
            this.FrequencyRateValues = frequencyRateValues;
        }
        public MapFeature(string computeRuleName, double score, double weight, int type, string alias)
        {
            this.Id = ObjectId.GenerateNewId();
            this.Score = score;
            this.Weight = weight;
            this.ComputeRuleName = computeRuleName;
            this.Type = type;
            this.Alias = alias;
        }
        public MapFeature(ObjectId _id, string computeRuleName,double score,double weight,int type,string alias)
        {
            this.Id = _id;
            this.Score = score;
            this.Weight = weight;
            this.ComputeRuleName = computeRuleName;
            this.Type = type;
            this.Alias = alias;
        }
        public MapFeature(string _id, string computeRuleName, double score, double weight, int type, string alias)
        {
            this.Id = ObjectId.Parse(_id);
            this.Score = score;
            this.Weight = weight;
            this.ComputeRuleName = computeRuleName;
            this.Type = type;
            this.Alias = alias;
        }

    }
}
