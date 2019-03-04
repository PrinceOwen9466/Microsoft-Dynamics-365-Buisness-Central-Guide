using Guide.Common.Infrastructure.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Models
{
    public class Reference : IReference
    {
        #region Properties

        #region IReference Implementation
        public Section TargetSection { get; set; }

        [XmlIgnore]
        public Type TargetPage { get; set; }
        
        public string TargetPageAssemblyName
        {
            get
            {
                if (TargetPage == null) return string.Empty;
                return TargetPage.AssemblyQualifiedName;
            }
            set => TargetPage = Type.GetType(value);
        }

        public int TargetPageIndex { get; set; }
        public string TargetPageName { get; set; }
        public bool IsSecton { get; set; }
        #endregion

        #endregion

        #region Constructors
        public Reference() { }
        public Reference(Section targetSection, Type targetPage, bool isSection = false)
        {
            TargetSection = targetSection;
            TargetPage = targetPage;
            IsSecton = isSection;
        }

        public Reference(Section targetSection, Type targetPage, string targetPageName, bool isSection = false)
        {
            TargetSection = targetSection;
            TargetPage = targetPage;
            TargetPageName = targetPageName;
            IsSecton = isSection;
        }

        public Reference(Reference reference)
        {
            TargetSection = reference.TargetSection;
            TargetPage = reference.TargetPage;
            TargetPageIndex = reference.TargetPageIndex;
            TargetPageName = reference.TargetPageName;
            IsSecton = reference.IsSecton;
        }
        #endregion

        #region Methods
        #endregion
    }
}
