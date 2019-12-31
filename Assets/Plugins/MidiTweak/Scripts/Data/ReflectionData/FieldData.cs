
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    [System.Serializable]
    public class FieldData
    {
        public List<string> parentFields = new List<string>();
        public string fieldName;
        public System.Type fieldType;
        public int assignedMidiInput = 0;
    }
}