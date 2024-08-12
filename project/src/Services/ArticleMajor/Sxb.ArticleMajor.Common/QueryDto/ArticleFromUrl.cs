using System;

namespace Sxb.ArticleMajor.Common.QueryDto
{
    public class ArticleFromUrl
    {
        public ArticleFromUrl(string fromUrl, string code)
        {
            FromUrl = fromUrl;
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public string FromUrl { get; set; }

        public string Code { get; set; }
    }
}
