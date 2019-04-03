using CommonServiceLocator;
using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Models
{
    public class Section : ISection
    {
        #region Properties
        public string Name { get; set; } = string.Empty;
        public int Index { get; set; }
        public string Description { get; set; }
        [XmlIgnore]
        public ObservableCollection<UIReference> Summary { get; } = new ObservableCollection<UIReference>();
        [XmlIgnore]
        public ObservableCollection<PresentationControl> Pages { get; } = new ObservableCollection<PresentationControl>();
        public int ContentCount => PageMap.Count;

        [XmlIgnore]
        public List<Reference> PageMap { get; set; } = new List<Reference>();


        public List<Page> PageList { get; set; } = new List<Page>();
        public int TotalPages => PageList.Count;


        #region Bindables
        /*
        public PresentationControl CurrentPage { get; set; }
        public int PageIndex { get; set; }
        */
        #endregion

        #endregion

        #region Constructors
        public Section() { }
        public Section(Section section)
        {
            Name = section.Name;
            Index = section.Index;
            Description = section.Description;

            foreach (var @ref in section.Summary)
                Summary.Add(new UIReference(@ref));

            foreach (var page in section.Pages)
                Pages.Add(page);

            foreach (var @ref in section.PageMap)
                PageMap.Add(new Reference(@ref));

            foreach (var page in section.PageList)
                PageList.Add(new Page(page));
        }
        #endregion

        #region Methods
        public void Initialize()
        {

        }


        #region Equity and Comparison
        static bool CompareSections(Section a, Section b)
        {
            bool aIsNull = object.ReferenceEquals(a, null);
            bool bIsNull = object.ReferenceEquals(b, null);

            if (aIsNull && bIsNull) return true;
            else if (aIsNull || bIsNull) return false;

            return a.Name.ToLower() == b.Name.ToLower();
        }
        #endregion

        #region Overrides
        public override bool Equals(object obj)
        {
            if (!(obj is Section)) return false;
            return CompareSections(this, (Section)obj);
        }

        public static bool operator ==(Section a, Section b) => CompareSections(a, b);
        public static bool operator !=(Section a, Section b) => !CompareSections(a, b);

        public override int GetHashCode() => Name.GetHashCode();

        public override string ToString() => Name;
        #endregion


        #endregion
    }
}
