using CommonServiceLocator;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Services.Interfaces;
using Lucene.Net.Documents;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Models
{
    public class Page : LinkBase
    {
        #region Properties
        Section _section = null;
        [XmlIgnore]
        public Section Section
        {
            get
            {
                if (_section == null)
                {
                    var configurationManager = ServiceLocator.
                        Current.GetInstance<IConfigurationManager>();

                    try
                    {
                        _section = configurationManager.CurrentConfiguration.
                            Sections.FirstOrDefault(s => s.Index.ToString() == SectionID);
                    }
                    catch { }
                }
                return _section;
            }
        }

        public override string TypeName { get; set; } = typeof(Page).AssemblyQualifiedName;
        #endregion

        #region Constructors
        public Page() { }
        public Page(Page page) : this()
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

        #region Overrides
        public override string ToString() => Title;
        #endregion

        #endregion
    }
}
