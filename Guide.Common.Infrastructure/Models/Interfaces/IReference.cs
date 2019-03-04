using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Models.Interfaces
{
    public interface IReference
    {
        Section TargetSection { get; }
        Type TargetPage { get; }
        string TargetPageName { get; }
        int TargetPageIndex { get; }
    }
}
