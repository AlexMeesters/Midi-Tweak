using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MidiJack;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

#if (UNITY_EDITOR)

using UnityEditor;

#endif

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Used to store all scene based information, used to listen to midi IO and ensures visualization of the process.
    /// </summary>

    [DisallowMultipleComponent, AddComponentMenu("")]
    public class MidiTweak : MonoBehaviour
    {
        public static MidiTweak Instance;

        [SerializeField]
        private DataKeyBindings configKeyBindings;

        public DataKeyBindings ConfigKeyBindings
        {
            get
            {
                if (configKeyBindings == null)
                {
                    configKeyBindings = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataKeyBindings>("Resources");
                }

                return configKeyBindings;
            }
        }

        [SerializeField]
        private DataSettings configSettings;

        public DataSettings ConfigSettings
        {
            get
            {
                if (configSettings == null)
                {
                    configSettings = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataSettings>("Resources");
                }

                return configSettings;
            }
        }

        [SerializeField]
        private DataPlayMode configPlayMode;

        public DataPlayMode ConfigPlayMode
        {
            get
            {
                if (configPlayMode == null)
                {
                    configPlayMode = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataPlayMode>("Resources");
                }

                return configPlayMode;
            }
        }

        public ComponentManipulator componentManipulator = new ComponentManipulator();

        private InterfaceManager interfaceManager = new InterfaceManager();
        public InterfaceManager InterfaceManager
        {
            get { return interfaceManager; }
        }

        private List<MidiBookmark> parameterBookMarks = new List<MidiBookmark>();
        public List<MidiBookmark> ParameterBookMarks
        {
            get { return parameterBookMarks; }
        }

        [SerializeField]
        private GameObject dataVizualizerPrefab;

        private GameObject instantiatedDataVizualizer;

        [System.Serializable]
        public class RuntimeSettings
        {
            public int CurrentZoomRange;
            public int ActiveParameterBookMark = -1;
            public bool IsFrozen;
            public float CurrentSliderPercentage = 0.5f;
            public float CurrentSliderValue = 0;
            public float SliderMidValue;
            public float SliderMinValue;
            public float SliderMaxValue;
        }

        private RuntimeSettings settings;

#if UNITY_EDITOR

        public GameObject selectedGameObject;
        public int SelectedTab;

        private ProcessorEditorInput editorInputProcessor = new ProcessorEditorInput();
        public Lowscope.Miditweak.ProcessorEditorInput EditorInputProcessor
        {
            get { return editorInputProcessor; }
        }

        public void UpdateReferences()
        {
            if (dataVizualizerPrefab == null)
            {
                dataVizualizerPrefab = Resources.Load("MidiTweak_DataVizualizer") as GameObject;
            }

            if (configSettings == null)
            {
                configSettings = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataSettings>("Resources");
            }

            if (configKeyBindings == null)
            {
                configKeyBindings = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataKeyBindings>("Resources");
            }

            if (configPlayMode == null)
            {
                configPlayMode = MidiTweakScriptableObjectUtility.GetOrCreateScriptableObject<DataPlayMode>("Resources");
            }
        }

#endif

        #region Initialization

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                GameObject.Destroy(this.gameObject);
            }

            bool loadedNewScene = false;

            // Retrieve previous settings and values if scene has been reloaded
            if (configSettings.KeepChangesAfterSceneLoad && Time.time != 0)
            {
                settings = configPlayMode.settings;
                ApplyTweakedValues();
                loadedNewScene = true;
            }
            else
            {
                settings = new RuntimeSettings();
                configPlayMode.data.Clear();
            }

            if (configSettings.instantiateInGameTweakMenuOnPlay)
            {
                IntantiateTweakingUserInterface();
            }

            parameterBookMarks = CreateParameterBookmarks();

            interfaceManager.NotifyParameterLoad(parameterBookMarks.ToArray());

            interfaceManager.NotifyKeyAssignment(configKeyBindings.Bindings);

            if (loadedNewScene)
            {
                ResendInterfaceNotifications();
            }
            else
            {
                SetActiveParameter(0);
            }
        }

        private void ApplyTweakedValues()
        {
            for (int i = 0; i < configPlayMode.data.Count; i++)
            {
                DataPlayMode.Data getData = configPlayMode.data[i];

                if (getData.value == 0)
                    continue;

                Component getComponent = componentManipulator.parameterDataCollection[getData.saveIndex].data[getData.reflectionIndex].component;
                FieldData getFieldData = componentManipulator.parameterDataCollection[getData.saveIndex].data[getData.reflectionIndex].fieldData[getData.fieldIndex];

                componentManipulator.SetFieldValue(getComponent, getFieldData, getData.value);
            }
        }

        private void OnDestroy()
        {
            configPlayMode.settings = settings;
        }

        public void ResendInterfaceNotifications()
        {
            if (parameterBookMarks.Count == 0)
            {
                return;
            }

            SetActiveParameter(settings.ActiveParameterBookMark, _updateZoomRange: false, _setFrozen: false);
            SetZoomRange(settings.CurrentZoomRange);
            SetFrozen(settings.IsFrozen);
            SetValueByPercentage(settings.CurrentSliderPercentage, settings.ActiveParameterBookMark);
        }

        private void IntantiateTweakingUserInterface()
        {
            if (dataVizualizerPrefab != null)
            {
                instantiatedDataVizualizer = GameObject.Instantiate(dataVizualizerPrefab);
                interfaceManager.ScanAddInterfaces(instantiatedDataVizualizer);

                instantiatedDataVizualizer.gameObject.SetActive(configSettings.displayInGameTweakMenuOnPlay);
            }
        }

        /// <summary>
        /// Parameter bookmarks are used to quickly access obtained reflection data
        /// The play mode data will also be saved with this method.
        /// </summary>
        /// <returns></returns>
        private List<MidiBookmark> CreateParameterBookmarks()
        {
            List<MidiBookmark> createParameterBookMark = new List<MidiBookmark>();

            for (int i = 0; i < componentManipulator.parameterDataCollection.Count; i++)
            {
                for (int i2 = 0; i2 < componentManipulator.parameterDataCollection[i].data.Count; i2++)
                {
                    for (int i3 = 0; i3 < componentManipulator.parameterDataCollection[i].data[i2].fieldData.Count; i3++)
                    {
                        ParameterData getParameterData = componentManipulator.parameterDataCollection[i].data[i2];

                        if (componentManipulator.GetFieldValue(getParameterData.component, getParameterData.fieldData[i3]) == null)
                            continue;

                        // Create midi bookmarks
                        MidiBookmark createParamBookmark =
                            new MidiBookmark(
                                getParameterData.fieldData[i3].assignedMidiInput,
                                getParameterData.component,
                                getParameterData.fieldData[i3],
                                0);

                        createParameterBookMark.Add(createParamBookmark);

                        // Store a link to the parameter data location, so it knows where to save the value later
                        configPlayMode.AddData(i, i2, i3);
                    }
                }
            }

            return createParameterBookMark;
        }

        #endregion

        #region Functionality

        public void SetNewMidValue(float _value, bool _centerSlider)
        {
            if (_value == 0)
            {
                _value = 0.001f;
            }

            settings.SliderMidValue = _value;

            if (_centerSlider)
            {
                settings.CurrentSliderPercentage = 0.5f;
                settings.CurrentSliderValue = Mathf.Lerp(settings.SliderMinValue, settings.SliderMaxValue, settings.CurrentSliderPercentage);
            }

            ResendInterfaceNotifications();
        }

        /// <summary>
        /// Set the value of a specific parameter by providing a slider percentage.
        /// This will modify the fields of component the target parameter.
        /// </summary>
        /// <param name="_percentage"> Ranges from 0 to 1.0f, based on the min-max values. </param>
        /// <param name="_paramIndex"> Index of the midi parameter bookmark. </param>
        public void SetValueByPercentage(float _percentage, int _paramIndex = -1)
        {
            if (_paramIndex == -1)
            {
                _paramIndex = settings.ActiveParameterBookMark;
            }

            SetValue(Mathf.Lerp(settings.SliderMinValue, settings.SliderMaxValue, _percentage), _paramIndex);
        }

        /// <summary>
        /// Set the value of a specific parameter.
        /// This will modify the fields of component the target parameter.
        /// </summary>
        /// <param name="_paramIndex"> Index of the midi parameter bookmark. </param>
        public void SetValue(float _value, int _paramIndex = -1)
        {
            if (_paramIndex == -1)
            {
                _paramIndex = settings.ActiveParameterBookMark;
            }

            if (!IsParamterValid(_paramIndex))
            {
                Debug.LogFormat("Midi Tweak: Attempted to access invalid parameter {0}", _paramIndex);
                return;
            }

            // TODO: This causes errors still...?
            if (_value == 0)
            {
                _value = 0.1f;
            }

            settings.CurrentSliderValue = _value;
            settings.CurrentSliderPercentage = (_value - settings.SliderMinValue) / (settings.SliderMaxValue - settings.SliderMinValue);

            if (!settings.IsFrozen)
            {
                // This sets the value of the actual object.
                float setFieldValue = componentManipulator.SetFieldValue(parameterBookMarks[_paramIndex].Component, parameterBookMarks[_paramIndex].Field, _value);
                configPlayMode.data[_paramIndex].value = setFieldValue;

                interfaceManager.NotifyValueChange(setFieldValue);
            }
            else
            {
                if (System.Math.Round(settings.CurrentSliderPercentage, 2) == 0.5f)
                {
                    SetFrozen(false);
                }

                interfaceManager.NotifyValueChange(_value);
            }
        }

        public MidiBookmark GetActiveBookmark()
        {
            return parameterBookMarks[settings.ActiveParameterBookMark];
        }

        public int GetActiveBookmarkIndex()
        {
            return settings.ActiveParameterBookMark;
        }

        public void SetFrozen(bool _state)
        {
            interfaceManager.NotifySetFrozen(_state);

            settings.IsFrozen = _state;
        }

        public void SetZoomRange(int _zoomRange)
        {
            if (_zoomRange == 0 || _zoomRange < 0)
            {
                _zoomRange = 1;
            }

            // Design choice to keep 
            float tweakRangeToDecimal = (float)_zoomRange * ((settings.SliderMidValue < 0) ? -0.01f : 0.01f);

            settings.SliderMinValue = (settings.SliderMidValue * (1 - tweakRangeToDecimal));
            settings.SliderMaxValue = (settings.SliderMidValue * (1 + tweakRangeToDecimal));

            settings.CurrentZoomRange = _zoomRange;

            interfaceManager.NotifyZoomChange(_zoomRange);

            interfaceManager.NotifyScopeChange(settings.SliderMinValue, settings.SliderMidValue, settings.SliderMaxValue);
            //interfaceManager.NotifyScopeChange(settings.SliderMidValue * (1 - tweakRangeToDecimal), settings.SliderMidValue, settings.SliderMidValue * (1 + tweakRangeToDecimal));

            SetValueByPercentage(settings.CurrentSliderPercentage, settings.ActiveParameterBookMark);
        }

        /// <summary>
        /// Zoom range is the offset from the mid value.
        /// </summary>
        /// <param name="_amount"></param>
        public void IncreaseZoomRange(int _amount)
        {
            settings.CurrentZoomRange += _amount;

            if (settings.CurrentZoomRange < 1)
            {
                settings.CurrentZoomRange = 1;
            }

            SetZoomRange(settings.CurrentZoomRange);
        }

        public void FreezeCurrentValue(bool _resetZoom = true)
        {
            if (configSettings.RequireRecenterOnFreeze)
            {
                if (settings.IsFrozen == true)
                {
                    return;
                }

                settings.IsFrozen = true;

                interfaceManager.NotifySetFrozen(true);
            }

            settings.SliderMidValue = Mathf.Lerp(settings.SliderMinValue, settings.SliderMaxValue, settings.CurrentSliderPercentage);


            if (_resetZoom)
            {
                if (configSettings.ResetToDefaultZoomOnFreeze)
                {
                    settings.CurrentZoomRange = configSettings.BaseZoomRange;
                }
            }

            SetZoomRange(settings.CurrentZoomRange);

        }

        public void ToggleDisplayUserInterface()
        {
            if (instantiatedDataVizualizer != null)
            {
                instantiatedDataVizualizer.gameObject.SetActive(!instantiatedDataVizualizer.activeSelf);
            }
        }

        public void SwitchParameter(int _direction)
        {
            int newParameterIndex = settings.ActiveParameterBookMark + _direction;

            if (!IsParamterValid(newParameterIndex))
                return;

            SetActiveParameter(settings.ActiveParameterBookMark + _direction);
        }

        public void SetActiveParameter(int _paramIndex, int _midiKeyIndex = 0, bool _updateZoomRange = true, bool _setFrozen = true)
        {
            if (parameterBookMarks.Count == 0)
                return;

            if (!IsParamterValid(_paramIndex))
            {
                Debug.LogFormat("Midi Tweak: Attempted to access invalid parameter {0}", _paramIndex);
                return;
            }

            _paramIndex = Mathf.Clamp(_paramIndex, 0, parameterBookMarks.Count - 1);

            settings.ActiveParameterBookMark = _paramIndex;

            if (_updateZoomRange)
            {
                settings.SliderMidValue = Convert.ToSingle(componentManipulator.GetFieldValue(parameterBookMarks[_paramIndex].Component, parameterBookMarks[_paramIndex].Field));

                if (settings.SliderMidValue == 0)
                {
                    settings.SliderMidValue = 1f;
                }

                if (settings.CurrentZoomRange == 0)
                {
                    settings.CurrentZoomRange = configSettings.BaseZoomRange;
                }

                SetZoomRange(settings.CurrentZoomRange);
            }

            if (_setFrozen)
            {
                SetFrozen(true);
            }

            interfaceManager.NotifyParameterChange(parameterBookMarks[_paramIndex].Component.ToString(), parameterBookMarks[_paramIndex].Field.fieldName, _paramIndex, _midiKeyIndex);
        }

        private bool IsParamterValid(int _index)
        {
            if (_index < 0 || _index > parameterBookMarks.Count - 1)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}