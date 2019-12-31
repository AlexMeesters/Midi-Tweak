

namespace Lowscope.Miditweak
{
#if UNITY_EDITOR

    /// <summary>
    /// Is used to assign midi fields within MidiTweak_Tab_Options.
    /// The indexes are based on multiple arrays (SaveData, Reflection and Field)
    /// </summary>

    public class MidiAssignmentRequest
    {
        private int saveDataIndex;
        private int reflectionDataIndex;
        private int fieldIndex;

        // Save data index corresponds to 

        public MidiAssignmentRequest(int _saveDataIndex, int _reflectionIndex, int _fieldIndex)
        {
            saveDataIndex = _saveDataIndex;
            reflectionDataIndex = _reflectionIndex;
            fieldIndex = _fieldIndex;
        }

        public bool CheckForMatch(int _objectIndex, int _reflectionIndex, int _fieldIndex)
        {
            return (_objectIndex == saveDataIndex && _reflectionIndex == reflectionDataIndex && _fieldIndex == fieldIndex);
        }
    }
#endif
}