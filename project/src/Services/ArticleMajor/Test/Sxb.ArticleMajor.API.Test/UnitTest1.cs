using DotNetCore.CAP;
using Moq;
using NUnit.Framework;
using Sxb.ArticleMajor.API.Application.Query;
using Sxb.ArticleMajor.API.Controllers;

namespace Sxb.ArticleMajor.API.Test
{
    public class Tests
    {
        ArticleController controller;

        [SetUp]
        public void Setup()
        {
            var articleQueryMock = new Mock<IArticleQuery>();
            var categoryQueryMock = new Mock<ICategoryQuery>();
            var capMock = new Mock<ICapPublisher>();
            //capMock.Setup(s=>s.)

            //controller = new ArticleController();

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}