using Guide.Common.Infrastructure.Models;
using Guide.Common.Infrastructure.Models.Interfaces;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Guide.Common.Infrastructure.Models
{
    public class Linker : LinkBase
    {
        #region Properties
        public override string TypeName { get; set; } = typeof(Linker).AssemblyQualifiedName;
        #endregion

        #region Methods



        #endregion
    }
}
