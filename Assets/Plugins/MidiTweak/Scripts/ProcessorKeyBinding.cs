using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MidiJack;

namespace Lowscope.Miditweak
{
    [RequireComponent(typeof(MidiTweak)), AddComponentMenu("")]
    public class ProcessorKeyBinding : MonoBehaviour
    {
        private MidiTweak midiTweak;

        private Dictionary<KeyBinding, Action> keyBindingActions = new Dictionary<KeyBinding, Action>();

        private int lastPressedNote;

        private void Start()
        {
            midiTweak = GetComponent<MidiTweak>();
            AttachBindingsToFunction();
        }

        private void OnEnable()
        {
            MidiMaster.knobDelegate += ProcessMidiNotification;
            MidiMaster.noteOnDelegate += ProcessMidiNotification;
        }

        private void OnDisable()
        {
            MidiMaster.knobDelegate -= ProcessMidiNotification;
            MidiMaster.noteOnDelegate -= ProcessMidiNotification;
        }

        private void AttachBindingsToFunction ()
        {
            DataKeyBindings keyBindingAsset = midiTweak.ConfigKeyBindings;
            DataSettings configurationAsset = midiTweak.ConfigSettings;

            keyBindingActions.Add(keyBindingAsset.IncreaseRange, () => { midiTweak.IncreaseZoomRange(configurationAsset.ZoomIncrementAmount); });

            keyBindingActions.Add(keyBindingAsset.DecreaseRange, () => { midiTweak.IncreaseZoomRange(-configurationAsset.ZoomIncrementAmount); });

            keyBindingActions.Add(keyBindingAsset.FreezeValue, () => { midiTweak.FreezeCurrentValue(); });

            keyBindingActions.Add(keyBindingAsset.SwitchParameterBackward, () => { midiTweak.SwitchParameter(-1); });

            keyBindingActions.Add(keyBindingAsset.SwitchParameterForward, () => { midiTweak.SwitchParameter(1); });

            keyBindingActions.Add(keyBindingAsset.ToggleMenuVisiblity, () => { midiTweak.ToggleDisplayUserInterface(); });
        }

        private void ProcessMidiNotification(MidiJack.MidiChannel _channel, int _note, float _velocity)
        {
            if (_velocity == 1 || _velocity == 0)
            {
                lastPressedNote = _note;
            }
        }

        private void Update()
        {
            // Return if no input has been detected
            if (!(Input.anyKey || lastPressedNote != -1))
                return;

            foreach (KeyValuePair<KeyBinding, Action> binding in keyBindingActions)
            {
                if (binding.Key.IsPressed(lastPressedNote))
                {
                    binding.Value();
                    break;
                }
            }

            lastPressedNote = -1;
        }
    }
}