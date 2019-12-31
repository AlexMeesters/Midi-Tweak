using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToZoomChange
    {
        void OnZoomChange(float _value);
    }
}
