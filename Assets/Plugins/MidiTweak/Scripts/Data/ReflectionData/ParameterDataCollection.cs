using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Holds an collection of parameter data for a specific game object.
    /// This is the data that is displayed once you have selected an game object within the midi tweak editor interface.
    /// </summary>

    [System.Serializable]
    public class ParameterDataCollection
    {
        public GameObject targetGameobject;
        public List<ParameterData> data = new List<ParameterData>();
    }
}