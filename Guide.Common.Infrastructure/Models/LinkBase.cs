using Guide.Common.Infrastructure.Models.Interfaces;
using Guide.Common.Infrastructure.Resources.Converters;
using Lucene.Net.Documents;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Guide.Common.Infrastructure.Models
{
    public abstract class LinkBase : DependencyObject, INotifyPropertyChanged, ILinkable
    {
        #region Properties
        [XmlIgnore]
        public virtual Type PageReference { get; protected set; }
        public virtual string PageReferenceName
        {
            get => PageReference.AssemblyQualifiedName;
            set => PageReference = Type.GetType(value);
        }

        public virtual string ID { get; set; }
        public virtual string SectionID { get; set; }

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(LinkBase), new PropertyMetadata(string.Empty));


        [XmlIgnore]
        public virtual string Content { get; set; }

        [TypeConverter(typeof(StringToArrayConverter))]
        public virtual string[] Tags { get; set; }

        public abstract string TypeName { get; set; }

        #region Events
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #endregion

        #region Methods

        #region INotifyPropertyChanged Implementation
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            //TODO: when we remove the old OnPropertyChanged method we need to uncomment the below line
            //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
#pragma warning disable CS0618 // Type or member is obsolete
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
		/// Checks if a property already matches a desired value. Sets the property and
		/// notifies listeners only when necessary.
		/// </summary>
		/// <typeparam name="T">Type of the property.</typeparam>
		/// <param name="storage">Reference to a property with both getter and setter.</param>
		/// <param name="value">Desired value for the property.</param>
		/// <param name="propertyName">Name of the property used to notify listeners. This
		/// value is optional and can be provided automatically when invoked from compilers that
		/// support CallerMemberName.</param>
		/// <returns>True if the value was changed, false if the existing value matched the
		/// desired value.</returns>
		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaisePropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <param name="onChanged">Action that is called after the property value has been changed.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, Action onChanged, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            onChanged?.Invoke();
            RaisePropertyChanged(propertyName);

            return true;
        }
        #endregion

        #region ILinkable Implementation
        public virtual void Initialize(Document document)
        {
            ID = document.Get(nameof(ID));
            SectionID = document.Get(nameof(SectionID));
            PageReferenceName = document.Get(nameof(PageReferenceName));
            Title = document.Get(nameof(Title));
            Content = document.Get(nameof(Content));
        }

        public virtual Document ToDocument()
        {
            Document document = new Document();
            document.Add(new Field(nameof(ID), ID, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(nameof(Title), Title, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field(nameof(PageReferenceName), PageReferenceName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(nameof(Content), Content, Field.Store.YES, Field.Index.ANALYZED));
            document.Add(new Field(nameof(TypeName), TypeName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            return document;
        }
        #endregion

        #endregion
    }
}
