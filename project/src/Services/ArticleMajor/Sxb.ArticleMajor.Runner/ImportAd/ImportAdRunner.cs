using Sxb.ArticleMajor.Runner.ImportAd.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib;
using Dapper.Contrib.Extensions;
using System.Data;
using System.Drawing;
using Kogel.Dapper.Extension.MsSql;

namespace Sxb.ArticleMajor.Runner.ImportAd
{
    internal class ImportAdRunner : BaseRunner<ImportAdRunner>
    {
        string configFileName = "Assets/AdConfig/PC子站广告配置.xlsx";
        string imagesPath = "Assets/AdConfig/banner0325";

        string connectionStr = "Server=192.168.31.13;Database=iSchoolArticle;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;";
        //string connectionStr = "Server=10.1.0.199;Database=iSchoolArticle;user id=iSchool;password=SxbLucas$0769;MultipleActiveResultSets=true;";
        IDbConnection _db { get; }

        private IDbConnection GetCollection()
        {
            var conn = new SqlConnection(connectionStr);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        UploadService _uploadService;
        public ImportAdRunner(UploadService uploadService)
        {
            _db = GetCollection();
            _uploadService = uploadService;
        }

        protected override void Running()
        {
            //图片名称, 图片地址
            IEnumerable<(string Name, string FileName)> images = Directory.GetFiles(imagesPath).Select(s =>
            {
                return (Path.GetFileNameWithoutExtension(s), s);
            });

            var reader = NPOIHelper.NPOIHelperBuild.GetReader(configFileName);
            var configs = reader.ReadSheet<AdConfig>(sheetIndex: 0);

            var importConfigs = new List<AdConfig>();
            foreach (var config in configs)
            {
                if (string.IsNullOrWhiteSpace(config.ImageName))
                    continue;

                //必须存在广告位
                if (config.LocationIds.Length == 0)
                    throw new Exception(nameof(config.LocationIds));

                //必须存在广告图片
                var image = images.First(s => s.Name == config.ImageName);
                config.ImageFileName = image.FileName;
                importConfigs.Add(config);
            }

            InsertAsync(importConfigs).GetAwaiter().GetResult();
        }

        public (int Width, int Height) GetImageSize(string imageFileName)
        {
            using Image image = Image.FromFile(imageFileName);
            return (image.Width, image.Height);
        }

        public async Task InsertAsync(List<AdConfig> configs)
        {
            foreach (AdConfig config in configs)
            {
                await InsertAdAsync(config.LocationIds, config.ImageName, config.ImageFileName);
            }
        }

        public async Task InsertAdAsync(int[] locationIds, string imageName, string imageFileName)
        {
            var creator = "system-2022-03-25-1";
            var time = DateTime.Now;
            var exists = await _db.QuerySet<AdvertisingBase>().Where(s => s.Title == imageName && s.Creator == creator).ToIEnumerableAsync();
            if (exists.Any())
            {
                //"已添加"
                return;
            }

            using var tran = _db.BeginTransaction();
            try
            {

                var uploadResponse = await _uploadService.UploadImg("article", imageName, imageFileName, path: "ad-image");
                var (width, height) = GetImageSize(imageFileName);

                //新增广告
                var adId = await _db.InsertAsync(new AdvertisingBase()
                {
                    CreateTime = time,
                    Creator = creator,
                    IsDelete = false,
                    Width = width,
                    Height = height,
                    PicUrl = uploadResponse.cdnUrl,
                    Status = 1,
                    Title = imageName,
                    UpdateTime = DateTime.Now,
                    Updator = creator,
                    Url = "",
                    IsSpecial = false
                }, tran);

                //新增多个排期
                var relations = new List<Ad_City_Schedule_R>();
                var schedules = locationIds.Select(locationId =>
                 {
                     var schedule = new AdvertisingSchedule()
                     {
                         Id = Guid.NewGuid(),
                         CreateTime = DateTime.Now,
                         Creator = creator,
                         UpdateTime = time,
                         Updator = creator,
                         UpTime = time,
                         ExpireTime = time.AddYears(1),
                         Location = locationId,
                         Sort = 1
                     };

                     relations.Add(new Ad_City_Schedule_R()
                     {
                         AdId = (int)adId,
                         CityId = 0,
                         Creator = creator,
                         CreateTime = time,
                         UpdateTime = time,
                         IsTop = false,
                         IsDeleted = false,
                         ScheduleId = schedule.Id,
                         Sort = 0,
                         DataId = string.Empty,
                         DataType = 0,
                         LocationId = locationId,
                         BeforeCount = 0,
                     });
                     return schedule;
                 });

                await _db.InsertAsync(schedules, tran);
                await _db.InsertAsync(relations, tran);

                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
