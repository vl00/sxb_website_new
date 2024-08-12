using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Sxb.ArticleMajor.API.Utils
{
    public class TemplateUtils
    {
        private string template = string.Empty;

        public string GetTemplate()
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                template = File.ReadAllText("wwwroot/template/article-detail.html");
            }
            return template;
        }


        public async Task<string> BuildAsync(string pageName, string content)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var filename = $"wwwroot/html-files/article-detail/{pageName}.html";
            using var sw = File.CreateText(filename);
            await sw.WriteAsync(GetTemplate().Replace("{{content}}", content));
            sw.Flush();

            stopwatch.Stop();
            Debug.WriteLine("build use time:{0}", stopwatch.ElapsedMilliseconds);
            return filename;
        }

        public static string TestContent = @"

　　中考网整理了关于2021年四川乐山中考考试命题实施意见，希望对同学们有所帮助，仅供参考。

　　一、命题指导思想

　　乐山市2021年初中学业水平考试是义务教育阶段的终结性考试，目的是全面、准确地反映初中毕业生在学科学习目标方面所达到的知识与能力水平，考试结果既是衡量学生是否达到毕业标准的主要依据，又是高中阶段学校招生的重要依据之一。

　　乐山市2021年初中学业水平考试命题工作全面贯彻落实习近平新时代中国特色社会主义思想，彰显社会主义核心价值观，紧紧围绕“培养什么人，怎样培养人，为谁培养人”这一教育根本任务，充分发挥考试的育人功能，使试题和考试有利于全面贯彻国家的教育方针，全面提高教育教学质量；有利于促进基础教育的均衡发展，体现“以学生发展为本”的新课程理念；有利于面向全体学生，体现九年义务教育的性质；有利于改进课堂教学，减轻学生过重的课业负担，促进学生主动学习，达到德、智、体、美等方面全面的发展；有利于课程改革发展，推进素质教育，培养学生创新意识和实践能力；有利于普通高中选拔合格新生。

　　二、命题原则

　　（一）全面落实立德树人要求

　　命题将适应课程改革发展的需求，全面落实立德树人要求。增强试题命制的基础性，考查学生的必备知识和关键能力；增强试题的综合性，体现学生综合素质和学科素养；加强试题命制的应用性，注重理论联系实际；增强试题命制的探究性和开放性，考查学生的创新意识和创新能力。推进素质教育实施，促进学生健康成长和发展。

　　（二）严格遵循命题依据

　　乐山市2021年初中学业水平考试命题严格依据义务教育课程标准及本届学生所学教材命制。

　　（三）试卷结构科学、内容合理

　　1.试卷结构力求简洁、合理，试题数量适当，给学生足够的思考时间；

　　2.不出偏题、怪题和计算、证明繁琐或似是而非的题目。

　　（四）确保公平公正

　　把促进公平公正作为试题命制的基本价值取向，体现地方教学实际，兼顾城镇和农村学生学情。

　　三、考试要求

　　（一）考试形式

　　考试涉及2021届语文、数学、外语、物理、化学、道德与法治、历史七科及2022届地理、生物两科，考试均采用闭卷、笔试形式。其中，语文、数学、外语实行分科命制，物理·化学两科、道德与法治·历史两科、地理·生物两科采用合卷形式命制。

　　语文、数学、英语三科试卷满分均为150分（其中英语含听力考试30分）；物理·化学试卷满分为170分，其中物理90分（含实验操作考试10分），化学80分（含实验操作考试10分）；道德与法治·历史试卷满分为100分，道德与法治、历史各为50分；地理·生物试卷满分为200分，地理、生物各为100分。

　　语文考试时间为150分钟；数学、英语、物理·化学、道德与法治·历史考试时间均为120分钟；地理·生物考试时间为90分钟。

　　（二）考试范围

　　●语文

　　以《义务教育语文课程标准（2011版）》和人教社《义务教育教科书<语文>》(部编版)为命题范围和依据。
";
    }
}
