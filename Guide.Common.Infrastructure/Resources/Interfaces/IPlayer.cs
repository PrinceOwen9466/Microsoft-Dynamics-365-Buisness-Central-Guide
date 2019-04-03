using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guide.Common.Infrastructure.Resources.Interfaces
{
    public interface IPlayer
    {
        #region Properties
        bool IsPlaying { get; }
        #endregion

        #region Methods
        void Play();
        void Pause();
        void Stop(bool deconstruct);
        #endregion
    }
}
