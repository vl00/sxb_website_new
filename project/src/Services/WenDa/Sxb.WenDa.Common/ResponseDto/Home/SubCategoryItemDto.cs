namespace Sxb.WenDa.Common.ResponseDto.Home
{

    /// <summary>
    /// 子站首页分类及其问题
    /// </summary>
    public class SubCategoryItemDto
    {
        public SubCategoryItemDto()
        {

        }

        public SubCategoryItemDto(CategoryChildDto categoryChildDto)
        {
            CategoryId = categoryChildDto.Id;
            CategoryName = categoryChildDto.Name;

            Children = categoryChildDto.Children?.Select(s => new SubCategoryItemDto(s)).ToList();
        }

        /// <summary>
        /// 分类id
        /// </summary>
        public long CategoryId { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 是否有子级
        /// </summary>
        public bool HasChild => Children != null && Children.Count != 0;

        /// <summary>
        /// 子级分类
        /// </summary>
        public List<SubCategoryItemDto> Children { get; set; }

        /// <summary>
        /// 问题列表
        /// </summary>
        public IEnumerable<QuestionLinkDto> Questions { get; set; }
    }
}
