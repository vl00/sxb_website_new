using MediatR;
using Sxb.Recommend.Infrastructure.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sxb.Infrastructure.Core.Extensions;
using Sxb.Recommend.Domain.Entity;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace Sxb.Recommend.Application.Services
{
    public class SchoolRedirectFrequencyService : ISchoolRedirectFrequencyService
    {
        ISchoolRedirectFrequencyRepository _repository;
        IMediator _mediator;
        ILogger<SchoolRedirectFrequencyService> _logger;
        IHostEnvironment _environment;
        IMemoryCache _memoryCache;
        public SchoolRedirectFrequencyService(ISchoolRedirectFrequencyRepository repository
            , IMediator mediator
            , ILogger<SchoolRedirectFrequencyService> logger
            , IHostEnvironment environment
            , IMemoryCache memoryCache)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
            _environment = environment;
            _memoryCache = memoryCache;
        }
        public async Task FlushFrequency()
        {
            int offset = 0, limit = 1000;
            var frequencies = await _repository.QueryFrequenciesAsync(offset, limit);
            while (frequencies.Any())
            {
                foreach (var frequency in frequencies)
                {
                    _logger.LogDebug($"正在刷新：{frequency.SIdP},{frequency.SIdS},{frequency.OpenTime}");
                    var existsFrequency = await _repository.QueryFrequencyAsync(frequency.SIdP, frequency.SIdS);

                    if (existsFrequency != null)
                    {
                        await _repository.DeleteAsync(existsFrequency.Id);
                    }
                    await _repository.AddAsync(frequency);
                    if (existsFrequency?.OpenTime != frequency.OpenTime)
                    {
                        _logger.LogDebug($"检测到打开次数变更：{frequency.SIdP},{frequency.SIdS},{existsFrequency?.OpenTime}-->{frequency.OpenTime}");
                        frequency.OpenTimeIsChange();
                        await _mediator.DispatchDomainEventsAsync(frequency);
                    }

                }
                offset += limit;
                frequencies = await _repository.QueryFrequenciesAsync(offset, limit);
            }

        }

        public async Task FlushFrequencyToLocal()
        {
            int offset = 0, limit = 1000;
            var frequencies = await _repository.QueryFrequenciesAsync(offset, limit);
            var now = DateTime.Now;
            string path = Path.Combine("Data", $"schoolfrequencies_{_environment.EnvironmentName}_{now.ToString("yyyy-MM-dd")}.csv");
            string prepath = Path.Combine("Data", $"schoolfrequencies_{_environment.EnvironmentName}_{now.AddDays(-1).ToString("yyyy-MM-dd")}.csv");
            if (File.Exists(path))
            {
                _logger.LogInformation($"已经存在今天的刷新结果，若需要强制刷新，请删除：{path}");
                return;
            }
            using (StreamWriter sw = new StreamWriter(path))
            {
                while (frequencies.Any())
                {
                    foreach (var frequency in frequencies)
                    {
                        _logger.LogDebug($"正在刷新：{frequency.SIdP},{frequency.SIdS},{frequency.OpenTime}");
                        var existsFrequency = ReadFrequencies(prepath).FirstOrDefault(s => s.SIdP == frequency.SIdP && s.SIdS == frequency.SIdS);
                        await sw.WriteLineAsync(frequency.ToCSV());
                        if (existsFrequency?.OpenTime != frequency.OpenTime)
                        {
                            _logger.LogDebug($"检测到打开次数变更：{frequency.SIdP},{frequency.SIdS},{existsFrequency?.OpenTime}-->{frequency.OpenTime}");
                            frequency.OpenTimeIsChange();
                            await _mediator.DispatchDomainEventsAsync(frequency);
                        }

                    }
                    offset += limit;
                    frequencies = await _repository.QueryFrequenciesAsync(offset, limit);
                }

            }
            if (File.Exists(prepath))
            {
                File.Delete(prepath);
            }
        }


        public async Task<SchoolRedirectFrequency> GetFrequency(Guid sidp, Guid sids)
        {
            var data = await  _memoryCache.GetOrCreateAsync($"frequency_{sidp}",async (entry) =>
            {
                entry.SetSize(0).SetSlidingExpiration(TimeSpan.FromSeconds(5));
                return await _repository.QueryFrequenciesAsync(sidp);
            });
            return data.FirstOrDefault(s => s.SIdS == sids);
        }

        public async Task<IEnumerable<SchoolRedirectFrequency>> GetFrequenciesAsync(Guid sidp)
        {
            var data = await _memoryCache.GetOrCreateAsync($"frequency_{sidp}", async (entry) =>
            {
                entry.SetSize(0).SetSlidingExpiration(TimeSpan.FromSeconds(5));
                return await _repository.QueryFrequenciesAsync(sidp);
            });
            return data;
        }

        public async Task<SchoolRedirectFrequency> QueryFrequencyAsync(Guid sidp, Guid sids)
        {
            var frequency = await _repository.QueryFrequencyAsync(sidp, sids);
            return frequency;

        }
        public IEnumerable<SchoolRedirectFrequency> ReadFrequencies()
        {
            string path = Path.Combine("Data", $"schoolfrequencies_{_environment.EnvironmentName}_{DateTime.Now.ToString("yyyy-MM-dd")}.csv");
            return ReadFrequencies(path);
        }
        IEnumerable<SchoolRedirectFrequency> ReadFrequencies(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        yield return new SchoolRedirectFrequency(line);
                    }
                }
            }

        }
    }
}
