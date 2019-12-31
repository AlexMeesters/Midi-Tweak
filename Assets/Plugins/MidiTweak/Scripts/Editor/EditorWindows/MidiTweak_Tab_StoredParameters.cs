using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Lowscope.Miditweak
{
    public class MidiTweak_Tab_StoredParameters : MidiTweak_Tab
    {

        public override void OnTabClosed()
        {

        }

        public override void OnTabOpened()
        {

        }

        public override void OnDisplayTab()
        {
            GUILayout.Space(5);

            if (midiTweak.componentManipulator.parameterDataCollection.Count == 0)
            {
                EditorGUILayout.HelpBox("You can add new parameters through the add parameters tab.", MessageType.Info);
            }

            for (int saveDataIndex = 0; saveDataIndex < midiTweak.componentManipulator.parameterDataCollection.Count; saveDataIndex++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image, GUILayout.MaxWidth(25), GUILayout.MaxHeight(20)))
                {
                    Selection.activeGameObject = midiTweak.componentManipulator.parameterDataCollection[saveDataIndex].targetGameobject;
                }

                EditorGUIUtility.ObjectContent(midiTweak, typeof(GameObject));
                GUILayout.TextField(string.Format("{0}", (midiTweak.componentManipulator.parameterDataCollection[saveDataIndex].targetGameobject == null) ? "Not Found" : midiTweak.componentManipulator.parameterDataCollection[saveDataIndex].targetGameobject.name), EditorStyles.boldLabel);
                GUILayout.EndHorizontal();

                int reflectionDataIndex = -1;

                bool hasEmptyData = false;

                foreach (ParameterData reflectionData in midiTweak.componentManipulator.parameterDataCollection[saveDataIndex].data.ToArray())
                {
                    reflectionDataIndex++;

                    GUILayout.Space(5);

                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    bool isComponentNull = reflectionData.component == null;
                    bool isComponentInvalidScript = false;

                    if (!isComponentNull)
                    {
                        isComponentInvalidScript = reflectionData.component.GetType() == typeof(UnityEngine.Object);
                    }

                    if (isComponentNull || isComponentInvalidScript)
                    {
                        //GUI.enabled = false;
                        hasEmptyData = true;

                        if (!isComponentNull && isComponentInvalidScript)
                        {
                            reflectionData.component = null;
                        }
                    }

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(EditorGUIUtility.ObjectContent(null, typeof(MonoScript)).image, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18));
                    GUILayout.TextField(string.Format("Component: {0}", (reflectionData.component == null) ? "Component not found" : reflectionData.component.GetType().ToString()), EditorStyles.miniLabel);
                    GUILayout.EndHorizontal();

                    for (int fieldDataIndex = 0; fieldDataIndex < reflectionData.fieldData.Count; fieldDataIndex++)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            EditorUtility.SetDirty(midiTweak);

                            midiTweak.componentManipulator.RemoveFieldData(reflectionData.component, reflectionData.fieldData[fieldDataIndex]);

                            EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);

                            if (midiTweak.ConfigSettings.SaveSceneOnMidiAssignment)
                            {
                                EditorSceneManager.SaveScene(midiTweak.gameObject.scene);
                            }

                            if (reflectionData == null || reflectionData.fieldData == null || midiTweak.componentManipulator.parameterDataCollection.Count <= saveDataIndex + 1)
                            {
                                return;
                            }

                            continue;
                        }

                        // Display field name and value

                        GUILayout.TextField(reflectionData.fieldData[fieldDataIndex].fieldName, GUILayout.MinWidth(120));

                        EditorGUI.BeginChangeCheck();

                        object getFieldValue = midiTweak.componentManipulator.GetFieldValue(reflectionData.component, reflectionData.fieldData[fieldDataIndex]);

                        if (getFieldValue != null)
                        {
                            if (ReflectionFunctions.IsNumericType(getFieldValue))
                            {
                                float getFieldData = Convert.ToSingle(midiTweak.componentManipulator.GetFieldValue(reflectionData.component, reflectionData.fieldData[fieldDataIndex]));
                                float floatField = EditorGUILayout.FloatField(getFieldData, GUILayout.MinWidth(85));

                                if (EditorGUI.EndChangeCheck())
                                {
                                    midiTweak.componentManipulator.SetFieldValue(reflectionData.component, reflectionData.fieldData[fieldDataIndex], floatField);
                                    EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);
                                }
                            }

                            bool isGuiEnabled = GUI.enabled;

                            if (isGuiEnabled)
                            {
                                GUI.enabled = false;
                            }

                            bool foundValue = false;
                            float tweakedValue = 0;

                            DataPlayMode.Data getTweakData = null;

                            for (int i = 0; i < midiTweak.ConfigPlayMode.data.Count; i++)
                            {
                                getTweakData = midiTweak.ConfigPlayMode.data[i];
                                if (getTweakData.saveIndex == saveDataIndex && getTweakData.reflectionIndex == reflectionDataIndex && getTweakData.fieldIndex == fieldDataIndex)
                                {
                                    tweakedValue = getTweakData.value;
                                    foundValue = true;
                                    break;
                                }
                            }

                            EditorGUILayout.FloatField((!foundValue) ? 0 : tweakedValue, GUILayout.MinWidth(85));

                            if (isGuiEnabled)
                            {
                                GUI.enabled = true;
                            }

                            if (foundValue && tweakedValue != 0 && !Application.isPlaying)
                            {
                                if (GUILayout.Button("Apply", GUILayout.MinWidth(75)))
                                {
                                    midiTweak.componentManipulator.SetFieldValue(reflectionData.component, reflectionData.fieldData[fieldDataIndex], tweakedValue);
                                    getTweakData.value = 0;
                                    EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);
                                }
                            }
                            else
                            {
                                GUILayout.Label("", GUILayout.MinWidth(75));
                            }

                        }
                        else
                        {
                            EditorGUILayout.TextField("NOT FOUND", GUILayout.MinWidth(85));
                            EditorGUILayout.TextField("NOT FOUND", GUILayout.MinWidth(85));
                            GUILayout.Label("", GUILayout.MinWidth(75));
                        }

                        int getMidiInput = reflectionData.fieldData[fieldDataIndex].assignedMidiInput;

                        bool isBeingTweaked = false;

                        if (midiTweak.EditorInputProcessor.InEditorUpdateMode && midiTweak.EditorInputProcessor.midiAssignmentRequest != null)
                        {
                            isBeingTweaked = midiTweak.EditorInputProcessor.midiAssignmentRequest.CheckForMatch(saveDataIndex, reflectionDataIndex, fieldDataIndex);
                        }

                        GUILayout.TextField(string.Format("Assigned to (ID): {0}", (getMidiInput != 0) ? getMidiInput.ToString() : "None"), EditorStyles.miniLabel, GUILayout.MinWidth(125));

                        if (isBeingTweaked == false)
                        {

                            if (getMidiInput == 0)
                            {
                                if (GUILayout.Button("Assign", GUILayout.MinWidth(70)))
                                {
                                    MidiAssignmentRequest midiAssignmentRequest = new MidiAssignmentRequest(saveDataIndex, reflectionDataIndex, fieldDataIndex);
                                    midiTweak.EditorInputProcessor.StartListeningForInput(EInputType.Midi, EInputTarget.MidiSlider, _midiBinding: midiAssignmentRequest);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("UnAssign", GUILayout.MinWidth(70)))
                                {
                                    EditorUtility.SetDirty(midiTweak);
                                    reflectionData.fieldData[fieldDataIndex].assignedMidiInput = 0;
                                    EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);
                                }
                            }

                        }
                        else
                        {
                            if (GUILayout.Button("Assigning", GUILayout.Width(70)))
                            {

                            }

                            // Check for midi input
                            if (midiTweak.EditorInputProcessor.LastConfirmedMidiKey != 0)
                            {
                                EditorUtility.SetDirty(midiTweak);
                                reflectionData.fieldData[fieldDataIndex].assignedMidiInput = midiTweak.EditorInputProcessor.LastConfirmedMidiKey;
                                midiTweak.EditorInputProcessor.StopListening();
                                EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);

                                if (midiTweak.ConfigSettings.SaveSceneOnMidiAssignment)
                                {
                                    EditorSceneManager.SaveScene(midiTweak.gameObject.scene);
                                }
                            }

                        }

                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();

                    if (hasEmptyData)
                    {
                        GUI.enabled = true;
                    }
                }

                if (hasEmptyData)
                {
                    if (Application.isPlaying == false)
                    {
                        EditorGUILayout.BeginHorizontal();

                        if (GUILayout.Button("Clear Null Data", GUILayout.Width(140)))
                        {
                            midiTweak.componentManipulator.ClearAllEmptyReferences();
                        }

                        if (GUILayout.Button("Rescan for components", GUILayout.Width(175)))
                        {
                            midiTweak.componentManipulator.RecoverComponentsByInstanceID();
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }

                GUILayout.EndVertical();
                GUILayout.Space(5);

            }
        }
    }
}