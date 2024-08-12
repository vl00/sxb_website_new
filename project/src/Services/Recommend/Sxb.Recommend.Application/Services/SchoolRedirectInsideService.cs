using MediatR;
using MongoDB.Bson;
using Sxb.Framework.Foundation;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public class SchoolRedirectInsideService : ISchoolRedirectInsideService
    {
        ISchoolRedirectInsideRepository _repository;
        IMediator _mediator;
        ISchoolRepository _schoolRepository;
        public SchoolRedirectInsideService(ISchoolRedirectInsideRepository repository
            , IRecommendService recommendService
            , IMediator mediator
            , ISchoolRepository schoolRepository)
        {
            _repository = repository;
            _mediator = mediator;
            _schoolRepository = schoolRepository;
        }

        public async Task<List<SchoolRedirectInside>> ListAsync(Guid primaryId)
        {
            return await _repository.ListAsync(s=>s.SIdP == primaryId);
        }

        public async Task Add(SchoolRedirectInside schoolRedirectInside)
        {
            await _repository.AddAsync(schoolRedirectInside);
            schoolRedirectInside.Create();
            await _mediator.DispatchDomainEventsAsync(schoolRedirectInside);
        }

        public async Task Add(string shortNo1, string shortNo2)
        {
            var no1 = UrlShortIdUtil.Base322Long(shortNo1);
            var no2 = UrlShortIdUtil.Base322Long(shortNo2);
            var (id1, id2) = await _schoolRepository.GetIdByNo(no1, no2);

            SchoolRedirectInside schoolRedirectInside = new SchoolRedirectInside(id1, id2, DateTime.Now);
            await Add(schoolRedirectInside);
        }

    }
}
