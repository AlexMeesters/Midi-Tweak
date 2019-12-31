using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lowscope.Miditweak
{
    [CustomEditor(typeof(MidiTweak))]
    public class MidiTweak_WindowOpener : Editor
    {
        private MidiTweak midiTweak;

        void OnEnable()
        {
            midiTweak = (MidiTweak)target;
        }

        public override void OnInspectorGUI()
        {
            if (midiTweak.ConfigSettings.DebugMode)
            {
                DrawDefaultInspector();
            }

            if (GUILayout.Button("Open Midi Tweak Window" ,GUILayout.MinHeight(50) ))
            {
                MidiTweak_Window window = (MidiTweak_Window)EditorWindow.GetWindow(typeof(MidiTweak_Window), false, "Midi Tweak");
                window.minSize = new Vector2(650, 250);
                window.Show();
                window.Init(midiTweak);
            }
        }

    }
}