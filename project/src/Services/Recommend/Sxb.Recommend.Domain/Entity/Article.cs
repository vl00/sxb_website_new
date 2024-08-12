using Sxb.Domain;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sxb.Recommend.Domain.Entity
{
    public class Article : Entity<Guid>, IAggregateRoot
    {
        /// <summary>
        ///  文章类型 1 政策性文章 2 数据对比类 3 教育投资类 4 学校介绍类 5 育儿成长类 6 学科备考类 7 热点新闻类
        /// </summary>
        public int Type { get; private set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? Time { get; private set; }


        /// <summary>
        /// 标签
        /// </summary>
        public List<Guid> Tags { get; private set; }

        /// <summary>
        /// 关联学校
        /// </summary>
        public List<Guid> Schools { get; private set; }


        /// <summary>
        /// 投放地区信息
        /// </summary>
        public List<DeployAreaInfo> DeployAreaInfos { get; private set; }


        /// <summary>
        /// 文章分类（按学校的类型进行分类，用于关联到学校）
        /// </summary>
        public List<int> SchoolTypes { get; private set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Show { get; private set; }


        /// <summary>
        /// 上线状态 true=>上线 false ->未上线
        /// </summary>
        public bool IsOnline
        {
            get
            {
                return !IsDeleted && Show && Time != null && Time < DateTime.Now;

            }
        }


        public Article(Guid id
            , int type
            , DateTime? time
            , List<Guid> tags
            , List<Guid> schools
            , List<DeployAreaInfo> deployAreaInfos
            , List<int> schoolTypes
            , bool isDeleted
            , bool show)
        {
            this.Id = id;
            this.Type = type;
            this.Time = time;
            this.Tags = tags;
            this.Schools = schools;
            this.DeployAreaInfos = deployAreaInfos;
            this.SchoolTypes = schoolTypes;
            this.IsDeleted = isDeleted;
            this.Show = show;

        }

        public Article(string csv)
        {
            var fields = csv.Split(',');
            this.Id = Guid.Parse(fields[0]);
            this.Type = string.IsNullOrEmpty(fields[1]) ? 0 : int.Parse(fields[1]);
            this.Time = string.IsNullOrEmpty(fields[2]) ? default(DateTime) : DateTime.Parse(fields[2]);
            this.Tags = string.IsNullOrEmpty(fields[3]) ? null : fields[3].Split("、").Select(s => Guid.Parse(s)).ToList();
            this.Schools = string.IsNullOrEmpty(fields[4]) ? null : fields[4].Split("、").Select(s => Guid.Parse(s)).ToList();
            this.DeployAreaInfos = string.IsNullOrEmpty(fields[5]) ? null : fields[5].Split("、").Select(s =>
            {
                var p_c_a = s.Split("-").Select(l => string.IsNullOrEmpty(l) ? 0 : int.Parse(l)).ToArray();
                return new DeployAreaInfo() { Province = p_c_a[0], City = p_c_a[1], Area = p_c_a[2] };
            }).ToList();
            this.SchoolTypes = string.IsNullOrEmpty(fields[6]) ? null : fields[6].Split("、").Select(s => int.Parse(s)).ToList();
            this.IsDeleted = string.IsNullOrEmpty(fields[7]) ? false : bool.Parse(fields[7]);
            this.Show = string.IsNullOrEmpty(fields[8]) ? false : bool.Parse(fields[8]);

        }

        public string ToCSV()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}"
                , this.Id
                , this.Type
                , this.Time
                , Tags == null ? "" : string.Join("、", this.Tags)
                , Schools == null ? "" : string.Join("、", this.Schools)
                , DeployAreaInfos == null ? "" : string.Join("、", this.DeployAreaInfos.Select(s => string.Format("{0}-{1}-{2}", s.Province, s.City, s.Area)))
                , SchoolTypes == null ? "" : string.Join("、", SchoolTypes)
                , this.IsDeleted
                , this.Show
                );
        }
    }
}
