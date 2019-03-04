using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Demos.Presentation.Models
{
    public class Page : PresentableBase
    {
        #region Properties

        string _content = string.Empty;
        public string Content { get => _content; private set => SetProperty(ref _content, value); }

        #endregion

        #region Constructors
        public Page(string content)
        {
            Content = content;
        }
        #endregion

        #region Methods
        #endregion
    }
}
