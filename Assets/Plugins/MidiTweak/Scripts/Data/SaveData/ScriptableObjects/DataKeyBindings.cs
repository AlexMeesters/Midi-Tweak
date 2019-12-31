using UnityEngine;
using System.Collections;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Storage for all the key binding configurations that the user will apply.
    /// </summary>

    [CreateAssetMenu(fileName = "DataKeyBindings", menuName = "Miditweak/DataKeyBindings")]
    public class DataKeyBindings : ScriptableObject
    {
        public KeyBinding IncreaseRange = new KeyBinding("Increase Zoom");
        public KeyBinding DecreaseRange = new KeyBinding("Decrease Zoom");
        public KeyBinding FreezeValue = new KeyBinding("Freeze Value");
        public KeyBinding SwitchParameterForward = new KeyBinding("Switch Parameter Forward");
        public KeyBinding SwitchParameterBackward = new KeyBinding("Switch Parameter Backward");
        public KeyBinding ToggleMenuVisiblity = new KeyBinding("Toggle Menu Visibility");

        public KeyBinding[] Bindings
        {
            get
            {
                return new KeyBinding[6]
                {
                    IncreaseRange,
                    DecreaseRange,
                    FreezeValue,
                    SwitchParameterForward,
                    SwitchParameterBackward,
                    ToggleMenuVisiblity
                };
            }
        }
    }
}
