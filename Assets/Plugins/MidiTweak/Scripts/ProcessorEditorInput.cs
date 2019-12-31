using UnityEngine;
using System.Collections;
using MidiJack;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR

namespace Lowscope.Miditweak
{
    [System.Serializable]
    public class ProcessorEditorInput
    {
        // Used to assign a specific midi id to a parameter
        public MidiAssignmentRequest midiAssignmentRequest;

        // Used to assign a midi or keyboard binding to an option
        private KeyBinding keyAssignmentRequest;
        public KeyBinding KeyAssignmentRequest
        {
            get { return keyAssignmentRequest; }
        }

        public bool InEditorUpdateMode;

        private byte lastConfirmedMidiKey;
        public byte LastConfirmedMidiKey
        {
            get { return lastConfirmedMidiKey; }
        }

        private byte lastUsedKnob;
        private byte lastUsedKnobValue;

        private EInputType inputType;
        public Lowscope.Miditweak.EInputType InputListenMode
        {
            get { return inputType; }
        }

        private EInputTarget inputTarget;

        private int processedMessages = 0;

        public void EditorUpdate()
        {
            var messageCount = MidiDriver.Instance.TotalMessageCount;

            if (processedMessages < messageCount + 10)
            {
                processedMessages++;
                return;
            }

            if (inputType == EInputType.Midi || inputType == EInputType.KeyboardMidi)
            {
                foreach (var message in MidiDriver.Instance.History)
                {
                    if (lastUsedKnob != message.data1)
                    {
                        lastUsedKnobValue = 0;
                    }

                    lastUsedKnob = message.data1;

                    if (lastUsedKnobValue == 0)
                    {
                        lastUsedKnobValue = message.data2;
                    }

                    int knobDistance = Mathf.Abs(lastUsedKnobValue - message.data2);

                    if (inputTarget == EInputTarget.MidiSlider)
                    {
                        if (knobDistance > 15 && message.data2 != 0 && message.data2 != 127)
                        {
                            lastConfirmedMidiKey = message.data1;
                        }
                    }

                    if (inputTarget == EInputTarget.MidiButton)
                    {
                        if (knobDistance == 127)
                        {
                            lastConfirmedMidiKey = message.data1;
                        }
                    }
                }

            }
        }

        public void StartListeningForInput(EInputType _listenMode, EInputTarget _inputTarget, MidiAssignmentRequest _midiBinding)
        {
            MidiDriver.Instance.History.Clear();
            midiAssignmentRequest = _midiBinding;
            StartListening(_listenMode, _inputTarget);
        }

        public void StartListeningForInput(EInputType _listenMode, EInputTarget _inputTarget, KeyBinding _keyBinding)
        {
            MidiDriver.Instance.History.Clear();
            keyAssignmentRequest = _keyBinding;
            StartListening(_listenMode, _inputTarget);
        }

        public void StopListening()
        {
            midiAssignmentRequest = null;
            keyAssignmentRequest = null;
            lastConfirmedMidiKey = 0;
            lastUsedKnobValue = 0;

            InEditorUpdateMode = false;
            EditorApplication.update -= EditorUpdate;
            MidiDriver.Instance.History.Clear();
        }

        private void StartListening (EInputType _inputType ,EInputTarget _inputTarget)
        {
            inputType = _inputType;
            inputTarget = _inputTarget;
            lastConfirmedMidiKey = 0;
            lastUsedKnobValue = 0;
            lastUsedKnob = 0;
            processedMessages = MidiDriver.Instance.TotalMessageCount;

            InEditorUpdateMode = true;
            EditorApplication.update += EditorUpdate;
        }
    }
}

#endif
