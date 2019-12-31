using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Used to display all parameters
    /// </summary>

    [AddComponentMenu("")]
    public class UITweakParameterList : MonoBehaviour, IListenToParameterChange, IListenToParameterLoad
    {
        [SerializeField]
        private Transform variableContainer = null;

        [SerializeField]
        private Text pageIndicator = null;

        private int pageListSize;
        private int currentPage = 0;

        private Text[] parameterDisplayers;
        private string[] paramterNameStrings;

        private void Awake()
        {
            if (variableContainer != null)
            {
                parameterDisplayers = variableContainer.GetComponentsInChildren<Text>();
            }

            pageListSize = parameterDisplayers.Length;
        }

        public void OnLoadParameters(MidiBookmark[] _parameters)
        {
            string[] parameterNames = new string[_parameters.Length];

            for (int i = 0; i < _parameters.Length; i++)
            {
                parameterNames[i] = string.Format("{0} - {1}", _parameters[i].Component.ToString(), _parameters[i].Field.fieldName);
            }

            paramterNameStrings = parameterNames;
        }

        public void OnParameterChange(string _gameObject, string _name, int _index, int _midiIndex)
        {
            this.gameObject.SetActive(true);

            int page = ((_index == 0) ? 0 : Mathf.FloorToInt((int)_index / pageListSize));

            if (pageIndicator != null)
            {
                pageIndicator.text = string.Format("{0}/{1}", (_index + 1) - (page*pageListSize) , page + 1);
            }

            // The index of the parameter displayers will always have the length of the pageListSize...

            if ((int)_index <= paramterNameStrings.Length)
            {
                int startIndex = page * pageListSize;

                for (int i = 0; i < pageListSize; i++)
                {
                    if (i >= parameterDisplayers.Length)
                    {
                        continue;
                    }

                    // Set the parameter display texts to match the parameter names.
                    if (i + startIndex < paramterNameStrings.Length)
                    {
                        parameterDisplayers[i].text = paramterNameStrings[i + startIndex];
                    }
                    else
                    {
                        parameterDisplayers[i].text = "-";

                    }

                    // If the index matches, make it bold.
                    if (i + startIndex == (int)_index)
                    {
                        parameterDisplayers[i].fontStyle = FontStyle.Bold;
                        parameterDisplayers[i].color = Color.white;
                    }
                    else
                    {
                        parameterDisplayers[i].fontStyle = FontStyle.Italic;
                        parameterDisplayers[i].color = Color.white;
                    }
                }

            }
        }
    }

}