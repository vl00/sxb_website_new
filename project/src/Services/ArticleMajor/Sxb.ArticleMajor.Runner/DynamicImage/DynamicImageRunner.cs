using Newtonsoft.Json;
using Sxb.ArticleMajor.Common.Enum;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Common.QueryDto;
using Sxb.ArticleMajor.Query.Mongodb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.DynamicImage
{
    internal partial class DynamicImageRunner : BaseRunner<DynamicImageRunner>
    {
        //是否上传到图片服务器
        bool enableUpload = true;
        private readonly UploadService _uploadService;
        private readonly ICategoryRepository _categoryRepository;

        public DynamicImageRunner(UploadService uploadService, ICategoryRepository categoryRepository)
        {
            _uploadService = uploadService;
            _categoryRepository = categoryRepository;
        }


        List<Category> allCategories;
        protected override void Running()
        {
            //var filename = "Assets\\Images\\中学教育\\中考体育\\中考体育\\中考体育1.jpg";
            //var file = new FileInfo(filename);


            var data = LoadFromFile<CategoryImage>("Assets\\category-images-637837150644403791.txt").ToArray();
            if (!data.Any())
            {
                data = GetCategoryImages();
                WriteToFile($"Assets\\category-images-{DateTime.Now.Ticks}.txt", data);
            }


            List<IdImage> idImages = new List<IdImage>();
            allCategories = _categoryRepository.GetListAsync().GetAwaiter().GetResult().ToList();
            foreach (var item in data)
            {
                var category = allCategories.FirstOrDefault(s => s.Name == item.Name);
                if (category == null) throw new Exception(nameof(category));

                idImages.AddRange(GetIdImages(item, category.Platform, depth: 0));
            }

            WriteToFile($"Assets\\category-images-{DateTime.Now.Ticks}.json", JsonConvert.SerializeObject(idImages));
        }

        public List<IdImage> GetIdImages(CategoryImage categoryImage, ArticlePlatform platform, int depth)
        {
            var data = new List<IdImage>();
            if (categoryImage.Images != null && categoryImage.Images.Length != 0)
            {
                var item = new IdImage();

                var (id, parentId) = GetId(categoryImage.Name, platform, depth);
                item.Id = id;
                item.ParentId = parentId;
                item.Name = categoryImage.Name;
                item.Images = categoryImage.Images;
                data.Add(item);
            }


            if (categoryImage.Children?.Any() == true)
            {
                int childDepth = depth + 1;
                foreach (var child in categoryImage.Children)
                    data.AddRange(GetIdImages(child, platform, childDepth));
            }
            return data;
        }

        private (int Id, int ParentId) GetId(string name, ArticlePlatform platform, int depth)
        {
            var names = new List<string>() { name };
            if (name == "教育体系")
                names.Add("教育体系介绍");
            else if (name == "填报时间")
                names.Add("填报志愿时间");
            else if (name == "填报志愿时间")
                names.Add("填报时间");
            else if (name == "能力测评")
                names.Add("能力测试");
            else if (name == "英语早教")
                names.Add("趣味英语早教");

            var category = allCategories.FirstOrDefault(s => s.Platform == platform && names.Contains(s.Name) && s.Depth == depth);
            int id = category.Id;//not null
            int parentId = category.ParentId;
            if (!category.IsLeaf)
            {
                //有同名叶子子级   e.g. 幼儿教育 -> 幼小衔接 -> 幼小衔接
                var child = allCategories.FirstOrDefault(s => s.ParentId == category.Id && s.Name == category.Name);
                if (child != null && child.IsLeaf)
                {
                    id = child.Id;
                    parentId = child.ParentId;
                }
            }
            return (id, parentId);
        }

        public CategoryImage[] GetCategoryImages()
        {
            var dirPath = "Assets\\Images";
            var dirs = Directory.GetDirectories(dirPath);
            var sites = dirs.Select(x => GetCategoryImage(x)).ToArray();
            return sites;
        }

        public string GetDirName(string dirPath)
        {
            return new DirectoryInfo(dirPath).Name;
        }

        public CategoryImage GetCategoryImage(string dirPath)
        {
            var item = new CategoryImage(dirPath);
            var children = Directory.GetDirectories(dirPath);

            item.Name = GetDirName(dirPath);
            item.Images = Directory.GetFiles(dirPath).Select(s => UploadArticleMajorImage(s)).ToArray();

            if (children.Any())
            {
                item.Children = children.Select(s => GetCategoryImage(s)).ToArray();
            }
            return item;
        }


        public string UploadArticleMajorImage(string filename)
        {
            var spe = Path.DirectorySeparatorChar;
            //Assets\\Images\\中学教育\\中考体育\\中考体育\\中考体育1.jpg
            var paths = filename.Split(spe).Skip(2).ToList();
            var path = "eg-articlemajor-images/" + string.Join('/', paths.Take(paths.Count - 1));
            if (!enableUpload)
            {
                return filename;
            }

            var name = Path.GetFileNameWithoutExtension(filename);
            using var stream = File.OpenRead(filename);
            var cosUrl = UploadArticleMajorImage(path, name, stream).GetAwaiter().GetResult();
            return cosUrl;
        }

        public async Task<string> UploadArticleMajorImage(string path, string filenName, Stream stream)
        {
            var response = await _uploadService.UploadImg(type: "article", filenName, stream, path);
            return response.cdnUrl;
        }

    }
}
