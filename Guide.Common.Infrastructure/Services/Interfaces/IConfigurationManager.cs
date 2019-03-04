using Guide.Common.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Services.Interfaces
{
    public interface IConfigurationManager
    {

        #region Properties
        Configuration CurrentConfiguration { get; }
        #endregion
    }
}
