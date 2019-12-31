using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToValueChange
    {
        void OnValueChange(float _value);
    }
}
