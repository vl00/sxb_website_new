using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sxb.Recommend.Infrastructure.IRepository;
using MongoDB.Bson;
using MediatR;
using Sxb.Infrastructure.Core.Extensions;

namespace Sxb.Recommend.Application.Services
{
    public class MapFeatureService : IMapFeatureService
    {
        IMapFeatureRepository _mapFeatureRepository;
        IMediator _mediator;

        public MapFeatureService(IMapFeatureRepository mapFeatureRepository, IMediator mediator)
        {
            _mapFeatureRepository = mapFeatureRepository;
            _mediator = mediator;
        }
        public async Task<IEnumerable<MapFeature>> GetAsync(int type)
        {
            return await _mapFeatureRepository.GetMapFeaturesAsync(type);
        }

        public async Task<MapFeature> GetAsync(string id)
        {
            return await _mapFeatureRepository.GetAsync(ObjectId.Parse(id));
        }

        public async Task InitialFeatures()
        {

            if ((await _mapFeatureRepository.HasMapFeature(1)) == false)
            {
                var newSchoolFeatures = new List<MapFeature>()
                {
                    new MapFeature("SchoolMapFeature.AreaComputeRule",7,1,1,"区域特性"),
                    new MapFeature("SchoolMapFeature.AuthenticationComputeRule",3,1,1,"学校认证特性"),
                    new MapFeature("SchoolMapFeature.CourseSettingComputeRule",2,1,1,"课程设置特性"),
                    new MapFeature("SchoolMapFeature.ScoreComputeRule",4,1,1,"评分特性"),
                    new MapFeature("SchoolMapFeature.SpecialCourseComputeRule",1,1,1,"特色课程特性"),
                    new MapFeature("SchoolMapFeature.SchFtypeComputeRule",5,1,1,"类型特性"),
                    new MapFeature("SchoolMapFeature.FrequencyComputeRule",10,1,1,"频率特性",new List<Domain.Value.FrequencySchoolRateValue>(){
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.2,
                             Score = 50
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.4,
                             Score = 40
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.6,
                             Score = 30
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.8,
                             Score = 20
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 1,
                             Score = 10
                         },
                        })
                    
                };
                await _mapFeatureRepository.InsertManyAsync(newSchoolFeatures);
            }

            if ((await _mapFeatureRepository.HasMapFeature(2)) == false)
            {
                var newSchoolFeatures = new List<MapFeature>()
                {
                    new MapFeature("ArticleMapFeature.CorrelationSchoolsComputeRule",6,1,2,"关联学校"),
                    new MapFeature("ArticleMapFeature.TagsComputeRule",5,1,2,"文章标签"),
                    new MapFeature("ArticleMapFeature.CityComputeRule",4,1,2,"投放城市"),
                    new MapFeature("ArticleMapFeature.ProvinceComputeRule",3,1,2,"投放省份"),
                    new MapFeature("ArticleMapFeature.SchoolTypesComputeRule",2,1,2,"文章分类"),
                    new MapFeature("ArticleMapFeature.TypeComputeRule",1,1,2,"文章类型"),
                    new MapFeature("ArticleMapFeature.FrequencyComputeRule",10,1,2,"频率特性",new List<Domain.Value.FrequencySchoolRateValue>(){
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.2,
                             Score = 50
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.4,
                             Score = 40
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.6,
                             Score = 30
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 0.8,
                             Score = 20
                         },
                         new Domain.Value.FrequencySchoolRateValue(){
                             Rate = 1,
                             Score = 10
                         },
                        })

                };
                await _mapFeatureRepository.InsertManyAsync(newSchoolFeatures);
            }

            if ((await _mapFeatureRepository.HasMapFeature(3)) == false)
            {
                var newSchoolFeatures = new List<MapFeature>()
                {
                    new MapFeature("SchoolFilterDefinitionMapFeature.AreaComputeRule",6,1,3,"区域特性"),
                    new MapFeature("SchoolFilterDefinitionMapFeature.AuthenticationComputeRule",3,1,3,"学校认证特性"),
                    new MapFeature("SchoolFilterDefinitionMapFeature.CourseSettingComputeRule",2,1,3,"课程设置特性"),
                    new MapFeature("SchoolFilterDefinitionMapFeature.ScoreComputeRule",4,1,3,"评分特性"),
                    new MapFeature("SchoolFilterDefinitionMapFeature.SpecialCourseComputeRule",1,1,3,"特色课程特性"),
                    new MapFeature("SchoolFilterDefinitionMapFeature.TypeComputeRule",5,1,3,"类型特性")
                };
                await _mapFeatureRepository.InsertManyAsync(newSchoolFeatures);
            }



        }

        public async Task<bool> Update(MapFeature mapFeature)
        {
            mapFeature = await _mapFeatureRepository.UpdateAsync(mapFeature);
            bool successFlag = mapFeature != null;
            if (successFlag)
            {
                await _mediator.DispatchDomainEventsAsync(mapFeature);
            }
            return successFlag;
        }
    }
}
