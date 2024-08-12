using MongoDB.Bson;
using Sxb.ArticleMajor.Common.MongoEntity;
using Sxb.ArticleMajor.Query.Mongodb;
using Sxb.Framework.Foundation;
using Sxb.Framework.Foundation.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.Runner.WashContent
{
    public class WashContentRunner : BaseRunner<WashContentRunner>
    {
        IArticleContentRepository _articleContentRepository;
        IArticleRepository _articleRepository;
        OutSiteCheck _outSiteCheck;
        FileLog _fileLog;
        FileLog _fileTimeLog;

        public WashContentRunner(IArticleContentRepository articleContentRepository, IArticleRepository articleRepository)
        {
            var id = Guid.NewGuid().ToString("N");
            var filename = $"./files/logs/{ClassName}-{id}.txt";
            var filenameTime = $"./files/logs/{ClassName}-{id}-time.txt";
            _fileLog = new FileLog(filename);
            _fileTimeLog = new FileLog(filenameTime);

            _outSiteCheck = new OutSiteCheck();
            _articleContentRepository = articleContentRepository;
            _articleRepository = articleRepository;
        }

        private const int Capacity = 10 * 10000;
        private const int BatchSize = 2 * 1000;
        private const int PageSize = 2 * 1000;
        protected override void Running()
        {
            _fileTimeLog.WriteLineNow();

            ObjectId? lastId = null;
            lastId = ObjectId.Parse("623bec68f7ea93ffab8ab7f7");
            new BatchHelper.BatchBuilder<ArticleContentWash>(Capacity, BatchSize)
                .From(async pageIndex =>
                {
                    //if (pageIndex > 10) return default;

                    var entities = await _articleContentRepository.GetList(lastId, pageIndex, PageSize);
                    lastId = entities.Count != 0 ? entities?[^1]?.Id : null;
                    _fileLog.WriteLine("-", pageIndex, entities.Count, DateTime.Now);
                    WriteLine("{0}-{1}-{2}", pageIndex, entities.Count, DateTime.Now);
                    return CommonHelper.MapperProperty<ArticleContent, ArticleContentWash>(entities);
                })
                .Convert(t =>
                {
                    var urls = GetOutUrls(t.Content);
                    if (urls == null || urls.Length == 0) return null; //remove

                     t.ReplaceUrls = urls;
                    return t;
                })
                .Handle(async items =>
                {
                    _fileTimeLog.WriteLineNow();

                    var replaceItems = Replace(items).ToList();
                    _fileLog.WriteLine(replaceItems);

                    await _articleContentRepository.UpdateContentAsync(replaceItems);
                })
                .Build()
                .Run();

            Dispose();
        }

        public void Dispose()
        {
            _fileLog?.Dispose();
            _fileLog = null;
            _fileTimeLog?.Dispose();
            _fileTimeLog = null;
        }

        private IEnumerable<ArticleContentWash> Replace(IEnumerable<ArticleContentWash> items)
        {
            var outUrls = items.SelectMany(s => s.ReplaceUrls).Distinct().ToArray();
            var codes = _articleRepository.GetCodesByFromUrlAsync(outUrls).GetAwaiter().GetResult();
            _outSiteCheck.AddArticleCode(codes.Select(s => (s.FromUrl, s.Code)).ToArray());


            foreach (var item in items)
            {
                bool isChanged = false;
                foreach (var url in item.ReplaceUrls)
                {
                    var code = _outSiteCheck.GetArticleCode(url);
                    if (code != null)
                    {
                        isChanged = true;
                        item.Content = item.Content.Replace(url, $"/detail/{code}");
                    }
                    //content = content.Replace(url, $"/");
                }
                if (isChanged)
                {
                    yield return item;
                }
            }
        }

        private string[] GetOutUrls(string content)
        {
            var matches = Regex.Matches(content, "<a.*?href=\"(.*?)\".*?>", RegexOptions.IgnoreCase);

            if (matches.Count > 0)
            {
                List<string> replaceUrls = new List<string>(matches.Count);
                foreach (Match match in matches)
                {
                    if (match.Groups.Count < 2) continue;

                    var url = match.Groups[1].Value;
                    if (_outSiteCheck.IsOutSite(url))
                    {
                        replaceUrls.Add(url);
                    }
                }
                return replaceUrls.ToArray();
            }
            return null;
        }
    }


}
