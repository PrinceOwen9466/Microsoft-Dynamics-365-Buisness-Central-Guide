using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Models
{
    public class Page
    {
        #region Properties
        [XmlIgnore]
        public Type PageReference { get; private set; }
        public string PageReferenceName
        {
            get => PageReference.AssemblyQualifiedName;
            set => PageReference = Type.GetType(value);
        }
        public string ID { get; set; }
        public string SectionID { get; set; }
        public string Title { get; set; }

        [XmlIgnore]
        public string Content { get; set; }
        #endregion

        #region Constructors
        public Page() { }
        public Page(Page page)
        {
            ID = page.ID;
            SectionID = page.SectionID;
            Title = page.Title;
            Content = page.Content;
            PageReferenceName = page.PageReferenceName;
        }
        #endregion



        #region Methods
        public void ConcatenateIDs(params string[] idHistory) => ID = string.Join("-", idHistory);
        public IEnumerable<string> IterateIDs() => ID.Split('-');
        #endregion
    }
}
