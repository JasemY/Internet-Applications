using System;
using System.Web;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Ia.Cl.Model
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// Handle HTML encoding, decoding functions.
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
    public class Html
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Html() { }

        ////////////////////////////////////////////////////////////////////////////

        ///<summary>
        ///
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remark>http://www.west-wind.com/weblog/posts/2009/Feb/05/Html-and-Uri-String-Encoding-without-SystemWeb</remark>
        public static string HtmlEncode(string s)
        {
#if WINDOWS_FORM

            if (s == null) return null;

            StringBuilder sb = new StringBuilder(s.Length);

            int len = s.Length;

            for (int i = 0; i < len; i++)
            {
                switch (s[i])
                {
                    case '<': sb.Append("&lt;"); break;
                    case '>': sb.Append("&gt;"); break;
                    case '"': sb.Append("&quot;"); break;
                    case '&': sb.Append("&amp;"); break;
                    default:
                        if (s[i] > 159)
                        {
                            // decimal numeric entity
                            sb.Append("&#");
                            sb.Append(((int)s[i]).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else sb.Append(s[i]);
                        break;
                }
            }

            return sb.ToString();
#else
            return HttpUtility.HtmlEncode(s);
#endif
        }

        ////////////////////////////////////////////////////////////////////////////

        ///<summary>
        ///
        /// </summary>
        public static string HtmlDecode(string s)
        {
#if WINDOWS_FORM
            s = s.Replace("&lt;","<");
            s = s.Replace("&gt;",">");
            s = s.Replace("&quot;",@"""");
            s = s.Replace("&amp;","&");

            return s;
#else
            return HttpUtility.HtmlDecode(s);
#endif
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Encode(string s)
        {
            s = HtmlEncode(s);

            // database requirement:
            s = s.Replace(@"'", @"_#039_");
            s = s.Replace(@"?", @"_#063_");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Decode(string s)
        {
            // database requirement:
            s = s.Replace(@"_#063_", @"?");
            s = s.Replace(@"_#039_", @"'");

            s = HtmlDecode(s);

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string DecodeRemoveNLLF(string s)
        {
            // database requirement:

            s = s.Replace(@"_#063_", @"?");
            s = s.Replace(@"_#039_", @"'");

            s = HtmlDecode(s);

            s = s.Replace("\n\r", " ");
            s = s.Replace("\r\n", " ");
            s = s.Replace("\n", " ");
            s = s.Replace("\r", " ");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string XmlEncode(string s)
        {
            s = HtmlEncode(s);

            s = s.Replace(@"'", @"_#039_");
            s = s.Replace(@"\", @"_#092_");
            s = s.Replace(@"?", @"_#063_");

            /*
            &amp;  =  &
            &lt;   =  <
            &gt;   =  >
            &quot; =  "
            &apos; =  '
            */

            // XML requirement:
            s = s.Replace("&", "_amp_");
            s = s.Replace(">", "_gt_");
            s = s.Replace("<", "_lt_");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string XmlDecode(string s)
        {
            // XML requirement
            s = s.Replace("_gt_", ">");
            s = s.Replace("_lt_", "<");
            s = s.Replace("_amp_", "&");

            s = s.Replace(@"_#039_", @"'");
            s = s.Replace(@"_#092_", @"\");
            s = s.Replace(@"_#063_", @"?");

            s = HtmlDecode(s);
            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string Code(string code)
        {
            // this displays an HTML code in regular text
            /*
            s=s.Replace("_gt_",">");
            s=s.Replace("_lt_","<");
            s=s.Replace("_amp_","&");

            s=s.Replace(@"_#039_",@"'");
            s=s.Replace(@"_#092_",@"\");
            s=s.Replace(@"_#063_",@"?");
            */

            code = HtmlEncode(code);
            return code;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string HtmlStrip(string source)
        {
            try
            {
                string result;

                // Remove HTML Development formatting
                // Replace line breaks with space
                // because browsers inserts space
                result = source.Replace("\r", " ");
                // Replace line breaks with space
                // because browsers inserts space
                result = result.Replace("\n", " ");
                // Remove step-formatting
                result = result.Replace("\t", string.Empty);
                // Remove repeating speces becuase browsers ignore them
                result = System.Text.RegularExpressions.Regex.Replace(result,
                                                                      @"( )+", " ");

                // Remove the header (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*head([^>])*>", "<head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*head( )*>)", "</head>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<head>).*(</head>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all scripts (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*script([^>])*>", "<script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*script( )*>)", "</script>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                //result = System.Text.RegularExpressions.Regex.Replace(result, 
                //        @"(<script>)([^(<script>\.</script>)])*(</script>)",
                //        string.Empty, 
                //        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<script>).*(</script>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // remove all styles (prepare first by clearing attributes)
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*style([^>])*>", "<style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"(<( )*(/)( )*style( )*>)", "</style>",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(<style>).*(</style>)", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert tabs in spaces of <td> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*td([^>])*>", "\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line breaks in places of <BR> and <LI> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*br( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*li( )*>", "\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // insert line paragraphs (double line breaks) in place
                // if <P>, <DIV> and <TR> tags
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*div([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*tr([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<( )*p([^>])*>", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Remove remaining tags like <a>, links, images,
                // comments etc - anything thats enclosed inside < >
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"<[^>]*>", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // replace special characters:
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @" ", " ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&bull;", " * ",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lsaquo;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&rsaquo;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&trade;", "(tm)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&frasl;", "/",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&lt;", "<",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&gt;", ">",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&copy;", "(c)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&reg;", "(r)",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove all others. More can be added, see
                // http://hotwired.lycos.com/webmonkey/reference/special_characters/
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         @"&(.{2,6});", string.Empty,
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // for testng
                //System.Text.RegularExpressions.Regex.Replace(result, 
                //      this.txtRegex.Text,string.Empty, 
                //      System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // make line breaking consistent
                result = result.Replace("\n", "\r");

                // Remove extra line breaks and tabs:
                // replace over 2 breaks with 2 and over 4 tabs with 4. 
                // Prepare first to remove any whitespaces inbetween
                // the escaped characters and remove redundant tabs inbetween linebreaks
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\t)", "\t\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\t)( )+(\r)", "\t\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)( )+(\t)", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove redundant tabs
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+(\r)", "\r\r",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Remove multible tabs followind a linebreak with just one tab
                result = System.Text.RegularExpressions.Regex.Replace(result,
                         "(\r)(\t)+", "\r\t",
                         System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                // Initial replacement target string for linebreaks
                string breaks = "\r\r\r";
                // Initial replacement target string for tabs
                string tabs = "\t\t\t\t\t";
                for (int index = 0; index < result.Length; index++)
                {
                    result = result.Replace(breaks, "\r\r");
                    result = result.Replace(tabs, "\t\t\t\t");
                    breaks = breaks + "\r";
                    tabs = tabs + "\t";
                }

                // Thats it.
                return result;

            }
            catch
            {
                //MessageBox.Show("Error");
                return null;
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string TextToHtml(string source)
        {
            // clean regular text format pages and return an equivalent html format

            string s;

            s = Decode(source);
            //s = global::Ia.Cl.Model.Html.Html_Strip(s);
            s = Regex.Replace(s, @"\.", @". ");
            s = Regex.Replace(s, @"[ ]+", @" ");
            s = s.Replace("\r", "");
            s = s.Replace("\n+", "\n");
            //s = "<p>" + s.Replace("\n", "</p>\n<p>") + "</p>";
            /*
            s = s.Replace("\n", "</p>\n<p>");

            // clean up
            u = sb.ToString();
            u = Regex.Replace(u, @"^\s+", "");
            u = Regex.Replace(u, @">\s+", ">");
            u = Regex.Replace(u, @"\s+<", "<");
            u = Regex.Replace(u, @"\s+", " ");
            u = Regex.Replace(u, @"\n+", @"<br/>"); // keep newlines
            //u = Regex.Replace(u, @"</ul>(.+?)</ul>", "</ul><p>$1</p></ul>");
            //u = Regex.Replace(u, @"</ul>(.+?)</p>", "</ul><p>$1</p></p>");
            //u = u.Replace(@"•", "<p/>&nbsp;&nbsp;&nbsp;•&nbsp;");
            */

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string TextToHtml2(string source)
        {
            // clean regular text format pages and return an equivalent html format

            string s;

            s = Decode(source);
            //s = global::Ia.Cl.Model.Html.Html_Strip(s);
            s = Regex.Replace(s, @"\.", @". ");
            s = Regex.Replace(s, @"[ ]+", @" ");
            s = s.Replace("\r", "");
            s = s.Replace("\n+", "\n");
            s = "<p>" + s.Replace("\n", "</p>\n<p>") + "</p>";

            /*
            s = s.Replace("\n", "</p>\n<p>");

            // clean up
            u = sb.ToString();
            u = Regex.Replace(u, @"^\s+", "");
            u = Regex.Replace(u, @">\s+", ">");
            u = Regex.Replace(u, @"\s+<", "<");
            u = Regex.Replace(u, @"\s+", " ");
            u = Regex.Replace(u, @"\n+", @"<br/>"); // keep newlines
            //u = Regex.Replace(u, @"</ul>(.+?)</ul>", "</ul><p>$1</p></ul>");
            //u = Regex.Replace(u, @"</ul>(.+?)</p>", "</ul><p>$1</p></p>");
            //u = u.Replace(@"•", "<p/>&nbsp;&nbsp;&nbsp;•&nbsp;");
            */

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string TextToHtmlAndOl_Ul_LiToBr(string source)
        {
            // clean regular text format pages and return an equivalent html format

            string s;

            s = Decode(source);
            s = Regex.Replace(s, @"\.", @". ");
            s = Regex.Replace(s, @"[ ]+", @" ");
            s = s.Replace("\r", "");
            s = s.Replace("\n+", "\n");

            s = s.Replace("<ol>", "<br/> <br/>");
            s = s.Replace("</ol>", "");
            s = s.Replace("<ul>", "<br/> <br/>");
            s = s.Replace("</ul>", "");
            s = s.Replace("<li>", "-");
            s = s.Replace("</li>", "<br/>");

            return s;
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
