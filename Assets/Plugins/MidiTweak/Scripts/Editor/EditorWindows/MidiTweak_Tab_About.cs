using UnityEngine;
using System.Collections;

namespace Lowscope.Miditweak
{
    public class MidiTweak_Tab_About : MidiTweak_Tab
    {
        public override void OnDisplayTab()
        {
            GUILayout.Label("Developed by Lowscope");

        }

        public override void OnTabClosed()
        {
 
        }

        public override void OnTabOpened()
        {

        }
    }
}