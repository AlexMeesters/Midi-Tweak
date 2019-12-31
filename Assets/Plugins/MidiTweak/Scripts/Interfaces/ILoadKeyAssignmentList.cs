using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    public interface ILoadKeyAssignmentList
    {
        void OnKeyAssignmentsLoaded(KeyBinding[] _bindingData);
    }
}
