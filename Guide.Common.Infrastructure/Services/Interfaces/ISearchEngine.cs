using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guide.Common.Infrastructure.Models;

namespace Guide.Common.Infrastructure.Services.Interfaces
{
    public interface ISearchEngine
    {
        string Search { get; set; }
        ObservableCollection<Page> Results { get; }
        void GenerateIndex(IEnumerable<Page> pages);
    }
}
