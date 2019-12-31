using UnityEngine;
using System.Collections;

/// <summary>
/// Base plugin configuration
/// </summary>

namespace Lowscope.Miditweak
{
    [CreateAssetMenu(fileName = "DataSettings", menuName = "Miditweak/DataSettings")]
    public class DataSettings : ScriptableObject
    {
        public bool DebugMode;
        public bool DebugDisplayMidiInputs;
        public int BaseZoomRange = 1000;
        public int ZoomIncrementAmount = 25;
        public bool ResetToDefaultZoomOnFreeze = true;
        public bool RequireRecenterOnFreeze = true;
        public bool LimitInputToMidiAssignments;
        public EMidiActivationTolerance midiActivationTolerance = EMidiActivationTolerance.High;
        public bool KeepChangesAfterSceneLoad = true;
        public bool SaveSceneOnMidiAssignment = false;
        public bool displayInGameTweakMenuOnPlay = false;
        public bool instantiateInGameTweakMenuOnPlay = false;

        public int midiActivationToleranceValue
        {
            get
            {
                switch (midiActivationTolerance)
                {
                    case EMidiActivationTolerance.Instant: return 0;
                    case EMidiActivationTolerance.High: return 10;
                    case EMidiActivationTolerance.Low: return 25;
                    default:
                        return 25;
                }
            }
        }
    }
}
