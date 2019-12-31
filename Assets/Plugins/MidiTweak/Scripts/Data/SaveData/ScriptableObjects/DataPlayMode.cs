using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Used to store changes that have been made during play mode.
    /// Data will be reset upon starting a new play mode.
    /// </summary>

    [CreateAssetMenu(fileName = "DataPlayMode", menuName = "Miditweak/DataPlayMode")]
    public class DataPlayMode : ScriptableObject
    {
        public MidiTweak.RuntimeSettings settings;

        public void OnEnable()
        {
            hideFlags = HideFlags.DontUnloadUnusedAsset;
        }

        [System.Serializable]
        public class Data
        {
            public int saveIndex;
            public int reflectionIndex;
            public int fieldIndex;
            public float value;

            public Data(int _saveIndex, int _reflectionIndex, int _fieldIndex, float _value)
            {
                saveIndex = _saveIndex;
                reflectionIndex = _reflectionIndex;
                fieldIndex = _fieldIndex;
                value = _value;
            }
        }

        public List<Data> data = new List<Data>();

        public void AddData (int _saveIndex, int _reflectionIndex, int _fieldIndex)
        {
            data.Add (new Data(_saveIndex, _reflectionIndex, _fieldIndex, 0));
        }
    }
}