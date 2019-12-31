using UnityEngine;
using System.Collections;

namespace Lowscope.Miditweak
{
    public abstract class InputProcessor
    {
        public abstract void Start(MidiTweak _midiTweak);
        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void Update();
    }
}
