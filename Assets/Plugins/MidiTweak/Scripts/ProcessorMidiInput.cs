using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MidiJack;


namespace Lowscope.Miditweak
{
    [RequireComponent(typeof(MidiTweak)), AddComponentMenu("")]
    public class ProcessorMidiInput : MonoBehaviour
    {
        private MidiTweak midiTweak;

        private List<MidiBookmark> bookMarks;

        private int framesRequiredForMidiSwitch = 25;

        private int lastNote = -1;

        private void Start()
        {
            midiTweak = GetComponent<MidiTweak>();
            bookMarks = midiTweak.ParameterBookMarks;
        }

        private void OnEnable()
        {
            MidiMaster.knobDelegate += ProcessMidiNotifications;
            MidiMaster.noteOnDelegate += ProcessMidiNotifications;
        }

        private void OnDisable()
        {
            MidiMaster.knobDelegate -= ProcessMidiNotifications;
            MidiMaster.noteOnDelegate -= ProcessMidiNotifications;
        }

        private bool IsInputBoundToBookMark(int _note, out int _index)
        {
            _index = -1;

            for (int paramIndex = 0; paramIndex < midiTweak.ParameterBookMarks.Count; paramIndex++)
            {
                if (bookMarks[paramIndex].KnobID == _note)
                {
                    _index = paramIndex;
                    return true;
                }
            }

            return false;
        }

        private void SwitchParameterWithTolerance(int _note, int _bookMark)
        {
            if (HasMetMidiSwitchTolerance(_note))
            {
                midiTweak.SetActiveParameter(_bookMark, _note);
            }
        }

        private void ProcessMidiNotifications(MidiChannel channel, int note, float velocity)
        {
            // This is to prevent any button press recognition.
            if (velocity == 1 || velocity == 0)
            {
                return;
            }

            MidiBookmark getActiveBookMark = midiTweak.GetActiveBookmark();

            if (getActiveBookMark.KnobID != note)
            {
                int getBookMarkIndex = -1;

                //  Switch the parameter to the bookmark it is bound to.
                if (IsInputBoundToBookMark(note, out getBookMarkIndex))
                {
                    SwitchParameterWithTolerance(note, getBookMarkIndex);
                    return;
                }
            }

            if (midiTweak.ConfigSettings.LimitInputToMidiAssignments && getActiveBookMark.KnobID != note)
                return;

            midiTweak.SetValueByPercentage(velocity);
        }

        private bool HasMetMidiSwitchTolerance(int note)
        {
            // Ensure that it is impossible to up the switch tolerance by moving different knobs.
            if (note != lastNote)
            {
                framesRequiredForMidiSwitch = midiTweak.ConfigSettings.midiActivationToleranceValue;
                lastNote = note;
            }

            if (framesRequiredForMidiSwitch == 0)
            {
                framesRequiredForMidiSwitch = midiTweak.ConfigSettings.midiActivationToleranceValue;
                return true;
            }
            else
            {
                framesRequiredForMidiSwitch--;
                return false;
            }
        }
    }
}