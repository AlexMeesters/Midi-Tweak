using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace Lowscope.Miditweak
{

    public class MidiTweak_Window : EditorWindow
    {
        [SerializeField]
        private MidiTweak_Tab activeTab;

        [SerializeField]
        public MidiTweak midiTweak;

        private Vector2 scrollPosition;

        [SerializeField]
        private MidiTweak_Tab[] tabs = new MidiTweak_Tab[3]
        {
            new MidiTweak_Tab_AddParameter(),
            new MidiTweak_Tab_StoredParameters(),
            new MidiTweak_Tab_Options()
        };

        private MidiTweak_Tab tweakDisplayerTab = new MidiTweak_Tab_TweakDisplayer();

        [SerializeField]
        private int midiTweakInstanceID;

        private int currentTab = 0;

        [MenuItem("Window/Midi Tweak")]
        public static void OpenWindow()
        {
            MidiTweak_Window window = (MidiTweak_Window)EditorWindow.GetWindow(typeof(MidiTweak_Window), false, "Midi Tweak");
            window.minSize = new Vector2(650, 250);
            window.Show();
        }

        public void Init(MidiTweak _midiTweak)
        {
            midiTweak = _midiTweak;

            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(midiTweak);
            Repaint();
        }

        private void OnEnable()
        {

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnPlayModeChanged;
#endif

            EditorSceneManager.activeSceneChanged += OnSceneChange;
            EditorSceneManager.sceneOpened += OnSceneLoad;

            ScanForMidiTweak();
        }

        private void OnDisable()
        {

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnPlayModeChanged;
#endif

            EditorSceneManager.activeSceneChanged -= OnSceneChange;
            EditorSceneManager.sceneOpened -= OnSceneLoad;

            // Ensure that there is no active update going on when the midi tweak window is closed
            if (midiTweak != null && midiTweak.EditorInputProcessor.InEditorUpdateMode)
            {
                midiTweak.EditorInputProcessor.StopListening();
            }

            AssetDatabase.SaveAssets();
        }

        private void ScanForMidiTweak()
        {
            if (Application.isPlaying)
            {
                if (MidiTweak.Instance != null)
                {
                    midiTweak = MidiTweak.Instance;
                    return;
                }
            }

            MidiTweak[] midiTweakContainers = GameObject.FindObjectsOfType<MidiTweak>();
            if (midiTweakContainers.Length > 1)
            {
                Debug.Log("More than one midi tweak instance was found within the scene. Duplicate instances have been removed.");
                midiTweak = midiTweakContainers[0];

                for (int i = 0; i < midiTweakContainers.Length; i++)
                {
                    if (i != 0)
                    {
                        GameObject.DestroyImmediate(midiTweakContainers[i].gameObject);
                    }
                }
            }
            else
            {
                if (midiTweakContainers.Length > 0)
                {
                    midiTweak = midiTweakContainers[0];
                }
            }
        }

        void OnSceneLoad(Scene _s1, OpenSceneMode _mode)
        {
            OnSceneChange(_s1, _s1);
        }

        void OnSceneChange(Scene _s1, Scene _s2)
        {
            ScanForMidiTweak();
            activeTab = null;
            Repaint();
        }

#if UNITY_2017_2_OR_NEWER
        void OnPlaymodeStateChanged(PlayModeStateChange _stateChange)
#else
        void OnPlayModeChanged ()
#endif
        {
            if (midiTweakInstanceID != 0)
            {
                midiTweak = EditorUtility.InstanceIDToObject(midiTweakInstanceID) as MidiTweak;

                if (midiTweak == null)
                {
                    ScanForMidiTweak();
                }
            }

            Repaint();
        }

        public void OnGUI()
        {
            if (midiTweak == null)
            {
                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("Create new Midi Tweak controller", GUILayout.MinHeight(50)))
                    {
                        midiTweak = new GameObject("Midi Tweak Controller", typeof(MidiTweak)).GetComponent<MidiTweak>();

                        midiTweak.UpdateReferences();

                        ProcessorKeyBinding processorKeyBinding = midiTweak.gameObject.AddComponent<ProcessorKeyBinding>();
                        processorKeyBinding.hideFlags = HideFlags.HideInInspector;

                        ProcessorMidiInput processorMidiInput = midiTweak.gameObject.AddComponent<ProcessorMidiInput>();
                        processorMidiInput.hideFlags = HideFlags.HideInInspector;

                        midiTweakInstanceID = midiTweak.GetInstanceID();
                        EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);
                    }
                }

                if (GUILayout.Button("Scan scene for Midi Tweak controller", GUILayout.MinHeight(50)))
                {
                    midiTweak = GameObject.FindObjectOfType<MidiTweak>();
                }

                activeTab = null;

                return;
            }
            else
            {
                midiTweakInstanceID = midiTweak.GetInstanceID();
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

            // Do not allow any changes to the UI when midi tweak is in editor update mode.
            if (midiTweak.EditorInputProcessor.InEditorUpdateMode)
            {
                GUI.enabled = false;
            }

            if (!Application.isPlaying)
            {
                currentTab = GUILayout.Toolbar(currentTab, new string[3] { "Add Parameter", "Stored Parameters", "Options" });

                if (activeTab == null)
                {
                    tabs[currentTab].OpenTab(midiTweak);
                    activeTab = tabs[currentTab];
                }
                else
                {
                    if (activeTab != tabs[currentTab])
                    {
                        activeTab.OnTabClosed();

                        tabs[currentTab].OpenTab(midiTweak);
                        activeTab = tabs[currentTab];
                    }
                }

                activeTab.DisplayTab();
            }
            else
            {
                if (activeTab != tweakDisplayerTab)
                {
                    activeTab = tweakDisplayerTab;
                    activeTab.OpenTab(midiTweak);
                }

                tweakDisplayerTab.DisplayTab();

                // TODO: Only do a repaint when something changes.
                Repaint();
            }

            if (midiTweak.EditorInputProcessor.InEditorUpdateMode)
            {
                DisplayUpdateModeInfo();
            }

            if (midiTweak.ConfigSettings.DebugDisplayMidiInputs && currentTab == 2)
            {
                Repaint();
            }

            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            GUILayout.Label("Developed by Lowscope   -   Contact: info@low-scope.com   -   Version 1.0", EditorStyles.miniLabel);
        }

        private void DisplayIsPlayingInfo()
        {
            EditorGUILayout.HelpBox("Note: Assigning parameters is only possible when outside of playmode.", MessageType.Info);
        }

        private void DisplayUpdateModeInfo()
        {
            Repaint();

            EditorUtility.SetDirty(midiTweak.ConfigSettings);
            EditorUtility.SetDirty(midiTweak);

            GUI.enabled = true;

            switch (midiTweak.EditorInputProcessor.InputListenMode)
            {
                case EInputType.Midi:
                    EditorGUILayout.HelpBox("Move any knob or slider on your midi device to assign it to the parameter.", MessageType.Info);
                    break;
                case EInputType.Keyboard:
                    EditorGUILayout.HelpBox("Press any key on your keyboard to assign it to the parameter.", MessageType.Info);
                    break;
                case EInputType.KeyboardMidi:
                    EditorGUILayout.HelpBox("Press any button on your midi device, or press any keyboard key to assign it to the parameter.", MessageType.Info);
                    break;
                default:
                    break;
            }

            if (GUILayout.Button("Cancel"))
            {
                midiTweak.EditorInputProcessor.StopListening();
            }
        }

    }
}