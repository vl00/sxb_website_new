namespace Sxb.WenDa.Query.ElasticSearch
{
    public class EsIndexConfig
    {
        public static EsIndexConfig DefaultConfig => new ();

        public EsIndices Indices { get; set; } = new EsIndices();


        public class EsIndices
        {
            public EsIndex Question { get; set; } = new EsIndex("wendaquestionindex", "wendaquestion");
            public EsIndex Answer { get; set; } = new EsIndex("wendaanswerindex", "wendaanswer");
            public EsIndex Subject { get; set; } = new EsIndex("wendasubjectindex", "wendasubject");
        }

        public class EsIndex
        {
            private string prefix { get; set; } = "";
            private string suffix { get; set; } = "";

            public EsIndex() { }
            public EsIndex(string name, string alias)
            {
                Name = WrapIndex(name ?? throw new ArgumentNullException(nameof(name)));
                Alias = WrapIndex(alias ?? throw new ArgumentNullException(nameof(alias)));
            }

            public string Name { get; private set; }
            public string Alias { get; private set; }

            public string WrapIndex(string name)
            {
                return prefix + name + suffix;
            }

            public string SearchIndex
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(Alias) ? Alias : Name;
                }
            }

            public bool Contains(string indexName)
            {
                return indexName != null &&
                    (indexName == Name || indexName == Alias);
            }

            public override string ToString()
            {
                return SearchIndex;
            }
        }
    }

}