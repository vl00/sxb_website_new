using Sxb.Domain;
using Sxb.Recommend.Domain.Enum;
using Sxb.Recommend.Domain.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class School : Entity<Guid>, IAggregateRoot
    {

        /// <summary>
        /// 区域
        /// </summary>
        public int Area { get; private set; }

        public int City { get; private set; }


        /// <summary>
        /// 办学类型
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// 学校类型
        /// </summary>
        public string SchFtype { get; private set; }

        /// <summary>
        /// 学校评分
        /// </summary>
        public double Score { get; private set; }

        /// <summary>
        /// 学校认证
        /// </summary>
        public IEnumerable<string> Authentication { get; private set; }

        /// <summary>
        /// 课程设置
        /// </summary>
        public IEnumerable<string> CourseSetting { get; private set; }

        /// <summary>
        /// 特色课程
        /// </summary>
        public IEnumerable<string> SpecialCourse { get; private set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// 状态为3才是上线
        /// </summary>
        public int Status { get; private set; }

        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return IsValid && Status == 3;
            }
        }

        public School(Guid id
            , int area
            , int type
            , double score
            , IEnumerable<string> authentication
            , IEnumerable<string> courseSetting
            , IEnumerable<string> specialCourse
            , int city
            , string schFtype
            , bool isValid
            , int status)
        {
            this.Id = id;
            this.Area = area;
            this.Score = score;
            this.Type = type;
            this.Authentication = authentication;
            this.CourseSetting = courseSetting;
            this.SpecialCourse = specialCourse;
            this.City = city;
            this.SchFtype = schFtype;
            this.IsValid = isValid;
            this.Status = status;
        }

        public School(string csvrow)
        {
            var fields = csvrow.Split(',');
            this.Id = Guid.Parse(fields[0]);
            this.Area = string.IsNullOrEmpty(fields[1]) ? 0 : int.Parse(fields[1]);
            this.Type = string.IsNullOrEmpty(fields[2]) ? 0 : int.Parse(fields[2]);
            this.Score = string.IsNullOrEmpty(fields[3]) ? 0d : double.Parse(fields[3]);
            this.Authentication = string.IsNullOrEmpty(fields[4]) ? new string[0] : fields[4].Split("、");
            this.CourseSetting = string.IsNullOrEmpty(fields[5]) ? new string[0] : fields[5].Split("、");
            this.SpecialCourse = string.IsNullOrEmpty(fields[6]) ? new string[0] : fields[6].Split("、");
            this.City = string.IsNullOrEmpty(fields[7]) ? 0 : int.Parse(fields[7]);
            this.SchFtype = string.IsNullOrEmpty(fields[8]) ? string.Empty : fields[8];
            this.IsValid = string.IsNullOrEmpty(fields[9]) ? false : bool.Parse(fields[9]);
            this.Status = string.IsNullOrEmpty(fields[10]) ? 0 : int.Parse(fields[10]);

        }

        public string ToCSV()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}"
                , this.Id
                , this.Area
                , this.Type
                , this.Score
                , string.Join("、", this.Authentication)
                , string.Join("、", this.CourseSetting)
                , string.Join("、", this.SpecialCourse)
                , this.City
                , this.SchFtype
                , this.IsValid
                , this.Status);
            return sb.ToString();
        }


        public void Change(EntityChangeType changeType)
        {
            this.AddDomainEvent(new SchoolHasChangeEvent(this, changeType));
        }


    }
}
