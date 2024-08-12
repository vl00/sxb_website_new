using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Domain.Value;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IComputeScoreService
    {
        Task<IEnumerable<SchoolMap>> GetSchoolMaps(School school);

        Task<IEnumerable<ArticleMap>> GetArticleMaps(Article article);

        Task<IEnumerable<SchoolScore>> GetSchoolScoresByFilter(SchoolFilterDefinition filterDefinition);
    }
}
