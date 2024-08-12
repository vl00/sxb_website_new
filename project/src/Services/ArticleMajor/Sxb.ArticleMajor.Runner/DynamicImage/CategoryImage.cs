namespace Sxb.ArticleMajor.Runner.DynamicImage
{
    public class CategoryImage
    {
        public CategoryImage(string dirPath)
        {
            DirPath = dirPath ?? throw new ArgumentNullException(nameof(dirPath));
        }

        public string DirPath { get; set; }

        public string Name { get; set; }

        public CategoryImage[] Children { get; set; }

        public string[] Images { get; set; }
    }
}
