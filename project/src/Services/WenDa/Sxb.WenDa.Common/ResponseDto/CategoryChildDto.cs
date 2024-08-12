namespace Sxb.WenDa.Common.ResponseDto
{
    /// <summary>
    /// 分类dto
    /// </summary>
    public class CategoryChildDto
    {
        public CategoryChildDto()
        {
        }

        public CategoryChildDto(long id, string name, List<CategoryChildDto> children)
        {
            Id = id;
            Name = name;
            Children = children;
        }

        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<CategoryChildDto> Children { get; set; }
    }




    /// <summary>
    /// 分类dto
    /// </summary>
    public class AttentionCategoryDto
    {
        public AttentionCategoryDto()
        {
        }

        public AttentionCategoryDto(long id, string name, List<AttentionCategoryDto> children = null)
        {
            Id = id;
            Name = name;
            Children = children;
        }

        /// <summary>
        /// id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否关注
        /// </summary>
        public bool IsAttention { get; set; }

        /// <summary>
        /// 子级
        /// </summary>
        public List<AttentionCategoryDto> Children { get; set; }
    }
}
