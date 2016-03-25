#if WINDOWS_FORM
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Windows mouse movements and properties control support class.
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
    public class Mouse
    {
        public Mouse() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static Point Position()
        {
            return Cursor.Position;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Position(out int x, out int y)
        {
            x = Cursor.Position.X;
            y = Cursor.Position.Y;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Position(int x, int y)
        {
            Cursor.Position = new Point(x, y);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Move(Point p)
        {
            Move(p.X, p.Y);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Move(int x, int y)
        {
            int x_orig, y_orig;
            int x_path, y_path;
            int step;

            x_orig = Cursor.Position.X;
            y_orig = Cursor.Position.Y;

            step = (int)Math.Sqrt(Math.Pow(x - x_orig, 2) + Math.Pow(y - y_orig, 2));

            step /= 10;

            for (int i = 1; i <= step; i++)
            {
                x_path = x_orig + i * (x - x_orig) / step;
                y_path = y_orig + i * (y - y_orig) / step;

                Cursor.Position = new Point(x_path, y_path);
                Thread.Sleep(30);
            }

            Cursor.Position = new Point(x, y);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Wait(int millisecond)
        {
            Thread.Sleep(millisecond);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ClickLeftSingle()
        {
            global::Ia.Cl.Model.Winapi.SendLeftClick();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ClickLeftDouble()
        {
            global::Ia.Cl.Model.Winapi.SendLeftClick();
            Thread.Sleep(100);
            global::Ia.Cl.Model.Winapi.SendLeftClick();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ClickLeftDragMouse(int x, int y)
        {
            global::Ia.Cl.Model.Winapi.SendLeftDownClick();
            Move(x, y);
            global::Ia.Cl.Model.Winapi.SendLeftUpClick();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ClickRightSingle()
        {
            global::Ia.Cl.Model.Winapi.SendRightClick();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void ClickRightDouble()
        {
            global::Ia.Cl.Model.Winapi.SendRightClick();
            Thread.Sleep(100);
            global::Ia.Cl.Model.Winapi.SendRightClick();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Key(string n)
        {
            // print string
            // SendKeys.Send(n);
            SendKeys.SendWait(n);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
#endif
