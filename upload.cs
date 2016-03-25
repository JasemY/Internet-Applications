using System;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Configuration;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Handle uploading functions.
    /// </summary>
    /// <value>
    /// Make sure you set permissions for IUSR and IWP to Read and Write for upload directory.
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
    public class Upload
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Upload() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool File(string path_file)
        {
            return File(path_file, true);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool File(string absolute_or_relative_path_file, bool use_temporary_folder)
        {
            bool b;
            string absolute_path, directory;

            b = false;

            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            HttpPostedFile hpf = hfc[0];

            /*

#if WINDOWS_FORM
                absolute_path = AppDomain.CurrentDomain.BaseDirectory.ToString() + @"app_data\temp\";
#else
                if (hpf.ContentLength > 0)
                {
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) absolute_path = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory + @"\";
                    else absolute_path = AppDomain.CurrentDomain.BaseDirectory;


                    hpf.SaveAs(absolute_path + relative_path_file);
                    b = true;
                }
#endif
             */

            try
            {
                // this will detect the initial "C:\" in the path_file to decide if it is absolute or relative
                if (absolute_or_relative_path_file.Contains(@":\"))
                {
                    // absolute path file
                    absolute_path = absolute_or_relative_path_file;
                }
                else
                {
                    absolute_path = global::Ia.Cl.Model.Default.AbsolutePath(use_temporary_folder);

                    absolute_path += absolute_or_relative_path_file;
                }

                directory = absolute_path.Substring(0, absolute_path.LastIndexOf(@"\"));

                if (!Directory.Exists(directory))
                {
                    // create the directory it does not exist.
                    Directory.CreateDirectory(directory);
                }

                hpf.SaveAs(absolute_path);
                b = true;

                /*
                using (FileStream fs = new FileStream(absolute_path, FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.WriteLine(line);
                        b = true;
                    }
                }
                */
            }
            catch (Exception)
            {
                b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Read and file into a stream
        /// </summary>
        public static StreamReader Stream()
        {
            StreamReader sr;
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            HttpPostedFile hpf = hfc[0];

            sr = null;

            try
            {
                sr = new StreamReader(hpf.InputStream);
            }
            catch (Exception)
            {
            }
            finally
            {
            }

            return sr;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool File(string path, int num)
        {
            return File(path, num, true);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool File(string file, int num, bool temp_folder)
        {
            bool b;
            int i, n, l;
            string path, path_file/*, ext*/;

            b = false;
            n = 0;
            //path = null;

            HttpFileCollection hfc = HttpContext.Current.Request.Files;

            for (i = 0; i < hfc.Count; i++)
            {
                path_file = "";

                HttpPostedFile hpf = hfc[i];

                l = hpf.FileName.LastIndexOf('.') + 1;

                //ext = hpf.FileName.Substring(l, hpf.FileName.Length - l);

                try
                {
                    if (hpf.ContentLength > 0)
                    {
                        if (temp_folder)
                        {
                            path = global::Ia.Cl.Model.Default.AbsolutePath(temp_folder);

                            path_file = path + file + "_" + num + "_" + n;// + "." + ext;
                        }
                        else
                        {
                            path = global::Ia.Cl.Model.Default.AbsolutePath();

                            path_file = path + file + "_" + num + "_" + n;// +"." + ext;
                        }

                        hpf.SaveAs(path_file);
                        b = true;
                        n++;
                    }
                }
                catch (Exception)
                {
                    b = false;
                }
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Media(string path, int num)
        {
            Media(path, num, true, false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Media(string relative_path, int num, bool temp_folder, bool overwrite)
        {
            int i, n;
            string relative_path_file, r;

            n = 0;

            HttpFileCollection hfc = HttpContext.Current.Request.Files;

            for (i = 0; i < hfc.Count; i++)
            {
                HttpPostedFile hpf = hfc[i];

                try
                {
                    if (hpf.ContentLength > 0)
                    {
                        // check if the path_file is an image
                        if (hpf.ContentType == "image/jpeg" || hpf.ContentType == "image/pjpeg" || hpf.ContentType == "image/gif")
                        {
                            relative_path_file = relative_path + num + "_" + n + ".jpg";

                            if (!overwrite)
                            {
                                while (global::Ia.Cl.Model.File.Exists(relative_path_file))
                                {
                                    relative_path_file = relative_path + num + "_" + ++n + ".jpg";
                                }
                            }

                            relative_path_file = relative_path_file.Replace(".jpg", "_temp.jpg");

                            // replace the steps below with a stream operation
                            hpf.SaveAs(global::Ia.Cl.Model.Default.AbsolutePath() + relative_path_file);

                            global::Ia.Cl.Model.Image.CreateGeneralUseImages(relative_path_file, relative_path, num, n);

                            global::Ia.Cl.Model.File.Delete(relative_path_file);

                            n++;
                        }
                        else if (hpf.ContentType == "audio/x-pn-realaudio")
                        {
                        }
                        else if (hpf.ContentType == "video/mp4")
                        {
                            relative_path_file = relative_path + num + "_" + n + ".mp4";

                            if (!overwrite)
                            {
                                while (global::Ia.Cl.Model.File.Exists(relative_path_file))
                                {
                                    relative_path_file = relative_path + num + "_" + ++n + ".mp4";
                                }
                            }

                            // replace the steps below with a stream operation
                            hpf.SaveAs(global::Ia.Cl.Model.Default.AbsolutePath() + relative_path_file);

                            n++;
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
#if DEBUG
                    r = "Error: " + ex.ToString();
#else
                    r = "Error: " + ex.Message;
#endif
                }
                finally
                {
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
