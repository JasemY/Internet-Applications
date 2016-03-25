#if WINDOWS_FORM
using System;
using System.Runtime.InteropServices;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Sound support class.
    /// </summary>
    /// <remarks> 
    /// Copyright © 2001-2015 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
    ///
    /// This library is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
    /// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
    ///
    /// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
    /// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
    /// 
    /// You should have received a copy of the GNU General Public License along with this library. If not, see http://www.gnu.org/licenses.
    /// 
    /// Copyright notice: This notice may not be removed or altered from any source distribution.
    /// </remarks>
    public abstract class Sound
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// </summary>
        public Sound() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        [DllImport("WinMM.dll")]
        public static extern bool PlaySound(string fname, int Mod, int flag);

        // these are the SoundFlags we are using here, check mmsystem.h for more 
        private static int SND_PURGE = 0x0040; // purge non-static events 
        private static int SND_ASYNC = 0x0001; // play asynchronously 
        //private static int SND_SYNC = 0x0000; // play synchronously (default) 
        private static int SND_NODEFAULT = 0x0002; // silence (!default) if sound not found 
        //private static int SND_MEMORY = 0x0004; // pszSound points to a memory file 
        //private static int SND_LOOP = 0x0008; // loop the sound until next sndPlaySound 
        //private static int SND_NOSTOP = 0x0010; // don't stop any currently playing sound 
        private static int SND_NOWAIT = 0x00002000; // don't wait if the driver is busy 
        //private static int SND_ALIAS = 0x00010000; // name is a registry alias 
        //private static int SND_ALIAS_ID = 0x00110000; // alias is a predefined ID 
        private static int SND_FILENAME = 0x00020000; // name is file name 
        //private static int SND_RESOURCE = 0x00040004; // name is resource name or atom 

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Play(string fname)
        {
            //StopPlay(); 
            PlaySound(fname, 0, SND_ASYNC | SND_NODEFAULT | SND_FILENAME | SND_NOWAIT);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Stop()
        {
            PlaySound(null, 0, SND_PURGE);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
#endif