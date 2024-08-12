using System;
using System.Collections.Generic;

namespace Sxb.WenDa.API.RequestContact.Wenda
{
    /// <summary>
    /// 发问题-查城市下的分类+标签
    /// </summary>
    public class GetCityCategoryQuery
    { 
        /// <summary>
        /// 城市编码.此参数必传.
        /// </summary>        
        public long City { get; set; }
        /// <summary>
        /// 分类id。<br/>
        /// 一开始选城市时,不传.后续选分类时,此参数必传.
        /// </summary>
        public long? CategoryId { get; set; }
    }

    public class GetCityCategoryQueryResult
    {
        /// <summary>城市编码</summary>
        public long City { get; set; }
        /// <summary>
        /// 分类id。选城市时,此值为0
        /// </summary>
        public long CategoryId { get; set; }
        /// <summary>
        /// 下一级分类s.<br/>
        /// 为null时表示该分类没有下级分类了.
        /// </summary>
        public List<CityCategoryItemVm> ChildrenCategories { get; set; }
        /// <summary>
        /// 标签s.<br/>
        /// 通常 childrenCategories 为null没下级分类, 但是有标签，也可能没 
        /// </summary>
        public List<CityCategoryItemVm> Tags { get; set; }
    }

    public class CityCategoryItemVm
    {
        /// <summary>id</summary>        
        public long Id { get; set; }
        /// <summary>名称</summary>        
        public string Name { get; set; }
        /// <summary>
        /// 是否需要查学校. <br/>
        /// 分类=学校问答 时此值为true.
        /// </summary>
        public bool? CanFindSchool { get; set; }
    }
}
