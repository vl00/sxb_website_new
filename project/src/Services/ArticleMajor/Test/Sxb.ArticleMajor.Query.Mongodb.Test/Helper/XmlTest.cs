using Newtonsoft.Json;
using Sxb.ArticleMajor.Common.MongoEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Sxb.ArticleMajor.Query.Mongodb.Test
{
    public class XmlHelper
    {
        public bool Trim { get; set; }

        readonly string[] IgnoreNodeNames = new string[] { "地方站点", "工具", "升学服务" };
        readonly string[] IgnoreLikeNodeNames = new string[] { "院校库" };


        public XmlHelper(bool trim)
        {
            Trim = trim;
        }

        public string RemoveNamespace(string filename)
        {
            //var text = "<xmap-content xmlns=\"urn:xmind:xmap:xmlns:content:2.0\" > <ssss>";
            var text = File.ReadAllText(filename);
            var newText = Regex.Replace(text, "<xmap-content[^>]*>", "<xmap-content>");
            //File.WriteAllText(filename, newText);
            return newText;
        }

        public List<Site> TransferXml()
        {
            string file = "content.xml";
            var text = RemoveNamespace(file);

            //XDocument document = XDocument.Load(file);
            XDocument document = XDocument.Parse(text);

            var topic = document.Root.Element("sheet").Element("topic");
            var title = topic.Element("title");
            var children = topic.Element("children");
            var topics = children.Elements("topics");

            var sites = topics.Elements("topic").Select(topicEl =>
            {
                var site = new Site();
                site.Id = topicEl.Attribute("id")?.Value;
                site.Title = topicEl.Element("title")?.Value;
                if (Trim)
                {
                    site.Id = site.Id?.Trim();
                    site.Title = site.Title?.Trim();
                }
                site.Children = GetChildren(topicEl);
                return site;
            }).ToList();
            return sites;
        }

        public List<Topic> GetChildren(XElement xElement)
        {
            var childrenEl = xElement.Element("children");
            if (childrenEl != null)
            {
                return childrenEl.Element("topics").Elements("topic")
                   .Where(topicEl =>
                   {
                       //排除@开头的注释
                       var title = topicEl.Element("title")?.Value;
                       if (title == null)
                       {
                           return false;
                       }
                       if (Trim)
                       {
                           title = title?.Trim();
                       }
                       //排除忽略的节点名称
                       if (IgnoreNodeNames.Contains(title))
                       {
                           return false;
                       }
                       if (IgnoreLikeNodeNames.Any(s=> title.IndexOf(s) != -1))
                       {
                           return false;
                       }
                       
                       return !title.StartsWith('@');
                   })
                   .SelectMany(topicEl =>
                   {
                       var id = topicEl.Attribute("id")?.Value;
                       var title = topicEl.Element("title")?.Value;
                       if (Trim)
                       {
                           id = id?.Trim();
                           title = title?.Trim();
                       }
                       if (title.Contains('#'))
                       {
                           //中考复习#图文
                           //移除#后的注释
                           title = Regex.Replace(title, "#.*", "");
                       }

                       //竖线分割为多个
                       if (title.Contains('|'))
                       {
                           return title.Split('|').Select(s => new Topic
                           {
                               Title = Trim ? (s?.Trim()) : s
                           });
                       }

                       return new List<Topic>()
                       {
                            new Topic
                            {
                                Id = id,
                                Title = title,
                                Children = GetChildren(topicEl)
                            }
                        };
                   }).ToList();
            }
            return null;
        }

        public class Site
        {
            public string Id { get; set; }
            public string Title { get; set; }

            public List<Topic> Children { get; set; }
        }


        public class Topic
        {
            private string title;

            public string Id { get; set; }
            public string Title { get => title; set => title = value; }

            public List<Topic> Children { get; set; }
        }
    }
}
