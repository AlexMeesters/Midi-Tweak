using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Displayed while in editor mode.
    /// </summary>

    public class MidiTweak_Tab_TweakDisplayer : MidiTweak_Tab, IListenToParameterChange, IListenToScopeChange, IListenToSetFrozen, IListenToZoomChange, IListenToValueChange
    {
        // Cached variables retrieved from the interface functionality

        private int parameterIndex;

        private float minScope;
        private float midScope;
        private float maxScope;

        private float currentValue;
        private float currentPercentage;

        private bool isFrozen;
        private float zoom;

        private float userSetValue;

        private string[] bookMarkStrings;

        private Vector2 scrollViewPosition;

        private string keyBindingNames;
        private string keyBindingInputs;

        private GUIStyle alignCenterStyle;
        private GUIStyle alignRightStyle;
        private GUIStyle alignCenterStyleBold;
        private GUIStyle alignCenterFloatField;

        private int newzoom = -1;

        public override void OnTabClosed()
        {

        }

        public override void OnTabOpened()
        {
            alignCenterStyle = new GUIStyle(GUI.skin.label);
            alignCenterStyle.alignment = TextAnchor.MiddleCenter;

            alignRightStyle = new GUIStyle(GUI.skin.label);
            alignRightStyle.alignment = TextAnchor.MiddleRight;

            alignCenterStyleBold = new GUIStyle(GUI.skin.label);
            alignCenterStyleBold.fontStyle = FontStyle.Bold;
            alignCenterStyleBold.alignment = TextAnchor.MiddleCenter;

            alignCenterFloatField = new GUIStyle(GUI.skin.textField);
            alignCenterFloatField.alignment = TextAnchor.MiddleCenter;

            midiTweak.InterfaceManager.AddInterface((IListenToParameterChange)this);
            midiTweak.InterfaceManager.AddInterface((IListenToScopeChange)this);
            midiTweak.InterfaceManager.AddInterface((IListenToSetFrozen)this);
            midiTweak.InterfaceManager.AddInterface((IListenToZoomChange)this);
            midiTweak.InterfaceManager.AddInterface((IListenToValueChange)this);

            UpdateBookMarkString();
            CreateKeyAssignmentString();

            midiTweak.ResendInterfaceNotifications();
        }


        public override void OnDisplayTab()
        {
            if (bookMarkStrings.Length == 0)
            {
                EditorGUILayout.HelpBox("No parameters have been added, please start adding parameters if you want to tweak anything.", MessageType.Info);
                GUI.enabled = false;
            }

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();

            DisplayParameters();

            GUILayout.BeginVertical(GUILayout.MinWidth(350));

            DisplayBindings();

            DisplayTweakInterface();

            GUILayout.EndVertical();


            GUILayout.EndHorizontal();
        }

        private void UpdateBookMarkString()
        {
            string[] bookmarkNames = new string[midiTweak.ParameterBookMarks.Count];

            for (int i = 0; i < midiTweak.ParameterBookMarks.Count; i++)
            {
                bookmarkNames[i] = string.Format("{0} - {1}", midiTweak.ParameterBookMarks[i].Component.ToString(), midiTweak.ParameterBookMarks[i].Field.fieldName);
            }

            bookMarkStrings = bookmarkNames;
        }

        public void OnParameterChange(string _gameObject, string _name, int _index, int _midiIndex)
        {
            parameterIndex = _index;
        }

        public void OnScopeChange(float _min, float _mid, float _max)
        {
            minScope = (float)Math.Round((decimal)_min, 3);
            midScope = (float)Math.Round((decimal)_mid, 3);
            maxScope = (float)Math.Round((decimal)_max, 3);
        }

        public void OnSetFrozen(bool _frozen)
        {
            isFrozen = _frozen;
        }

        public void OnZoomChange(float _value)
        {
            zoom = _value;
        }

        public void OnValueChange(float _value)
        {
            currentValue = (float)Math.Round((decimal)_value, 3);
            currentPercentage = (currentValue - minScope) / (maxScope - minScope);
        }

        public void CreateKeyAssignmentString()
        {
            System.Text.StringBuilder bindingNameBuilder = new System.Text.StringBuilder();
            System.Text.StringBuilder inputNameBuilder = new System.Text.StringBuilder();

            KeyBinding[] bindings = midiTweak.ConfigKeyBindings.Bindings;

            for (int i = 0; i < bindings.Length; i++)
            {
                bindingNameBuilder.AppendFormat("{0} \n", bindings[i].Name);
                inputNameBuilder.AppendFormat("{0} \n", (!bindings[i].HasAssignedInput()) ? "None" : bindings[i].GetAssignedInput().ToString());
            }

            keyBindingNames = bindingNameBuilder.ToString();
            keyBindingInputs = inputNameBuilder.ToString();
        }

        private void DrawSlider()
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            // TODO: Fix this when Unity has a better solution to hide a slider field
            EditorGUIUtility.fieldWidth = 0.01f;

            EditorGUI.BeginChangeCheck();

            currentPercentage = EditorGUILayout.Slider(currentPercentage, 0, 1, GUILayout.Width(210));

            if (EditorGUI.EndChangeCheck())
            {
                midiTweak.SetValueByPercentage(currentPercentage, parameterIndex);
            }

            EditorGUIUtility.fieldWidth = 0;
            GUILayout.EndHorizontal();
        }

        private void DisplayParameters()
        {
            GUILayout.BeginVertical();

            DrawLabel("Parameters", alignCenterStyle);

            scrollViewPosition = GUILayout.BeginScrollView(scrollViewPosition, EditorStyles.helpBox, GUILayout.ExpandHeight(true));

            for (int i = 0; i < bookMarkStrings.Length; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);

                if (i != parameterIndex)
                {
                    if (GUILayout.Button(bookMarkStrings[i], EditorStyles.label))
                    {
                        midiTweak.SetActiveParameter(i);
                        midiTweak.SetValueByPercentage(0.5f, i);
                        midiTweak.SetFrozen(false);
                    }
                }
                else
                {
                    GUILayout.Label(bookMarkStrings[i], EditorStyles.boldLabel);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndScrollView();
        }

        private void DisplayBindings()
        {
            DrawLabel("Bindings", alignCenterStyle);

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.Label(keyBindingNames);

            GUILayout.FlexibleSpace();

            GUILayout.Label(keyBindingInputs, alignRightStyle);
            GUILayout.EndHorizontal();
        }

        private void DisplayTweakInterface()
        {
            DrawLabel("Tweak Tools", alignCenterStyle);

            GUILayout.BeginVertical(EditorStyles.helpBox);

            if (bookMarkStrings.Length > 0)
            {
                DrawLabel(bookMarkStrings[parameterIndex], alignCenterStyle, 300);
            }

            DrawScopeValues();

            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            DrawZoomPercentage("-");

            DrawSlider();

            DrawZoomPercentage("+");
            GUILayout.EndHorizontal();
            //
            if (newzoom == zoom)
            {
                midiTweak.FreezeCurrentValue(false);
                midiTweak.SetValueByPercentage(0.50f, parameterIndex);
                midiTweak.SetZoomRange((int)newzoom);
                newzoom = -1;
            }

            string currentValueString = string.Format("{0} ({1})", currentValue.ToString(), currentPercentage.ToString("00%"));

            DrawLabel(currentValueString, alignCenterStyleBold, 100);

            GUILayout.Space(5);

            // Draw button to freeze value
            if (!isFrozen)
            {
                if (GUILayout.Button("Freeze Value", GUILayout.MinHeight(25)))
                {
                    midiTweak.FreezeCurrentValue(false);
                    midiTweak.SetValueByPercentage(0.50f, parameterIndex);
                    midiTweak.SetFrozen(false);
                }
            }
            else
            {
                DrawLabel("Recenter to start tweaking", alignCenterStyle, 200);
            }

            GUILayout.EndVertical();
        }

        private void DrawScopeValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(50);

            EditorGUILayout.LabelField(minScope.ToString(), GUILayout.Width(50));

            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();

            midScope = EditorGUILayout.FloatField(midScope, alignCenterFloatField, GUILayout.Width(50));

            if (EditorGUI.EndChangeCheck())
            {
                midiTweak.SetNewMidValue(midScope, true);
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField(maxScope.ToString(), alignRightStyle, GUILayout.Width(50));
            GUILayout.Space(50);
            GUILayout.EndHorizontal();
        }

        private static void DrawLabel(string _title, GUIStyle _style, float _width = 100)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(_title, _style, GUILayout.Width(_width));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }


        private void DrawZoomPercentage(string _postfix)
        {
            EditorGUI.BeginChangeCheck();

            // Draw slider and zoom percentages
            EditorGUILayout.LabelField(string.Format(_postfix), GUILayout.Width(15));
            zoom = EditorGUILayout.IntField((int)zoom, GUILayout.Width(40));
            EditorGUILayout.LabelField(string.Format("%"), GUILayout.Width(15));
            GUILayout.FlexibleSpace();

            if (EditorGUI.EndChangeCheck())
            {
                newzoom = (int)zoom;
            }
        }
    }
}