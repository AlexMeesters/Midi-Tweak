using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToParameterLoad
    {
        void OnLoadParameters(MidiBookmark[] _paramters);
    }
}
