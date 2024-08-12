
using Dapper;
using Kogel.Dapper.Extension.MsSql;
using Sxb.Framework.Foundation.Helper;
using Sxb.WenDa.Common.Entity;
using Sxb.WenDa.Query.SQL;
using Sxb.WenDa.Query.SQL.Repositories;
using Sxb.WenDa.Runner.ImportFromExcel.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sxb.WenDa.Runner.ImportFromExcel
{
    internal class ImportFromExcelRunner : BaseRunner<ImportFromExcelRunner>
    {
        private const int MaxColumnLength = 100;
        private readonly LocalQueryDB _localQueryDB;

        private readonly IQuestionRepository _questionRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICityCategoryRepository _cityCategoryRepository;

        public ImportFromExcelRunner(IQuestionRepository questionRepository, IAnswerRepository answerRepository, ICategoryRepository categoryRepository, ICityCategoryRepository cityCategoryRepository, LocalQueryDB localQueryDB)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
            _answerRepository = answerRepository ?? throw new ArgumentNullException(nameof(answerRepository));
            _categoryRepository = categoryRepository;
            _cityCategoryRepository = cityCategoryRepository;
            _localQueryDB = localQueryDB;
        }

        protected override void Running()
        {
            string fileName = "ImportFromExcel/问答数据导入.xlsx";
            var helper = NPOIHelper.NPOIHelperBuild.GetReader(fileName);


            RunSheetDataAsync(helper.ReadSheet(0, MaxColumnLength), "广州").GetAwaiter().GetResult();
            RunSheetDataAsync(helper.ReadSheet(1, MaxColumnLength), "佛山").GetAwaiter().GetResult();
            RunSheetDataAsync(helper.ReadSheet(2, MaxColumnLength), "深圳").GetAwaiter().GetResult();
        }

        public async Task RunSheetDataAsync(DataTable data, string cityName)
        {
            var importQuestions = Questions(data, cityName).ToList();


            var batchAdd = await ConvertToAsync(importQuestions);
            //await _questionRepository.BatchAddQuestions(batchAdd.Questions, batchAdd.Answers, batchAdd.Tags);

            await new BatchHelper.BatchBuilder<Question>(10000, 80)
                .From(batchAdd.Questions)
                .Handle(async questions =>
                {
                    await _localQueryDB.Connection.CommandSet<Question>().InsertAsync(questions, new[] { "No" });
                })
                .Build()
                .RunAsync();

            await new BatchHelper.BatchBuilder<QuestionAnswer>(10000, 100)
                .From(batchAdd.Answers)
                .Handle(async answers =>
                {
                    await _localQueryDB.Connection.CommandSet<QuestionAnswer>().InsertAsync(answers, new[] { "No" });
                })
                .Build()
                .RunAsync();


            await new BatchHelper.BatchBuilder<QuestionTag>(10000, 100)
                .From(batchAdd.Tags)
                .Handle(async tags =>
                {
                    await _localQueryDB.Connection.CommandSet<QuestionTag>().InsertAsync(tags);
                })
                .Build()
                .RunAsync();
        }

        public async Task<IEnumerable<Guid>> GetInternalUserIds(int top = 10000)
        {
            //var sql = $"select top {top} user_id from iSchoolUser.dbo.talent where IsInternal=1 ";
            var sql = @$"
select  top {top} U.Id from iSchoolUser.dbo.UserInfo U
WHERE channel='1' and NOT EXISTS (
	SELECT 1 from iSchoolUser.dbo.Talent T WHERE T.user_Id = U.Id
)
-- ORDER BY NEWID()
";
            return await _localQueryDB.SlaveConnection.QueryAsync<Guid>(sql, new { });
        }

        public async Task<BatchAdd> ConvertToAsync(List<QuestionImport> importQuestions)
        {
            var internalUserIds = (await GetInternalUserIds()).ToList();
            int internalCount = internalUserIds.Count;
            var categories = (await _categoryRepository.GetAll()).ToList();
            var cities = (await _cityCategoryRepository.GetCitys()).ToList();

            List<Question> questions = new List<Question>();
            List<QuestionAnswer> answers = new List<QuestionAnswer>();
            List<QuestionTag> tags = new List<QuestionTag>();
            foreach (var item in importQuestions)
            {

                var questionId = Guid.NewGuid();
                var cityId = cities.FirstOrDefault(s => s.Name == item.CityName)?.Id ?? 0;
                var plaform = categories.FirstOrDefault(s => s.Depth == 1 && s.Name == item.CategoryName1);
                var category2 = categories.FirstOrDefault(s => s.Depth == 2 && s.Parentid == plaform.Id && s.Name == item.CategoryName2);
                var category3 = categories.FirstOrDefault(s => s.Depth == 3 && s.Parentid == category2.Id && s.Name == item.CategoryName3);
                var tagNames = item.TagNames?.Split(',');

                var categoryId = category2.Id;//not null
                if (category3 != null) categoryId = category3.Id;

                if (category3 != null && tagNames.Length > 0)
                {
                    var itemTags = categories.Where(s =>
                                s.Depth == 4
                                && s.Type == 2
                                && s.Parentid == category3.Id
                                && tagNames.Any(name => name == s.Name)
                            ).Select(s => new QuestionTag()
                            {
                                Qid = questionId,
                                TagId = s.Id
                            }).ToList();
                    tags.AddRange(itemTags);
                }

                var question = new Question();
                questions.Add(question);

                question.Id = questionId;
                question.IsValid = true;

                bool questionTimeFormatSuccess = DateTime.TryParse(item.Time, out DateTime questionTime);
                if (!questionTimeFormatSuccess)
                {
                    var randomS = new Random().Next(0, 3600);
                    var randomM = new Random().Next(1000, 100000);
                    questionTime = DateTime.Now.AddMinutes(-randomM).AddSeconds(-randomS);
                }
                question.CreateTime = questionTime;

                question.UserId = internalUserIds[new Random().Next(0, internalCount)];
                question.ModifyDateTime = question.CreateTime;
                question.Modifier = question.UserId;
                question.LastEditTime = null;
                question.EditCount = 0;
                question.Title = item.Title;
                question.IsAnony = false;
                question.AnonyUserName = null;
                question.SubjectId = null;
                question.Platform = plaform.Id;
                question.City = cityId;
                question.CategoryId = categoryId;
                question.Content = "";
                question.Imgs = null;
                question.Imgs_s = null;
                question.IsRealUser = false;

                //注意回答数量
                question.ReplyCount = item.Answers.Count;

                foreach (var importAnswer in item.Answers)
                {
                    var answer = new QuestionAnswer();
                    answers.Add(answer);
                    answer.Id = Guid.NewGuid();
                    answer.Qid = question.Id;
                    answer.UserId = internalUserIds[new Random().Next(0, internalCount)];

                    DateTime answerTime = DateTime.MinValue;
                    //问题时间格式错误, 随机了时间, 则回答也随机
                    if (!questionTimeFormatSuccess)
                    {
                        var randomS = new Random().Next(0, 3600);
                        answerTime = question.CreateTime.AddSeconds(randomS);
                    }
                    //回答时间格式错误, 随机时间
                    else if (!DateTime.TryParse(importAnswer.Time, out answerTime))
                    {
                        var randomS = new Random().Next(0, 3600);
                        answerTime = question.CreateTime.AddSeconds(randomS);
                    }
                    answer.CreateTime = answerTime;
                    Debug.Assert(answer.CreateTime != DateTime.MinValue);

                    answer.Modifier = answer.UserId;
                    answer.ModifyDateTime = answer.CreateTime;
                    answer.Imgs = null;
                    answer.Imgs_s = null;
                    answer.Content = importAnswer.Content;
                    answer.IsAnony = false;
                    answer.AnonyUserName = null;
                    answer.IsValid = true;
                    answer.ReplyCount = 0;
                }
            }
            return new BatchAdd()
            {
                Questions = questions,
                Answers = answers,
                Tags = tags
            };
        }

        public IEnumerable<QuestionImport> Questions(DataTable table, string cityName)
        {
            var answerContentName = "回答{0}";
            var answerTimeName = "回答时间{0}";
            foreach (DataRow row in table.Rows)
            {
                var import = new QuestionImport()
                {
                    CityName = cityName,
                    CategoryName1 = row["一级分类"] + "",
                    CategoryName2 = row["二级分类"] + "",
                    CategoryName3 = row["三级分类"] + "",
                    TagNames = row["标签"] + "",
                    Url = row["链接"] + "",
                    From = row["来源"] + "",
                    Title = row["问题"] + "",
                    Time = row["问题时间"] + "",
                };

                //回答1	回答时间1
                var answers = new List<AnswerImport>();
                for (int i = 1; i <= 20; i++)
                {
                    var answerContent = row[string.Format(answerContentName, i)] + "";
                    var answerTime = row[string.Format(answerTimeName, i)] + "";
                    //空行后, 跳出
                    if (string.IsNullOrWhiteSpace(answerContent)) break;

                    answers.Add(new AnswerImport()
                    {
                        Content = answerContent,
                        Time = answerTime
                    });
                }
                import.Answers = answers;

                yield return import;
            }
        }
    }
}
