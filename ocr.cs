#if WINDOWS_FORM
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Threading;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Handles OCR operations.
    /// </summary>
    /// <value>
    /// See http://code.google.com/p/tesseract-ocr/downloads/list. Include tessnet2.dll and tessdata folder
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
    public class Ocr
    {
        StringBuilder sb;
        List<tessnet2.Word> result;
        tessnet2.Tesseract ocr;
        ManualResetEvent m_event;

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Ocr()
        {
            ocr = new tessnet2.Tesseract();
            ocr.Init("eng", false);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            ocr.Dispose();
            sb.Length = 0;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void DumpResult(List<tessnet2.Word> result)
        {
            foreach (tessnet2.Word word in result) Console.WriteLine("{0} : {1}", word.Confidence, word.Text);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public string DoOCRNormal(Bitmap image)
        {
            result = ocr.DoOCR(image, Rectangle.Empty);

            sb = new StringBuilder(result.Count);

            foreach (tessnet2.Word word in result)
            {
                sb.Append(word.Text + " ");
            }

            return sb.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public List<tessnet2.Word> DoOCRNormal_Word(Bitmap image)
        {
            result = ocr.DoOCR(image, Rectangle.Empty);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void DoOCRMultiThred(Bitmap image, string lang)
        {
            tessnet2.Tesseract ocr = new tessnet2.Tesseract();
            ocr.Init(lang, false);
            // If the OcrDone delegate is not null then this'll be the multithreaded version
            ocr.OcrDone = new tessnet2.Tesseract.OcrDoneHandler(Finished);
            // For event to work, must use the multithreaded version
            ocr.ProgressEvent += new tessnet2.Tesseract.ProgressHandler(ocr_ProgressEvent);
            m_event = new ManualResetEvent(false);
            ocr.DoOCR(image, Rectangle.Empty);
            // Wait here it's finished
            m_event.WaitOne();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public void Finished(List<tessnet2.Word> result)
        {
            //DumpResult(result);
            m_event.Set();
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        void ocr_ProgressEvent(int percent)
        {
            //Console.WriteLine("{0}% progression", percent);
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
#endif
