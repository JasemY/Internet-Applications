using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections;
using System.Configuration;
using System.Xml;
using System.Text;
using System.Data;
using System.Security.Cryptography;

#if WINDOWS_FORM
using System.ComponentModel;
using System.Windows.Forms;
#else
using System.Web;
#endif

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Image processing support class.
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
    public class Image
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Image()
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void CreateGeneralUseImages(string pathSourceFile)
        {
            CreateGeneralUseImages(pathSourceFile, pathSourceFile, -1, -1);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void CreateGeneralUseImages(string pathSourceFile, string pathDestinationPath)
        {
            CreateGeneralUseImages(pathSourceFile, pathDestinationPath, -1, -1);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void CreateGeneralUseImages(string pathSourceFile, string pathDestinationPath, int num, int suffix)
        {
            // resize main image and generate images of the different formats
            string name, extension;//, absolutePath;

            // we will check if paths are absolute or relative and adjust them accordingly.

            pathSourceFile = AbsolutePath(pathSourceFile);
            pathDestinationPath = AbsolutePath(pathDestinationPath);

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(pathSourceFile);

                extension = System.IO.Path.GetExtension(pathSourceFile);
                extension = extension.ToLower();

                // this solves a bug:
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                image.RotateFlip(RotateFlipType.Rotate180FlipNone);

                // below; this generates images that assume the original image has dimentions ration of 4:3 (1024:768)
                // generate tiny, small, middle, and large size images and the sample image:

                // this will generate images that have a fixed "area". This will preserve the original width-height ratio, but will
                // also give a scaled, suitable versions:

                // note that if w0*h0=A0 and w*h=Af, then h=h0*(sqrt(Af))/(sqrt(A)) and  w=w0*(sqrt(Af))/(sqrt(A))

                double w, h, A, Af, r, w2, h2;
                w = image.Width; h = image.Height;
                A = w * h;

                System.Drawing.Image.GetThumbnailImageAbort dummyCallBack = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallback);

                Af = 640 * 480;
                if (Af > A) Af = A;
                r = Math.Sqrt(Af) / Math.Sqrt(A);
                System.Drawing.Image very_large = image.GetThumbnailImage(Convert.ToInt32(w * r), Convert.ToInt32(h * r), dummyCallBack, IntPtr.Zero);

                Af = 400 * 300;
                if (Af > A) Af = A;
                r = Math.Sqrt(Af) / Math.Sqrt(A);
                System.Drawing.Image large = image.GetThumbnailImage(Convert.ToInt32(w * r), Convert.ToInt32(h * r), dummyCallBack, IntPtr.Zero);

                Af = 200 * 150;
                if (Af > A) Af = A;
                r = Math.Sqrt(Af) / Math.Sqrt(A);
                System.Drawing.Image middle = image.GetThumbnailImage(Convert.ToInt32(w * r), Convert.ToInt32(h * r), dummyCallBack, IntPtr.Zero);

                Af = 120 * 90;
                if (Af > A) Af = A;
                r = Math.Sqrt(Af) / Math.Sqrt(A);
                System.Drawing.Image small = image.GetThumbnailImage(Convert.ToInt32(w * r), Convert.ToInt32(h * r), dummyCallBack, IntPtr.Zero);

                Af = 80 * 60;
                if (Af > A) Af = A;
                r = Math.Sqrt(Af) / Math.Sqrt(A);
                System.Drawing.Image tiny = image.GetThumbnailImage(Convert.ToInt32(w * r), Convert.ToInt32(h * r), dummyCallBack, IntPtr.Zero);

                // this saves an exact size version:
                System.Drawing.Image same = image.GetThumbnailImage(Convert.ToInt32(w), Convert.ToInt32(h), dummyCallBack, IntPtr.Zero);

                // the methods below will generate images with fixed width and varying high. Width range from 75 to 500.
                System.Drawing.Image image_500, image_400, image_250, image_100, image_75;

                w2 = 500;
                h2 = w2 * h / w;
                image_500 = image.GetThumbnailImage(Convert.ToInt32(w2), Convert.ToInt32(h2), dummyCallBack, IntPtr.Zero);

                w2 = 400;
                h2 = w2 * h / w;
                image_400 = image.GetThumbnailImage(Convert.ToInt32(w2), Convert.ToInt32(h2), dummyCallBack, IntPtr.Zero);

                w2 = 250;
                h2 = w2 * h / w;
                image_250 = image.GetThumbnailImage(Convert.ToInt32(w2), Convert.ToInt32(h2), dummyCallBack, IntPtr.Zero);

                w2 = 100;
                h2 = w2 * h / w;
                image_100 = image.GetThumbnailImage(Convert.ToInt32(w2), Convert.ToInt32(h2), dummyCallBack, IntPtr.Zero);

                w2 = 75;
                h2 = w2 * h / w;
                image_75 = image.GetThumbnailImage(Convert.ToInt32(w2), Convert.ToInt32(h2), dummyCallBack, IntPtr.Zero);

                if (num < 0 && suffix < 0)
                {
                    name = pathDestinationPath.Replace(".jpg", "");
                }
                else if (num < 0) name = pathDestinationPath + "_" + suffix;
                else if (suffix < 0) name = pathDestinationPath + "_" + num;
                else name = pathDestinationPath + num + "_" + suffix;

                // 

                //middle = ImageShadow.OnPaint(middle);

                // save new images:

                if (extension == ".jpg")
                {
                    SaveJpeg(name + "_vl.jpg", very_large, 100);
                    SaveJpeg(name + "_l.jpg", large, 100);
                    SaveJpeg(name + "_m.jpg", middle, 100);
                    SaveJpeg(name + "_s.jpg", small, 100);
                    SaveJpeg(name + "_t.jpg", tiny, 100);
                    SaveJpeg(name + ".jpg", same, 100);

                    SaveJpeg(name + "_500.jpg", image_500, 100);
                    SaveJpeg(name + "_400.jpg", image_400, 100);
                    SaveJpeg(name + "_250.jpg", image_250, 100);
                    SaveJpeg(name + "_100.jpg", image_100, 100);
                    SaveJpeg(name + "_75.jpg", image_75, 100);
                }
                else
                {
                    very_large.Save(name + "_vl.png", ImageFormat.Png);
                    very_large.Dispose();

                    large.Save(name + "_l.png", ImageFormat.Png);
                    large.Dispose();

                    middle.Save(name + "_m.png", ImageFormat.Png);
                    middle.Dispose();

                    small.Save(name + "_s.png", ImageFormat.Png);
                    small.Dispose();

                    tiny.Save(name + "_t.png", ImageFormat.Png);
                    tiny.Dispose();

                    same.Save(name + ".png", ImageFormat.Png);
                    same.Dispose();


                    image_500.Save(name + "_500.png", ImageFormat.Png);
                    image_500.Dispose();

                    image_400.Save(name + "_400.png", ImageFormat.Png);
                    image_400.Dispose();

                    image_250.Save(name + "_250.png", ImageFormat.Png);
                    image_250.Dispose();

                    image_100.Save(name + "_100.png", ImageFormat.Png);
                    image_100.Dispose();

                    image_75.Save(name + "_75.png", ImageFormat.Png);
                    image_75.Dispose();
                }

                // cut a middle section of the main image to produce a suitable sample image:
                /*

                System.Drawing.Image sample = image.GetThumbnailImage(100, 300, dummyCallBack, IntPtr.Zero);

                Graphics graphic = Graphics.FromImage(sample);

                try
                {
                    RectangleF dest = new RectangleF(0, 0, 100, 400);
                    RectangleF source = new RectangleF(150, 0, 100, 400);
                    graphic.DrawImage(large, dest, source, GraphicsUnit.Pixel);

                    if (extension == ".jpg") sample.Save(name + "_sample.jpg", ImageFormat.Jpeg);
                    else sample.Save(name + "_sample.png", ImageFormat.Png);

                    sample.Dispose();
                }
                catch (Exception)
                {
                    //result_l.Text += " "+ex.ToString();
                }
                finally
                {
                    if (null != graphic) graphic.Dispose();
                }
                */

                image.Dispose();
            }
            catch (Exception)
            {
            }
        }

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>

    public class ImageShadow : Image
    {
        private Color _panelColor;

        public Color PanelColor
        {
            get { return _panelColor; }
            set { _panelColor = value; }
        }

        private Color _borderColor;

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        private int shadowSize = 5;
        private int shadowMargin = 2;

        // static for good perfomance 
        static Image shadowDownRight = new Bitmap(typeof(ImageShadow), "Images.tshadowdownright.png");
        static Image shadowDownLeft = new Bitmap(typeof(ImageShadow), "Images.tshadowdownleft.png");
        static Image shadowDown = new Bitmap(typeof(ImageShadow), "Images.tshadowdown.png");
        static Image shadowRight = new Bitmap(typeof(ImageShadow), "Images.tshadowright.png");
        static Image shadowTopRight = new Bitmap(typeof(ImageShadow), "Images.tshadowtopright.png");

        public ImageShadow()
        {
        }

        public static System.Drawing.Image OnPaint(System.Drawing.Image i)
        {
            // Get the graphics object. We need something to draw with ;-)
            Graphics g = Graphics.FromImage(i);

            // Create tiled brushes for the shadow on the right and at the bottom.
            TextureBrush shadowRightBrush = new TextureBrush(shadowRight, WrapMode.Tile);
            TextureBrush shadowDownBrush = new TextureBrush(shadowDown, WrapMode.Tile);

            // Translate (move) the brushes so the top or left of the image matches the top or left of the
            // area where it's drawed. If you don't understand why this is necessary, comment it out. 
            // Hint: The tiling would start at 0,0 of the control, so the shadows will be offset a little.
            shadowDownBrush.TranslateTransform(0, Height - shadowSize);
            shadowRightBrush.TranslateTransform(Width - shadowSize, 0);

            // Define the rectangles that will be filled with the brush.
            // (where the shadow is drawn)
            Rectangle shadowDownRectangle = new Rectangle(
                shadowSize + shadowMargin,                      // X
                Height - shadowSize,                            // Y
                Width - (shadowSize * 2 + shadowMargin),        // width (stretches)
                shadowSize                                      // height
                );                                    
           
            Rectangle shadowRightRectangle = new Rectangle(
                Width - shadowSize,                             // X
                shadowSize + shadowMargin,                      // Y
                shadowSize,                                     // width
                Height - (shadowSize * 2 + shadowMargin)        // height (stretches)
                );

            // And draw the shadow on the right and at the bottom.
            g.FillRectangle(shadowDownBrush, shadowDownRectangle);
            g.FillRectangle(shadowRightBrush, shadowRightRectangle);

            // Now for the corners, draw the 3 5x5 pixel images.
            g.DrawImage(shadowTopRight, new Rectangle(Width - shadowSize, shadowMargin, shadowSize, shadowSize));
            g.DrawImage(shadowDownRight, new Rectangle(Width - shadowSize, Height - shadowSize, shadowSize, shadowSize));
            g.DrawImage(shadowDownLeft, new Rectangle(shadowMargin, Height - shadowSize, shadowSize, shadowSize));

            // Fill the area inside with the color in the PanelColor property.
            // 1 pixel is added to everything to make the rectangle smaller. 
            // This is because the 1 pixel border is actually drawn outside the rectangle.
             Rectangle fullRectangle = new Rectangle(
                1,                                              // X
                1,                                              // Y
                Width - (shadowSize + 2),                       // Width
                Height - (shadowSize + 2)                       // Height
                );                     
            
            if (PanelColor != null)
            {
                SolidBrush bgBrush = new SolidBrush(_panelColor);
                g.FillRectangle(bgBrush, fullRectangle);
            }

            // Draw a nice 1 pixel border it a BorderColor is specified
            if (_borderColor != null)
            {
                Pen borderPen = new Pen(BorderColor);
                g.DrawRectangle(borderPen, fullRectangle);
            }

            // Memory efficiency
            shadowDownBrush.Dispose();
            shadowRightBrush.Dispose();

            shadowDownBrush = null;
            shadowRightBrush = null;
        }
    }

         */

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static bool SaveJpeg(string sFileName, System.Drawing.Image img, long nQuality)
        {
            //Syntax: SaveJpeg(filename, image object, quality (1-100))

            try
            {
                // Build image encoder detail
                ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
                EncoderParameters encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, nQuality);
                // Data used "System.Drawing.Imaging" because there is another Encoder in System.Text

                // Get jpg format id
                foreach (ImageCodecInfo encoder in encoders)
                {
                    if (encoder.MimeType == "image/jpeg")
                    {
                        // Save
                        img.Save(sFileName, encoder, encoderParameters);
                        img.Dispose();
                        return true;
                    }
                }

                //result_l.Text += " Suitable JPEG encoder format not found";
            }
            catch (Exception)
            {
                //result_l.Text += " Error saving:"+sFileName+":"+ex.Message;
            }
            return false;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delete all images generated from CreateGeneralUseImages() function
        /// <param name="relative_path">Folder path of all images to be deleted</param>
        /// <param name="num">Designated main number of all images</param>
        /// <param name="suffix">Suffix image number</param>
        /// </summary>
        public static void DeleteGeneralUseImages(string relative_path, int num, int suffix)
        {
            int count;
            string name, absolutePath, r;
            string[] p;

            absolutePath = AbsolutePath();

            if (num < 0 && suffix < 0) name = absolutePath + relative_path;
            else if (num < 0) name = absolutePath + relative_path + "_" + suffix;
            else if (suffix < 0) name = absolutePath + relative_path + "_" + num;
            else name = absolutePath + relative_path + num + "_" + suffix;

            File.Delete(name + "_vl.jpg");
            File.Delete(name + "_l.jpg");
            File.Delete(name + "_m.jpg");
            File.Delete(name + "_s.jpg");
            File.Delete(name + "_t.jpg");
            File.Delete(name + ".jpg");

            File.Delete(name + "_500.jpg");
            File.Delete(name + "_400.jpg");
            File.Delete(name + "_250.jpg");
            File.Delete(name + "_100.jpg");
            File.Delete(name + "_75.jpg");

            // rearrange images so they stay in sequence
            try
            {
                // rename all following images so that they fill the name of the missing image:
                // count images:
                p = Directory.GetFiles(absolutePath + relative_path, num + @"_*_t.jpg");

                count = p.Length;

                for (int i = suffix; i < count; i++)
                {
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_vl.jpg", absolutePath + relative_path + num + "_" + i + "_vl.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_l.jpg", absolutePath + relative_path + num + "_" + i + "_l.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_m.jpg", absolutePath + relative_path + num + "_" + i + "_m.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_s.jpg", absolutePath + relative_path + num + "_" + i + "_s.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_t.jpg", absolutePath + relative_path + num + "_" + i + "_t.jpg");
                    //File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_sample.jpg", absolutePath + relative_path + num + "_" + i + "_sample.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + ".jpg", absolutePath + relative_path + num + "_" + i + ".jpg");

                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_500.jpg", absolutePath + relative_path + num + "_" + i + "_500.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_400.jpg", absolutePath + relative_path + num + "_" + i + "_400.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_250.jpg", absolutePath + relative_path + num + "_" + i + "_250.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_100.jpg", absolutePath + relative_path + num + "_" + i + "_100.jpg");
                    File.Move(absolutePath + relative_path + num + "_" + (i + 1) + "_75.jpg", absolutePath + relative_path + num + "_" + i + "_75.jpg");
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

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Delete all images generated from CreateGeneralUseImages() function
        /// <param name="relative_path">Folder path of all images to be deleted</param>
        /// <param name="num">Designated main number of all images</param>
        /// </summary>
        public static void DeleteGeneralUseImages(string relative_path, int num)
        {
            string absolutePath;

            absolutePath = AbsolutePath();

            foreach (string file in Directory.GetFiles(absolutePath + relative_path, num + "_*.jpg"))
            {
                System.IO.File.Delete(file);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Move images from a suffix to another
        /// <param name="relative_path">Folder path of all images to be moved</param>
        /// <param name="num">Designated main number of all images</param>
        /// <param name="suffix_old">Suffix image number</param>
        /// <param name="suffix_new">New suffix image number</param>
        /// </summary>
        public static void MoveGeneralUseImages(string relative_path, int num, int suffix_old, int suffix_new)
        {
            string name, name_new, absolutePath;

            absolutePath = AbsolutePath();

            if (num < 0 && suffix_old < 0)
            {
                name = name_new = absolutePath + relative_path;
            }
            else if (num < 0)
            {
                name = absolutePath + relative_path + "_" + suffix_old;
                name_new = absolutePath + relative_path + "_" + suffix_new;
            }
            else if (suffix_old < 0)
            {
                name = name_new = absolutePath + relative_path + "_" + num;
            }
            else
            {
                name = absolutePath + relative_path + num + "_" + suffix_old;
                name_new = absolutePath + relative_path + num + "_" + suffix_new;
            }

            File.Move(name + "_vl.jpg", name_new + "_vl.jpg");
            File.Move(name + "_l.jpg", name_new + "_l.jpg");
            File.Move(name + "_m.jpg", name_new + "_m.jpg");
            File.Move(name + "_s.jpg", name_new + "_s.jpg");
            File.Move(name + "_t.jpg", name_new + "_t.jpg");
            File.Move(name + ".jpg", name_new + ".jpg");

            File.Move(name + "_500.jpg", name_new + "_500.jpg");
            File.Move(name + "_400.jpg", name_new + "_400.jpg");
            File.Move(name + "_250.jpg", name_new + "_250.jpg");
            File.Move(name + "_100.jpg", name_new + "_100.jpg");
            File.Move(name + "_75.jpg", name_new + "_75.jpg");
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static int CountGeneralUseImages(string relative_path, int num)
        {
            int count;
            string absolutePath, r;
            string[] p;

            count = 0;
            absolutePath = AbsolutePath();

            try
            {
                // count images:
                p = Directory.GetFiles(absolutePath + relative_path, num + @"_*_t.jpg");

                count = p.Length;
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

            return count;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static bool ThumbnailCallback()
        {
            return false;
        }

#if WINDOWS_FORM
#else
        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Default(string file)
        {
            string line;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));
                line = "<img src=\"" + file + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" border=0>";
                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ImageUrl(string file, string url)
        {
            string line;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));
                line = "<a href=" + url + "><img src=\"" + file + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" border=0></a>";
                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ImageShadowed(string file)
        {
            // produce an image with nice shadow for a ltr document:
            string line;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));

                line = "<table cellpadding=0 cellspacing=0><tr><td><img src=\"" + file + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" border=0/></td><td width=5 height=5 valign=top background=image/shadow_ver_ltr.jpg><img src=image/shadow_ver_top_ltr.jpg width=5 height=5></td></tr><tr><td background=image/shadow_hor_ltr.jpg align=left><img src=image/shadow_hor_left_ltr.jpg width=5 height=5></td><td width=5 height=5><img src=image/shadow_corner_ltr.jpg width=5 height=5></td></tr></table>";

                /*
                <table cellpadding="0" cellspacing="0">
                 <tr>
                  <td style="width:5px;height:5;vertical-align:top;background-image:url(../image/shadow_ver.jpg)"><img src="../image/shadow_ver_top.jpg" style="width:5px;height:5" alt=""></td>
                  <td><asp:HyperLink id="photo_hl" runat="server"/></td>
                 </tr>
                 <tr>
                  <td style="width:5px;height:5"><img src="../image/shadow_corner.jpg" style="width:5px;height:5" alt=""></td>
                  <td style="text-align:right;background-image:url(../image/shadow_hor.jpg)"><img src="../image/shadow_hor_left.jpg" style="width:5px;height:5" alt=""></td>
                 </tr>
                </table>

                */

                /*
                line = "<div id=\"image_shadowed\" style=\"position:relative\">"+
                  "<img src=\""+file+"\" style=\"position:absolute:left:0px;top:0px;width:"+image.Width+"px;height:"+image.Height+"px\"/>"+
                  "<img src=image/shadow_ver_top_ltr.jpg style=\"position:absolute;left:"+image.Width+"px;top:0px;width:5px;height:5px\"/>"+
                  "<img style=\"position:absolute;left:"+image.Width+"px;top:5px;width:5px;height:"+(image.Height-5)+"px;background-image:url(image/shadow_ver_ltr.jpg)\"/>"+
                  "<img src=image/shadow_hor_left_ltr.jpg style=\"position:absolute;left:0px;top:"+image.Height+"px;width:5px;height:5px\"/>"+
                  "<img style=\"position:absolute;left:5px;top:"+image.Height+";width:"+(image.Width-5)+"px;height:5px;background-image:url(image/shadow_hor_ltr.jpg)\"/>"+
                  "<img src=image/shadow_corner_ltr.jpg style=\"position:absolute;left:"+image.Width+"px;top:"+image.Height+"px;width:5px;height:5px\"/>"+
                  "</div>";
          
                  */

                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Add shadows to image. This does not display well on Chrome
        /// </summary>
        public static string ImageShadowedUrl(System.Web.UI.Page page, string file, string url)
        {
            string line;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));

                line = "<table class=shadow cellpadding=0 cellspacing=0><tr><td><a href=" + url + "><img src=\"" + global::Ia.Cl.Model.Default.AbsolutePathUrl(page, file) + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" border=0/></a></td><td width=5 height=5 valign=top background=" + page.ResolveClientUrl("~/image/shadow_ver_ltr.jpg") + "><img src=" + page.ResolveClientUrl("~/image/shadow_ver_top_ltr.jpg") + " width=5 height=5></td></tr>" +
                    "<tr><td background=" + page.ResolveClientUrl("~/image/shadow_horizontal.jpg") + " align=right><img src=" + page.ResolveClientUrl("~/image/shadow_hor_left_ltr.jpg") + " width=5 height=5></td><td width=5 height=5><img src=" + page.ResolveClientUrl("~/image/shadow_corner_ltr.jpg") + " width=5 height=5></td></tr></table>";

                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Png_Transparent(System.Web.UI.Page p, string file)
        {
            // produce div with a transparent PNG part showing
            string line, image_url;

            image_url = global::Ia.Cl.Model.Default.AbsoluteUrl(p) + file;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));
                line = "<div style=\"width:" + image.Width + ";height:" + image.Height + ";filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + image_url + "');\"><img src=\"" + image_url + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" style=\"filter:Alpha(opacity=0)\"/></div>";
                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Png_TransparentShadowedUrl(System.Web.UI.Page p, string file, string navigate_url)
        {
            // produce div with a transparent PNG part showing and shadowed borders
            string line, image_url;

            image_url = global::Ia.Cl.Model.Default.AbsoluteUrl(p) + file;

            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath + "/" + file));
                line = "<div style=\"width:" + image.Width + ";height:" + image.Height + ";filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src='" + image_url + "');\"><img src=\"" + image_url + "\" width=\"" + image.Width + "\" height=\"" + image.Height + "\" style=\"filter:Alpha(opacity=0)\"/></div>";
                line = "<table cellpadding=0 cellspacing=0><tr><td style=\"cursor:hand;\"><a href=" + navigate_url + ">" + line + " </a></td><td width=5 height=5 valign=top background=image/shadow_ver_ltr.jpg><img src=image/shadow_ver_top_ltr.jpg width=5 height=5></td></tr><tr><td background=image/shadow_hor_ltr.jpg align=left><img src=image/shadow_hor_left_ltr.jpg width=5 height=5></td><td width=5 height=5><img src=image/shadow_corner_ltr.jpg width=5 height=5></td></tr></table>";
                image.Dispose();
            }
            catch
            {
                line = ""; //"&nbsp;";
            }

            return line;
        }
