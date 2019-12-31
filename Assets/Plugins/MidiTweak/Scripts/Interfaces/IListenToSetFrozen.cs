using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToSetFrozen
    {
        void OnSetFrozen(bool _frozen);
    }
}
