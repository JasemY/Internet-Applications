#if WINDOWS_FORM
using System;
using System.Runtime.InteropServices;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// WINAPI click events support class.
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
    public class Winapi
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(UInt32 dwFlags, UInt32 dx, UInt32 dy, UInt32 dwData, IntPtr dwExtraInfo);

        //DWORD dwData; // amount of wheel movement

        private const UInt32 MouseEventLeftDown = 0x0002;
        private const UInt32 MouseEventLeftUp = 0x0004;

        private const UInt32 MouseEventRightDown = 0x0008;
        private const UInt32 MouseEventRightUp = 0x0010;

        private const UInt32 MouseEventWheelUp = 128;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendDoubleClick()
        {
            mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
            mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendRightClick()
        {
            mouse_event(MouseEventRightDown, 0, 0, 0, new System.IntPtr());
            mouse_event(MouseEventRightUp, 0, 0, 0, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendLeftClick()
        {
            mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
            mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendLeftDownClick()
        {
            mouse_event(MouseEventLeftDown, 0, 0, 0, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendLeftUpClick()
        {
            mouse_event(MouseEventLeftUp, 0, 0, 0, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void SendWheelUpClick()
        {
            mouse_event(MouseEventWheelUp, 0, 0, 1, new System.IntPtr());
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
#endif