#endif

        /*
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        private static void Image_Rotate_Prepare(XmlNode xn)
        {
            // prepare images read from the folder
            //bool b;
            int i;
            string original, destination;
            string source;

            i = 0;

            foreach (XmlNode n in xn.SelectNodes("* /rotator/group"))
            {
                original = n.ParentNode.SelectSingleNode("original").Attributes["folder"].Value;
                destination = n.ParentNode.SelectSingleNode("destination").Attributes["folder"].Value;

                foreach (XmlNode o in n.SelectNodes("item"))
                {
                    source = o.SelectSingleNode("source").InnerText;

                    CreateGeneralUseImages(original + source, destination + i++);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string Image_Rotate(string s_xml, string group_name, string size, int life_minute, string css_class)
        {
            int id, id_start;
            string source, destination, url, alternate_text, extension, s, t;
            StringBuilder sb;
            Hashtable ht;
            XmlNode xn;

            id = 0;
            sb = new StringBuilder(1000);
            ht = new Hashtable(100);

            id_start = -1;
            destination = null;

            xn = xml.ReturnXmlNode(s_xml);

            try
            {
                foreach (XmlNode n in xn.SelectNodes("* /rotator/group/item"))
                {
                    if (n.ParentNode.Attributes["name"].Value == group_name)
                    {
                        if (id_start == -1) id_start = id;

                        source = n.SelectSingleNode("source").InnerText;
                        destination = n.ParentNode.ParentNode.SelectSingleNode("destination").Attributes["folder"].Value;

                        extension = System.IO.Path.GetExtension(source);
                        extension = extension.ToLower();

                        if (size == "large") s = "l";
                        else if (size == "middle") s = "m";
                        else s = "m";

                        alternate_text = n.SelectSingleNode("alternate_text").InnerText;

                        t = destination + id + "_" + s + extension + "|" + alternate_text;

                        // change system path format to url path format
                        t = "~/" + t.Replace(@"\", @"/");
                        ht[id] = t;
                    }

                    id++;
                }

                id = ((DateTime.UtcNow.Minute + 1) / life_minute) % ht.Count + id_start;

                if (destination != null)
                {
                    // generate images if there are non
                    if (global::Ia.Cl.Model.File.Count(destination, "*") == 0) Image_Rotate_Prepare(xn);
                }

                // id = global::Ia.Cl.Model.Default.Random(ht.Count-1);

                url = ht[id].ToString().Split('|')[0];
                alternate_text = ht[id].ToString().Split('|')[1];

                sb.Append("<span class=\"" + css_class + "\">");
                sb.Append("<table>");
                sb.Append("<tr><td><img src=" + url + " alt=" + alternate_text + "/></td></tr>");
                sb.Append("<tr><td>" + alternate_text + "</td></tr>");
                sb.Append("</table>");
                sb.Append("</span>");
            }
            catch (Exception) { }

            return sb.ToString();
        }
        */


        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Compare two bitmap images and return true if they match
        /// </summary>
        public static bool Compare(Bitmap bmp1, Bitmap bmp2)
        {
            // http://www.codeproject.com/KB/GDI-plus/comparingimages.aspx

            bool b;

            b = true;

            // test to see if we have the same size of image
            if (bmp1.Size != bmp2.Size) b = false;
            else
            {
                // convert each image to a byte array

                System.Drawing.ImageConverter ic = new System.Drawing.ImageConverter();

                byte[] btImage1 = new byte[1];
                btImage1 = (byte[])ic.ConvertTo(bmp1, btImage1.GetType());

                byte[] btImage2 = new byte[1];
                btImage2 = (byte[])ic.ConvertTo(bmp2, btImage2.GetType());

                // compute a hash for each image
                SHA256Managed shaM = new SHA256Managed();
                byte[] hash1 = shaM.ComputeHash(btImage1);
                byte[] hash2 = shaM.ComputeHash(btImage2);

                // compare the hash values
                for (int i = 0; i < hash1.Length && i < hash2.Length && b; i++) if (hash1[i] != hash2[i]) b = false;
            }

            return b;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Check 
        /// </summary>
        private static string AbsolutePath(string relativeOrAbsolutePath)
        {
            string path;

            if (relativeOrAbsolutePath.Contains(":"))
            {
                // this is an absolute path and we will return it
                path = relativeOrAbsolutePath;
            }
            else
            {
                // this is a relative path and we will add to it the absolute path
                path = AbsolutePath() + relativeOrAbsolutePath;
            }

            return path;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Return the absolute path
        /// </summary>
        private static string AbsolutePath()
        {
            string path;

#if WINDOWS_FORM
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) path = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.DataDirectory + @"\";
            else path = AppDomain.CurrentDomain.BaseDirectory;
#else
            path = AppDomain.CurrentDomain.BaseDirectory.ToString();
#endif

            return path;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
