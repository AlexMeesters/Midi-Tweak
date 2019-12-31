using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface IListenToParameterChange
    {
        void OnParameterChange(string _gameObject, string _name, int _index, int _midiIndex);
    }
}
