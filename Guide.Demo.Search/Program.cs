using Guide.Demo.Search.Extensions;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Demo.Search
{
    class Program
    {
        #region Properties
        static DirectoryInfo INDEX_DIR = new DirectoryInfo(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Index"));
        static string DOCUMENT_DIR = @"C:\NVIDIA\DisplayDriver\335.23\Win8_WinVista_Win7_64\International";
        #endregion

        #region Methods
        static void Main(string[] args)
        {
            // IndexFiles();
            // Search("NVIDIA");
        }

        static void IndexFiles()
        {
            using (MMapDirectory mapDirectory = new MMapDirectory(INDEX_DIR))
            using (IndexWriter writer = new IndexWriter(mapDirectory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED))
                foreach (string file in DirectoryHelper.FindAccessibleFiles(DOCUMENT_DIR, ".txt", true, null, null))
                {
                    FileInfo f = new FileInfo(file);
                    Document document = new Document();
                    document.Add(new Field("path", f.FullName, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    document.Add(new Field("modified", DateTools.TimeToString(f.LastAccessTime.Ticks, DateTools.Resolution.MINUTE), Field.Store.YES, Field.Index.NOT_ANALYZED));

                    using (var reader = new StreamReader(f.FullName))
                        document.Add(new Field("contents", reader.ReadToEnd(), Field.Store.YES, Field.Index.ANALYZED));
                    writer.AddDocument(document);
                }
        }

        static void Search(string term)
        {
            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using (MMapDirectory mapDirectory = new MMapDirectory(INDEX_DIR))
            using (IndexSearcher searcher = new IndexSearcher(mapDirectory))
            {
                Console.Out.Write("Query: ");

                if (term.Length <= 0) return;

                QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "contents", analyzer);
                Query query = parser.Parse(term);
                Console.Out.WriteLine("Searching for: " + query.ToString("contents"));
                var hits = searcher.Search(query, 100).ScoreDocs;

                foreach (var hit in hits)
                {
                    var doc = searcher.Doc(hit.Doc);
                    string content = doc.Get("contents");
                    Console.WriteLine(content);
                }
            }

        }
        #endregion
    }
}
