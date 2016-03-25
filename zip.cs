using System;
using System.Web;
using System.Xml;
using System.IO;
using System.Text;
using Ionic.Zip;

namespace Ia.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// ZIP support class. Compress and de-compress a string using ZIP technology (requires ICSharpCode.SharpZipLib.dll)
    /// This code was updated but not tested.
    /// </summary>
    /// 
    /// <remarks>
    /// Copyright © 2008-2013 Jasem Y. Al-Shamlan (info@ia.com.kw), Internet Applications - Kuwait. All Rights Reserved.
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
    public class Zip
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Zip() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Compress(FileInfo fi)
        {
            using (ZipFile zip = new ZipFile())
            {
                // add this map file into the "images" directory in the zip archive
                zip.AddFile("c:\\images\\personal\\7440-N49th.png", "images");
                // add the report into a different directory in the archive
                zip.AddFile("c:\\Reports\\2008-Regional-Sales-Report.pdf", "files");
                zip.AddFile("ReadMe.txt");
                zip.Save("MyZipFile.zip");
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Decompress(FileInfo fi)
        {
            string zipToUnpack = fi.FullName;
            string unpackDirectory = "Extracted Files";
            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
            {
                // below: we extract every entry, but we could extract conditionally based on entry name, size, date, checkbox status, etc.  
                foreach (ZipEntry e in zip1)
                {
                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
