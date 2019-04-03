using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Models.Interfaces
{
    public interface ILinkable
    {
        #region Properties
        string ID { get; set; }
        string Title { get; set; }
        string PageReferenceName { get; set; }
        string Content { get; set; }
        string TypeName { get; set; }
        #endregion

        #region Methods
        void Initialize(Document document);
        Document ToDocument();
        #endregion

    }
}
