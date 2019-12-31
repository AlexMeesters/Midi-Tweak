using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Lowscope.Miditweak
{
    public class MidiTweak_Tab_AddParameter : MidiTweak_Tab
    {
        private List<ParameterData> activeReflectionData = null;
        private GameObject activeGameObject;
        private GameObject inspectorGameObject;

        public override void OnTabClosed()
        {

        }

        public override void OnTabOpened()
        {

        }

        public override void OnDisplayTab()
        {
            GUILayout.Space(5);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUI.BeginChangeCheck();

            inspectorGameObject = (GameObject)EditorGUILayout.ObjectField(inspectorGameObject, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                if (inspectorGameObject != null)
                {
                    if (inspectorGameObject.scene == midiTweak.gameObject.scene)
                    {
                        midiTweak.selectedGameObject = inspectorGameObject;
                    }
                    else
                    {
                        Debug.Log("MidiTweak Error: Invalid parameter, the Gameobject has to be within the same scene as the MidiTweak data container. ");
                        inspectorGameObject = null;
                        midiTweak.selectedGameObject = null;
                    }
                }
                else
                {
                    midiTweak.selectedGameObject = null;
                }
            }

            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Layout)
            {
                if (inspectorGameObject == null)
                {
                    if (midiTweak.selectedGameObject != null)
                    {
                        inspectorGameObject = midiTweak.selectedGameObject;
                    }
                }

                bool hasSwitchedObject = activeGameObject != inspectorGameObject;

                if (activeGameObject == null && inspectorGameObject != null)
                {
                    activeGameObject = inspectorGameObject;
                }

                if (activeReflectionData == null && activeGameObject != null)
                {
                    activeReflectionData = ReflectionFunctions.GetTypeFields(activeGameObject);
                }

                if (hasSwitchedObject)
                {
                    activeReflectionData = null;
                    activeGameObject = inspectorGameObject;
                }
            }


            if (inspectorGameObject != null && activeReflectionData != null)
            {
                for (int i = 0; i < activeReflectionData.Count; i++)
                {
                    GUILayout.BeginVertical(EditorStyles.helpBox);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(EditorGUIUtility.ObjectContent(null, typeof(MonoScript)).image, GUILayout.MaxWidth(18), GUILayout.MaxHeight(18));
                    EditorGUILayout.TextField(string.Format("Component: {0}", activeReflectionData[i].component.GetType().Name), EditorStyles.miniLabel);
                    GUILayout.EndHorizontal();

                    DisplayFieldData(activeReflectionData[i].component, activeReflectionData[i].fieldData);
                    GUILayout.EndVertical();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Select an object to add new parameters.", MessageType.Info);
            }


            GUILayout.EndVertical();
        }

        void DisplayFieldData(Component _component, List<FieldData> _fieldData)
        {
            for (int i = 0; i < _fieldData.Count; i++)
            {

                if (!string.IsNullOrEmpty(_fieldData[i].fieldName))
                {
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    bool isFieldStored = midiTweak.componentManipulator.ValidateFieldData(_component, _fieldData[i]);

                    GUILayout.Space(_fieldData[i].parentFields.Count * 25);

                    if (GUILayout.Button((isFieldStored) ? "-" : "+", GUILayout.Width(20)))
                    {
                        EditorUtility.SetDirty(midiTweak);

                        if (!isFieldStored)
                        {
                            midiTweak.componentManipulator.StoreFieldData(_component, _fieldData[i]);
                        }
                        else
                        {
                            midiTweak.componentManipulator.RemoveFieldData(_component, _fieldData[i]);
                        }

                        EditorSceneManager.MarkSceneDirty(midiTweak.gameObject.scene);

                        if (midiTweak.ConfigSettings.SaveSceneOnMidiAssignment)
                        {
                            EditorSceneManager.SaveScene(midiTweak.gameObject.scene);
                        }
                    }

                    if (isFieldStored)
                    {
                        GUI.enabled = false;
                    }

                    GUILayout.TextField(string.Format("{0}", _fieldData[i].fieldName), EditorStyles.label);

                    if (isFieldStored)
                    {
                        GUI.enabled = true;
                    }

                    GUILayout.EndHorizontal();
                }
            }
        }
    }
}