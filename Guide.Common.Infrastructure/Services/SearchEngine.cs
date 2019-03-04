using Guide.Common.Infrastructure.Services.Interfaces;
using Guide.Common.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace Guide.Common.Infrastructure.Services
{
    public class SearchEngine : BindableBase, ISearchEngine
    {
        #region Properties

        string _search = string.Empty;
        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                SearchIndexes();
            }
        }
        public ObservableCollection<Page> Results { get; } = new ObservableCollection<Page>();

        #region Internals
        System.IO.DirectoryInfo IndexDirectory { get; } 
            = new System.IO.DirectoryInfo(Core.SEARCH_INDEX_DIR);
        Analyzer Analyzer { get; }
        MMapDirectory MapDirectory { get; }
        IndexSearcher Searcher { get; }
        QueryParser Parser { get; set; }
        #endregion

        #endregion

        #region Constructors
        public SearchEngine()
        {
            MapDirectory = new MMapDirectory(IndexDirectory);
            Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Searcher = new IndexSearcher(MapDirectory);
            Parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "content", Analyzer);
        }
        #endregion

        #region Methods

        #region ISearchEngine Implemenation
        public void GenerateIndex(IEnumerable<Page> pages)
        {
            Core.ClearDirectory(Core.SEARCH_INDEX_DIR);
            using (IndexWriter writer = new IndexWriter(MapDirectory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                foreach (var page in pages)
                {
                    Document document = new Document();
                    document.Add(new Field("id", page.ID, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("pageReference", page.PageReferenceName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("content", page.Content, Field.Store.YES, Field.Index.ANALYZED));
                }
        }
        #endregion

        void SearchIndexes()
        {
            if (Search.Length <= 0) return;

            Query query = Parser.Parse(Search);
            var hits = Searcher.Search(query, 100).ScoreDocs;

            Results.Clear();
            foreach (var hit in hits)
            {
                var doc = Searcher.Doc(hit.Doc);
                Page page = new Page();
                page.ID = doc.Get("id");
                page.PageReferenceName = doc.Get("pageReference");
                Results.Add(page);
            }
            RaisePropertyChanged(nameof(Results));
        }

        #endregion
    }
}
