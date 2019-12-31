using UnityEngine;


namespace Lowscope.Miditweak
{
    /// <summary>
    /// Key binding data container to store input (Midi/Keyboard)
    /// and use it to utilize functionality during runtime. Such as switching parameters.
    /// </summary>

    [System.Serializable]
    public class KeyBinding
    {
        [SerializeField]
        private int knobID;

        [SerializeField]
        private KeyCode keyCode;

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        public enum InputBindingType { None, Midi, Keyboard };

        public KeyBinding(string _name, int _knobID = -1, KeyCode _keyCode = KeyCode.None)
        {
            name = _name;
            knobID = -1;
            keyCode = KeyCode.None;
        }

        public bool IsPressed(int _note = -1)
        {
            if (HasAssignedInput())
            {
                if (ReturnBindingType() != KeyBinding.InputBindingType.Midi)
                {
                    if (Input.GetKeyDown(GetKeyCode()))
                    {
                        return true;
                    }
                }
                else
                {
                    if (_note == (int)GetMidiInput())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public KeyCode GetKeyCode()
        {
            return keyCode;
        }

        public int GetMidiInput()
        {
            return knobID;
        }

        public bool DoesMidiInputMatch(uint _input)
        {
            return _input == knobID;
        }

        public bool DoesKeyboardInputMatch(KeyCode _input)
        {
            if (_input == KeyCode.None)
            {
                return false;
            }

            return _input == keyCode;
        }

        public void AssignMidiInput(int _input)
        {
            if (HasAssignedInput())
            {
                ClearInput();
            }

            knobID = _input;
        }

        public void AssignKeyBoardInput(KeyCode _input)
        {
            if (_input == KeyCode.None)
            {
                return;
            }

            if (HasAssignedInput())
            {
                ClearInput();
            }

            keyCode = _input;
        }

        public bool HasAssignedInput()
        {
            return (knobID != -1 || keyCode != KeyCode.None);
        }

        public object GetAssignedInput()
        {
            if (ReturnBindingType() == InputBindingType.Keyboard)
            {
                return keyCode;
            }
            else
            {
                if (ReturnBindingType() == InputBindingType.Midi)
                {
                    return knobID;
                }
            }

            return null;
        }

        public InputBindingType ReturnBindingType()
        {
            if (knobID != -1)
            {
                return InputBindingType.Midi;
            }

            if (keyCode != KeyCode.None)
            {
                return InputBindingType.Keyboard;
            }

            return InputBindingType.None;
        }

        public void ClearInput()
        {
            knobID = -1;
            keyCode = KeyCode.None;
        }

        public string ReturnAssignmentStatus()
        {
            if (HasAssignedInput())
            {
                if (knobID != -1)
                {
                    return knobID.ToString();
                }
                else
                {
                    return keyCode.ToString();
                }
            }
            else
            {
                return "None";
            }
        }
    }
}