using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class SchoolMapService : ISchoolMapService
    {
        ISchoolMapRepository _schoolMapRepository;
        ISchoolFileRepository _schoolFileRepository;
        IComputeScoreService _computeScoreService;
        public SchoolMapService(ISchoolMapRepository schoolMapRepository
            , IComputeScoreService computeScoreService
            , ISchoolFileRepository schoolFileRepository)
        {
            _schoolMapRepository = schoolMapRepository;
            this._computeScoreService = computeScoreService;
            _schoolFileRepository = schoolFileRepository;
        }
        public async Task ClearAll()
        {
            await _schoolMapRepository.ClearAll();
        }

        public async Task UpsertSchoolMaps(IEnumerable<Guid> schoolIds)
        {
            foreach (var schoolId in schoolIds)
            {
                var school = _schoolFileRepository.Query(s => s.IsOnline && s.Id == schoolId).FirstOrDefault();
                if (school != null)
                {
                    await UpsertSchoolMaps(school);
                }

            }


        }

        public async Task<IEnumerable<SchoolMap>> UpsertSchoolMaps(School school)
        {
            var schoolMaps = await _computeScoreService.GetSchoolMaps(school);
            await _schoolMapRepository.InsertManyAsync(schoolMaps);
            return schoolMaps;
        }



    }
}
