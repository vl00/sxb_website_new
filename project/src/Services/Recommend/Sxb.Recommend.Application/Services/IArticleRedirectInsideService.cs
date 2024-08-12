using Sxb.Recommend.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.Recommend.Application.Services
{
    public interface IArticleRedirectInsideService
    {
        Task Add(ArticleRedirectInside articleRedirectInside);
        Task Add(string shortNo1, string shortNo2);
    }
}
