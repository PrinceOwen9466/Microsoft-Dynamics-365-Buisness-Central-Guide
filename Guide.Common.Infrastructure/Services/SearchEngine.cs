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
using System.IO;
using Lucene.Net.Analysis.Tokenattributes;
using Guide.Common.Infrastructure.Models.Interfaces;
using System.Windows;

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
        public ObservableCollection<ILinkable> Results { get; } = new ObservableCollection<ILinkable>();

        #region Internals
        System.IO.DirectoryInfo IndexDirectory { get; } 
            = new System.IO.DirectoryInfo(Core.SEARCH_INDEX_DIR);
        Analyzer Analyzer { get; }
        MMapDirectory MapDirectory { get; }
        IndexSearcher Searcher { get; set; }
        QueryParser Parser { get; set; }
        #endregion

        #endregion

        #region Constructors
        public SearchEngine()
        {
            MapDirectory = new MMapDirectory(IndexDirectory);
            Analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            Parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Content", Analyzer);
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
                    document.Add(new Field("title", page.Title, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("pageReference", page.PageReferenceName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("content", page.Content, Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(document);
                }
        }

        public void GenerateIndex(IEnumerable<ILinkable> linkables)
        {
            Core.ClearDirectory(Core.SEARCH_INDEX_DIR);
            using (IndexWriter writer = new IndexWriter(MapDirectory, Analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                foreach (var linkable in linkables)
                    writer.AddDocument(linkable.ToDocument());
        }
        #endregion

        void SearchIndexes()
        {
            if (Searcher == null) Searcher = new IndexSearcher(MapDirectory);

            Results.Clear();
            if (Search.Length <= 0) return;

            Query query = Parser.Parse(Search);
            
            TopDocs docs = Searcher.Search(query, 100);
            var hits = docs.ScoreDocs;
            

            Core.Log.Debug($"Searching!!! {docs.TotalHits}");

            /*
            using (var reader = new StringReader(Search))
            using (var tokenStream = Analyzer.TokenStream("content", reader))
            {
                tokenStream.Reset();
                var termAttrib = tokenStream.AddAttribute<ITermAttribute>();
                while (tokenStream.IncrementToken())
                {
                    string term = termAttrib.Term;
                    Core.Log.Debug(term);
                }
                tokenStream.End();
            }
            */
            foreach (var hit in hits)
            {
                var linkable = ResolveLinkable(Searcher.Doc(hit.Doc));
                Results.Add(linkable);
            }

            /*
            foreach (var hit in hits)
            {
                var doc = Searcher.Doc(hit.Doc);
                Page page = new Page();
                page.ID = doc.Get("id");
                page.Title = doc.Get("title");
                page.PageReferenceName = doc.Get("pageReference");
                Results.Add(page);
                Core.Log.Debug($"Found {page.ID}!");
            }
            */
            RaisePropertyChanged(nameof(Results));
        }


        ILinkable ResolveLinkable(Document document)
        {
            Type type = Type.GetType(document.Get(nameof(ILinkable.TypeName)));
            ILinkable instance = Activator.CreateInstance(type) as ILinkable;
            instance.Initialize(document);
            return instance;
        }
        #endregion
    }
}
