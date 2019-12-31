using UnityEngine;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Reflection data storage which is retrieved from an component.
    /// </summary>

    [System.Serializable]
    public class ParameterData
    {
        public Component component;
        public int componentid;
        public List<FieldData> fieldData = new List<FieldData>();
    }
}