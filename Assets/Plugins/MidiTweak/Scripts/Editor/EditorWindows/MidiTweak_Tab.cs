
namespace Lowscope.Miditweak
{
    /// <summary>
    /// Base class for all tabs
    /// </summary>
    public abstract class MidiTweak_Tab
    {
        protected MidiTweak midiTweak;

        public void OpenTab(MidiTweak _midiTweak)
        {
            if (midiTweak == null)
            {
                midiTweak = _midiTweak;
            }

            OnTabOpened();
        }

        public void DisplayTab()
        {
            OnDisplayTab();
        }

        public abstract void OnDisplayTab();
        public abstract void OnTabOpened();
        public abstract void OnTabClosed();
    }
}