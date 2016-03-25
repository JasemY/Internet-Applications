using System;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;
using System.Data;

#if WINDOWS_FORM
using System.Windows.Forms;
#endif

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// File manipulation related support class.
    /// 
    /// </summary>
    /// <value>
    /// Unify path detection and handling into a single private function
    /// </value>
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
    public class File
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public File() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Read(string file)
        {
            return Read(file, false, System.Text.Encoding.UTF8);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Read(string file, System.Text.Encoding encoding)
        {
            return Read(file, false, encoding);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Read(string absoluteOrRelativePathFile, bool useTemporaryFolder, System.Text.Encoding encoding)
        {
            // read text from file
            string absolutePath;
            string sa;
            StringBuilder sb = new StringBuilder(100000);
            StreamReader sr = null;

            absolutePath = null;

            try
            {
                // this will detect the initial "C:\" in the path_file to decide if it is absolute or relative
                if (absoluteOrRelativePathFile.Contains(@":\"))
                {
                    // absolute path file
                    absolutePath = absoluteOrRelativePathFile;
                }
                else
                {
                    absolutePath = global::Ia.Cl.Model.Default.AbsolutePath(useTemporaryFolder);

                    absolutePath += absoluteOrRelativePathFile;
                }

                if (System.IO.File.Exists(absolutePath))
                {
                    using (sr = new StreamReader(absolutePath, encoding))
                    {
                        while ((sa = sr.ReadLine()) != null) sb.Append(sa + "\r\n");
                    }
                }
                else { }
            }
            catch (Exception)
            {
#if DEBUG
#else
#endif
            }
            finally { sr.Close(); }

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Create(string file, string line)
        {
            return Create(file, line, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Create(string absoluteOrRelativePathFile, string line, bool useTemporaryFolder)
        {
            // create text to file
            bool done = false;
            string absolutePath;
            StreamWriter sw; // = null;

            absolutePath = global::Ia.Cl.Model.Default.AbsolutePath(useTemporaryFolder);

            absolutePath += absoluteOrRelativePathFile;

            using (sw = new StreamWriter(absolutePath, false, Encoding.UTF8))
            {
                sw.Write(line);
                done = true;
            }

            /*
            below: this does not product good UTF8 encoding
      
            using(sw = System.IO.File.CreateText(path))
            {
              sw.WriteLine(line);
              done = true;
            }    
            */

            return done;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Write(string absoluteOrRelativePathFile, string line)
        {
            return Write(absoluteOrRelativePathFile, line, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Write(string absoluteOrRelativePathFile, string line, bool useTemporaryFolder)
        {
            // write text to and existing file

            bool b;
            string absolutePath, directory;

            b = false;
            absolutePath = null;

            try
            {
                // this will detect the initial "C:\" in the path_file to decide if it is absolute or relative
                if (absoluteOrRelativePathFile.Contains(@":\"))
                {
                    // absolute path file
                    absolutePath = absoluteOrRelativePathFile;
                }
                else
                {
                    absolutePath = global::Ia.Cl.Model.Default.AbsolutePath(useTemporaryFolder);

                    absolutePath += absoluteOrRelativePathFile;
                }

                directory = absolutePath.Substring(0, absolutePath.LastIndexOf(@"\"));

                if (!Directory.Exists(directory))
                {
                    // create the directory it does not exist.
                    Directory.CreateDirectory(directory);
                }

                // remove the readonly attribute if file exists
                if (File.Exists(absolutePath))
                {
                    FileAttributes attributes = System.IO.File.GetAttributes(absolutePath);

                    if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attributes = attributes & ~FileAttributes.ReadOnly;
                        System.IO.File.SetAttributes(absolutePath, attributes);
                    }
                }

                using (FileStream fs = new FileStream(absolutePath, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(line);
                        b = true;
                    }
                }
            }
            catch (Exception)
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Append(string absoluteOrRelativePathFile, string line)
        {
            return Append(absoluteOrRelativePathFile, line, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Append(string absoluteOrRelativePathFile, string line, bool useTemporaryFolder)
        {
            // append text to file
            bool done = false;
            string absolutePath;
            StreamWriter sw = null;

            absolutePath = global::Ia.Cl.Model.Default.AbsolutePath(useTemporaryFolder);

            absolutePath += absoluteOrRelativePathFile;

            if (System.IO.File.Exists(absolutePath))
            {
                using (sw = System.IO.File.AppendText(absolutePath))
                {
                    sw.WriteLine(line);
                    done = true;
                }
            }
            else done = false;

            return done;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Delete(string absoluteOrRelativePathFile)
        {
            // delete file
            bool done = false;
            string absolutePath;

            // this will detect the initial "C:\" in the path_file to decide if it is absolute or relative
            if (absoluteOrRelativePathFile.Contains(@":\"))
            {
                // absolute path file
                absolutePath = absoluteOrRelativePathFile;
            }
            else
            {
                // relative path file
                absolutePath = global::Ia.Cl.Model.Default.AbsolutePath() + absoluteOrRelativePathFile;
            }

            if (System.IO.File.Exists(absolutePath))
            {
                System.IO.File.Delete(absolutePath);
                done = true;
            }
            else done = false;

            return done;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Move(string absoluteOrRelativeSourceFileName, string absoluteOrRelativeDestinationFileName)
        {
            // move (rename) file
            bool done;
            string absoluteSourceFileName, absoluteDestinationFileName;

            // this will detect the initial "C:\" in the path_file to decide if it is absolute or relative
            if (absoluteOrRelativeSourceFileName.Contains(@":\")) absoluteSourceFileName = absoluteOrRelativeSourceFileName;
            else absoluteSourceFileName = global::Ia.Cl.Model.Default.AbsoluteTempPath() + absoluteOrRelativeSourceFileName;

            if (absoluteOrRelativeDestinationFileName.Contains(@":\")) absoluteDestinationFileName = absoluteOrRelativeDestinationFileName;
            else absoluteDestinationFileName = global::Ia.Cl.Model.Default.AbsoluteTempPath() + absoluteOrRelativeDestinationFileName;

            if (System.IO.File.Exists(absoluteSourceFileName) && !System.IO.File.Exists(absoluteDestinationFileName))
            {
                System.IO.File.Move(absoluteSourceFileName, absoluteDestinationFileName);
                done = true;
            }
            else done = false;

            return done;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Exists(string file)
        {
            return Exists(file, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool Exists(string absoluteOrRelativePathFile, bool useTemporaryFolder)
        {
            // check if file exists
            bool done = false;
            string absolutePath;

            absolutePath = global::Ia.Cl.Model.Default.AbsolutePath(useTemporaryFolder);

            absolutePath += absoluteOrRelativePathFile;

            if (System.IO.File.Exists(absolutePath)) done = true;
            else done = false;

            return done;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static StreamReader OpenText(string file)
        {
            // 
            string path;
            StreamReader sr;

            sr = null;

            path = global::Ia.Cl.Model.Default.AbsolutePath();

            path = path + file;

            sr = System.IO.File.OpenText(path);

            return sr;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Count the number of files within path that conform to the regular expression passed in file
        /// <param name="file">File name (could be a regular expression)</param>
        /// <param name="path">Directory within which to search</param>
        /// <returns>int value of number of occurances of file withing path</returns>
        /// </summary>
        public static int Count(string path, string file)
        {
            int c;
            FileInfo[] fip;

            fip = Info(path, file);

            if (fip != null) c = fip.Length;
            else c = 0;

            return c;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the files within path that conform to the regular expression passed in file
        /// <param name="file">File name (could be a regular expression)</param>
        /// <param name="path">Directory within which to search</param>
        /// <returns>FileInfo[] of files within directory</returns>
        /// </summary>
        public static FileInfo[] Info(string path, string file)
        {
            string absolutePath;
            FileInfo[] fip;
            DirectoryInfo di;

            absolutePath = "";
            fip = null;

            try
            {
#if WINDOWS_FORM
#else
                absolutePath = AppDomain.CurrentDomain.BaseDirectory.ToString();
#endif
                absolutePath += path;

                di = new DirectoryInfo(absolutePath);
                fip = di.GetFiles(file);
            }
            catch (Exception)
            {
                //throw(Exception ERRORMAN); // result_l.Text += " The process failed: "+e.ToString();
            }

            return fip;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        public static bool IsValidFilename(string testName)
        {
            bool b;

            Regex containsABadCharacter = new Regex("[" + Regex.Escape(System.IO.Path.GetInvalidFileNameChars().ToString()) + "]");

            if (containsABadCharacter.IsMatch(testName)) b = false;
            else b = true;

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
