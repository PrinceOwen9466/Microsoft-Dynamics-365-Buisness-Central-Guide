using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Models.Interfaces
{
    public interface ISection
    { 
        int Index { get; }
        string Name { get; }
        string Description { get; }
        List<Reference> PageMap { get; }
        int TotalPages { get; }
    }
}
