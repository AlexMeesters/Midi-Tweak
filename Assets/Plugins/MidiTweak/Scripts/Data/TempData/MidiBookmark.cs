using UnityEngine;
using System.Collections;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// A midi bookmark links to the corresponding field data on a component.
    /// It is created when the game starts.
    /// </summary>

    [System.Serializable]
    public class MidiBookmark
    {
        private int knobID;
        public int KnobID { get { return knobID; } }

        private Component component;
        public Component Component { get { return component; } }

        private FieldData field;
        public FieldData Field { get { return field; } }

        public MidiBookmark(int _knobID, Component _component, FieldData _fieldData, float _value)
        {
            knobID = _knobID;
            component = _component;
            field = _fieldData;
        }
    }
}