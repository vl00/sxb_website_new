using Sxb.Framework.Cache.Redis;
using Sxb.School.Common.DTO;
using Sxb.School.Common.Entity;
using Sxb.School.Query.SQL.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sxb.School.API.Application.Query
{
    public class SchoolScoreQuery : ISchoolScoreQuery
    {
        readonly ISchoolScoreRepository _schoolScoreRepository;
        readonly IEasyRedisClient _easyRedisClient;
        public SchoolScoreQuery(ISchoolScoreRepository schoolScoreRepository, IEasyRedisClient easyRedisClient)
        {
            _schoolScoreRepository = schoolScoreRepository;
            _easyRedisClient = easyRedisClient;
        }

        public async Task<double> GetSchoolRankingInCity(int cityCode, double score, string schFType)
        {
            var result = await _easyRedisClient.GetOrAddAsync($"SchoolScoreRanking:City_{cityCode}-Score_{score}-SchFType_{schFType}", async () =>
            {
                var find = await _schoolScoreRepository.GetSchoolRankingInCity(cityCode, score, schFType);
                if (find > 0) find = double.Parse((find.ToString("0.00"))) * 100;

                return find;
            }, TimeSpan.FromHours(3));
            if (result == 0) result = 1;
            return result;
        }

        public async Task<SchoolScoreTreeDTO> GetSchoolScoreTreeByEID(Guid eid)
        {
            if (eid == default) return null;
            var scoreIndexs = await _easyRedisClient.GetOrAddAsync("ext:adscoreindex", async () =>
            {
                return await _schoolScoreRepository.GetAllScoreIndexs();
            });
            if (scoreIndexs == default || !scoreIndexs.Any()) return null;
            var schoolScores = await _schoolScoreRepository.GetExtensionScores(eid);
            if (schoolScores == default || !schoolScores.Any()) return null;
            var resultItem = new SchoolScoreTreeDTO()
            {
                CueentIndex = new IndexItem()
                {
                    ID = 22,
                    Name = scoreIndexs.FirstOrDefault(p => p.ID == 22).Index_Name,
                    ParentID = 0
                },
                Score = schoolScores.FirstOrDefault(p => p.IndexID == 22)?.Score
            };

            var indexIDs = schoolScores.Select(p => p.IndexID).ToArray();
            var secondLeve = scoreIndexs.Where(p => indexIDs.Contains(p.ID) && p.ParentID == 22);
            var thirdLevel = scoreIndexs.Where(p => indexIDs.Contains(p.ID) && p.ParentID != 22);

            resultItem.SubItems = new List<SchoolScoreTreeDTO>();

            foreach (var item in secondLeve)
            {
                var secondSubItem = new SchoolScoreTreeDTO()
                {
                    CueentIndex = new IndexItem()
                    {
                        ID = item.ID,
                        Name = item.Index_Name,
                        ParentID = item.ParentID
                    },
                    Score = schoolScores.FirstOrDefault(p => p.IndexID == item.ID)?.Score,
                    SubItems = new List<SchoolScoreTreeDTO>()
                };

                var subItems = thirdLevel.Where(p => p.ParentID == item.ID);
                if (subItems?.Any() == true)
                {
                    foreach (var thirdItem in subItems)
                    {
                        secondSubItem.SubItems.Add(new SchoolScoreTreeDTO()
                        {
                            CueentIndex = new IndexItem()
                            {
                                ID = thirdItem.ID,
                                Name = thirdItem.Index_Name,
                                ParentID = thirdItem.ParentID
                            },
                            Score = schoolScores.FirstOrDefault(p => p.IndexID == thirdItem.ID)?.Score
                        });

                    }
                }
                resultItem.SubItems.Add(secondSubItem);
            }

            return resultItem;
        }

        public async Task<IEnumerable<SchoolExtensionScoreInfo>> GetScores(Guid eid)
        {
            return await _schoolScoreRepository.GetExtensionScores(eid);
        }
    }
}
