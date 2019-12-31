using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToScopeChange
    {
        void OnScopeChange(float _min, float _mid, float _max);
    }
}
