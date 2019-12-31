using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Loads in the current key binding configuration once the game is loaded.
    /// </summary>

    [AddComponentMenu("")]
    public class UITweakKeyAssignmentList : MonoBehaviour, ILoadKeyAssignmentList
    {
        [SerializeField]
        private Text bindingKeyIndicatorText = null,  bindingValueIndicatorText = null;

        public void OnKeyAssignmentsLoaded(KeyBinding[] _bindingData)
        {
            StringBuilder bindingKeyStringBuilder = new StringBuilder();
            StringBuilder bindingValueStringBuilder = new StringBuilder();

            for (int i = 0; i < _bindingData.Length; i++)
            {
                bindingKeyStringBuilder.AppendFormat("{0} \n", _bindingData[i].Name);

                object getAssignedInput = _bindingData[i].GetAssignedInput();
                string getAssignedInputName = (getAssignedInput != null) ? getAssignedInput.ToString() : "None";

                bindingValueStringBuilder.AppendFormat("{0} \n", getAssignedInputName);
            }

            if (bindingKeyIndicatorText != null)
            {
                bindingKeyIndicatorText.text = bindingKeyStringBuilder.ToString();
            }

            if (bindingValueIndicatorText != null)
            {
                bindingValueIndicatorText.text = bindingValueStringBuilder.ToString();
            }
        }
    }

}
