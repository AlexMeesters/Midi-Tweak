using System;
using System.Collections.Generic;

namespace Lowscope.Miditweak
{
    /// <summary>
    /// Defines how many frames it takes before the midi input switches.
    /// This is beneficial to prevent accidental midi input switches.
    /// </summary>

    public enum EMidiActivationTolerance
    {
        Instant,
        High,
        Low
    }
}
