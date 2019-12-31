using UnityEngine;
using UnityEditor;

namespace Lowscope.Miditweak
{
    public class MidiTweak_Tab_Options : MidiTweak_Tab
    {
        private bool toggleDebug;

        public override void OnTabOpened()
        {

        }

        public override void OnTabClosed()
        {

        }

        public override void OnDisplayTab()
        {
            EditorUtility.SetDirty(midiTweak.ConfigSettings);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Key Bindings (Midi / Keyboard)", EditorStyles.boldLabel);

            for (int i = 0; i < midiTweak.ConfigKeyBindings.Bindings.Length; i++)
            {
                KeyBinding getBinding = midiTweak.ConfigKeyBindings.Bindings[i];

                DrawInputDrawer(getBinding.Name, getBinding, i);
            }

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label("General", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("MIDI start zoom (%)", GUILayout.MaxWidth(240));
            midiTweak.ConfigSettings.BaseZoomRange = EditorGUILayout.IntField(midiTweak.ConfigSettings.BaseZoomRange, GUILayout.MaxWidth(50));
            midiTweak.ConfigSettings.BaseZoomRange = (int)GUILayout.HorizontalSlider(midiTweak.ConfigSettings.BaseZoomRange, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label("MIDI zoom button Increment (%)", GUILayout.MaxWidth(240));
            midiTweak.ConfigSettings.ZoomIncrementAmount = EditorGUILayout.IntField(midiTweak.ConfigSettings.ZoomIncrementAmount, GUILayout.MaxWidth(50));
            midiTweak.ConfigSettings.ZoomIncrementAmount = (int)GUILayout.HorizontalSlider(midiTweak.ConfigSettings.ZoomIncrementAmount, 0, 100);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.ResetToDefaultZoomOnFreeze = GUILayout.Toggle(midiTweak.ConfigSettings.ResetToDefaultZoomOnFreeze, " Reset to initial zoom on freeze");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.RequireRecenterOnFreeze = GUILayout.Toggle(midiTweak.ConfigSettings.RequireRecenterOnFreeze, " Require recenter on freeze");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.LimitInputToMidiAssignments = GUILayout.Toggle(midiTweak.ConfigSettings.LimitInputToMidiAssignments, " Limit midi input to midi assignments");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.KeepChangesAfterSceneLoad = GUILayout.Toggle(midiTweak.ConfigSettings.KeepChangesAfterSceneLoad, " Keep changes after scene load");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.SaveSceneOnMidiAssignment = GUILayout.Toggle(midiTweak.ConfigSettings.SaveSceneOnMidiAssignment, " Auto save scene when changes are made to the parameter storage");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.instantiateInGameTweakMenuOnPlay = GUILayout.Toggle(midiTweak.ConfigSettings.instantiateInGameTweakMenuOnPlay, " Instantiate the tweaking menu on play");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.displayInGameTweakMenuOnPlay = GUILayout.Toggle(midiTweak.ConfigSettings.displayInGameTweakMenuOnPlay, " Display in-game tweak menu on play");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            midiTweak.ConfigSettings.midiActivationTolerance = (EMidiActivationTolerance)EditorGUILayout.EnumPopup("Midi Activation Tolerance", midiTweak.ConfigSettings.midiActivationTolerance);
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Debug", EditorStyles.boldLabel);

            midiTweak.ConfigSettings.DebugDisplayMidiInputs = EditorGUILayout.Toggle("Display midi input info", midiTweak.ConfigSettings.DebugDisplayMidiInputs);

            if (midiTweak.ConfigSettings.DebugDisplayMidiInputs)
            {
                DisplayInputMasterDebug();
            }

            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void ClearDuplicateKeyBindings(KeyCode _binding, int _index)
        {
            for (int i = 0; i < midiTweak.ConfigKeyBindings.Bindings.Length; i++)
            {
                if (i != _index && midiTweak.ConfigKeyBindings.Bindings[i].DoesKeyboardInputMatch(_binding))
                {
                    midiTweak.ConfigKeyBindings.Bindings[i].ClearInput();
                }
            }
        }

        private void ClearDuplicateMidiBindings(uint _input, int _index)
        {
            for (int i = 0; i < midiTweak.ConfigKeyBindings.Bindings.Length; i++)
            {
                if (i != _index && midiTweak.ConfigKeyBindings.Bindings[i].DoesMidiInputMatch(_input))
                {
                    midiTweak.ConfigKeyBindings.Bindings[i].ClearInput();
                }
            }
        }

        private void DrawInputDrawer(string _inputName, KeyBinding _inputBinding, int _index)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(_inputName.ToString(), GUILayout.MinWidth(175));
            GUILayout.Label(string.Format("Assigned to: {0}", _inputBinding.ReturnAssignmentStatus()), GUILayout.MinWidth(150));

            if (midiTweak.EditorInputProcessor.KeyAssignmentRequest == _inputBinding)
            {
                if (Event.current.keyCode != KeyCode.None)
                {
                    if (Event.current.keyCode == KeyCode.Escape)
                    {
                        midiTweak.EditorInputProcessor.StopListening();
                    }
                    else
                    {
                        foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                        {
                            if (Event.current.keyCode == keyCode)
                            {
                                EditorUtility.SetDirty(midiTweak.ConfigKeyBindings);

                                _inputBinding.AssignKeyBoardInput(keyCode);
                                ClearDuplicateKeyBindings(keyCode, _index);
                                midiTweak.EditorInputProcessor.StopListening();
                            }
                        }
                    }
                }

                if (midiTweak.EditorInputProcessor.LastConfirmedMidiKey != 0)
                {
                    EditorUtility.SetDirty(midiTweak.ConfigKeyBindings);

                    _inputBinding.AssignMidiInput(midiTweak.EditorInputProcessor.LastConfirmedMidiKey);
                    ClearDuplicateMidiBindings(midiTweak.EditorInputProcessor.LastConfirmedMidiKey, _index);
                    midiTweak.EditorInputProcessor.StopListening();
                }

                if (GUILayout.Button("Assigning", GUILayout.MinWidth(100)))
                {

                }
            }
            else
            {

                if (_inputBinding.HasAssignedInput() == false)
                {
                    if (GUILayout.Button("Assign", GUILayout.MinWidth(100)))
                    {
                        midiTweak.EditorInputProcessor.StartListeningForInput(EInputType.KeyboardMidi, EInputTarget.MidiButton, _inputBinding);
                    }
                }
                else
                {
                    if (GUILayout.Button("UnAssign", GUILayout.MinWidth(100)))
                    {
                        EditorUtility.SetDirty(midiTweak.ConfigKeyBindings);

                        _inputBinding.ClearInput();
                    }
                }
            }

            GUILayout.EndHorizontal();
        }

        private void DisplayToggleDebug()
        {
            toggleDebug = midiTweak.ConfigSettings.DebugMode;

            EditorGUI.BeginChangeCheck();

            toggleDebug = EditorGUILayout.Toggle("Toggle Debug Mode", toggleDebug);

            if (EditorGUI.EndChangeCheck())
            {
                midiTweak.ConfigSettings.DebugMode = toggleDebug;
            }
        }

        // This code is taken from UI window of Midi Jack Copyright (C) 2013-2016 Keijiro Takahashi
        private void DisplayInputMasterDebug()
        {
            var endpointCount = MidiJack.MidiJackWindow.CountEndpoints();
            var messageCount = MidiJack.MidiDriver.Instance.TotalMessageCount;

            // Endpoints
            var temp = "Detected MIDI devices:";
            for (var i = 0; i < endpointCount; i++)
            {
                var id = MidiJack.MidiJackWindow.GetEndpointIdAtIndex(i);
                var name = MidiJack.MidiJackWindow.GetEndpointName(id);
                temp += "\n" + id.ToString("X8") + ": " + name;
            }
            EditorGUILayout.HelpBox(temp, MessageType.None);

            // Message history
            temp = "Recent MIDI messages:";
            foreach (var message in MidiJack.MidiDriver.Instance.History)
                temp += "\n" + message.ToString();
            EditorGUILayout.HelpBox(temp, MessageType.None);
        }
    }
}