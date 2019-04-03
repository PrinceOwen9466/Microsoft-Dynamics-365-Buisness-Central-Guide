using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Containers;
using Lucene.Net.Documents;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Guide.Common.Infrastructure.Models
{
    public class MediaLink : LinkBase
    {
        #region Properties
        public string ResourceKey { get; set; }
        public override string TypeName { get; set; } = typeof(MediaLink).AssemblyQualifiedName;

        public BitmapSource Thumbnail => Source.ThumbnailSource;


        #region Internals
        MediaSource Source { get; set; }
        #endregion

        #endregion

        #region Methods

        #region Overrides
        public override void Initialize(Document document)
        {
            base.Initialize(document);
            /*
            ResourceKey = document.Get(nameof(ResourceKey));

            Source = (MediaSource)Application.Current.TryFindResource(ResourceKey);
            if (Source == null) throw new InvalidOperationException("Media Source could not be located");
            
            Source.ThumbnailLoaded += (s, e) => RaisePropertyChanged(nameof(Thumbnail));
            await Source.LoadThumbnail();
            */
        }

        public override Document ToDocument()
        {
            Document doc = base.ToDocument();
            doc.Add(new Field(nameof(ResourceKey), ResourceKey, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }
        #endregion

        #endregion

    }
}
