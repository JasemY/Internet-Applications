using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ia.Cl.Model
{
    /// <summary publish="true">
    /// Newspaper and publication display format support class.
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
    public class Newspaper
    {
        public Newspaper() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static DataTable CountryMenu(DataTable countryDataTable, DataTable advertisementDataTable)
        {
            // This will format the country menu and add a count columns to indicate the number
            // of advs under the country

            // This returns a DataTable that has the result of the filter operation on the table

            //string sort = "id ASC",filter="";
            DataTable dt = new DataTable();
            DataRow dr;

            dt = countryDataTable.Clone();
            dt.Columns.Add(new DataColumn("count", System.Type.GetType("System.Int32")));

            foreach (DataRow row in countryDataTable.Rows) //,sort))
            {
                dr = dt.NewRow();

                if (row["id"].ToString() == "0") advertisementDataTable.DefaultView.RowFilter = "show = true AND country_id <> -1";
                else advertisementDataTable.DefaultView.RowFilter = "show = true AND country_id = " + row["id"];

                dr["count"] = advertisementDataTable.DefaultView.Count;
                dr["id"] = row["id"];
                dr["name"] = row["name"].ToString() + " (" + dr["count"].ToString() + ")";
                dr["name_e"] = row["name_e"].ToString() + " (" + dr["count"].ToString() + ")";
                dt.Rows.Add(dr);
            }

            return dt;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static string ClassifiedGrid(DataView dataView)
        {
            // the screenResolution will define the resolution of the user's computer screen. The output will be formated to best fit 
            // this screen.

            // This function will generate a string format of a table that contains ads generated in a tabular form
            bool added, show;
            string str, id, width, height, html, style;
            int duration;
            DateTime modified;
            int xpos, ypos, count, last_line_count, columns;

            columns = 7;

            Grid grid = new Grid(columns, 30);
            Point point;

            // loop through classified ads

            foreach (DataRowView adv in dataView)
            {
                id = adv["id"].ToString();
                width = adv["width"].ToString();
                height = adv["height"].ToString();
                html = global::Ia.Cl.Model.Html.HtmlDecode(adv["html"].ToString());
                style = adv["style"].ToString();
                show = (bool)adv["show"];
                modified = (DateTime)(adv["modified"]);
                duration = int.Parse(adv["duration"].ToString());

                added = false;

                xpos = 0; ypos = 0;

                if (Math.Abs(modified.DayOfYear - DateTime.Now.DayOfYear) <= duration)
                {
                    // this checks if the adv was modified within x days from this day.

                    if (show)
                    {
                        while (!added)
                        {
                            if (grid.IsAvailable(xpos, ypos, int.Parse(width), int.Parse(height)))
                            {
                                grid.Add(int.Parse(id), html, xpos, ypos, int.Parse(width), int.Parse(height), style);
                                added = true;
                            }

                            // increment position
                            if (xpos == grid.Width) { xpos = 0; ypos++; }
                            else xpos++;
                        }
                    }
                }

            }

            // output the values stored in the grid:

            count = last_line_count = 0;
            str = "<table class=grid cellspacing=3>";

            for (int y = 0; y <= grid.Height; y++)
            {
                str += "<tr>";

                for (int x = 0; x <= grid.Width; x++)
                {
                    point = grid.Read(x, y);

                    if (point != null)
                    {
                        //str += point.id+":"+point.x+":"+point.y+":"+point.html;
                        str += "<td width=" + (100 * point.width) + " colspan=" + point.width + " height=" + (100 * point.height) + " rowspan=" + point.height + " class=\"" + point.style + "\">";
                        str += point.html;
                        str += "</td>";
                        count++;
                    }
                    /*
                              else if(count >= dv.Count && grid.IsAvailable(x,y,1,1))
                              {
                                str += "<td width=100 colspan=1 height=100 rowspan=1>&nbsp;</td>";
                                last_line_count++;
                              }
                    */
                    else if (grid.IsAvailable(x, y, 1, 1))
                    {
                        str += "<td width=100 colspan=1 height=100 rowspan=1>&nbsp;</td>";
                        last_line_count++;
                    }
                }
                str += "</tr>";

                if (last_line_count == (grid.Width + 1)) break; // exit the height loop
                else last_line_count = 0;
            }
            str += "</table>";

            return str;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class Grid
        {
            /// <summary/>
            public bool occupied;

            /// <summary/>
            public int x, y, grid_width, grid_height;

            private ArrayList point = new ArrayList();

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Grid(int grid_width, int grid_height)
            {
                this.grid_width = grid_width;
                this.grid_height = grid_height;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public void Add(int id, string html, int x, int y, int width, int height, string style)
            {
                point.Add(new Point(id, html, x, y, width, height, style));
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Point Read(int x, int y)
            {
                foreach (Point p in point)
                {
                    if (p.x == x && p.y == y) return p;
                }

                return null;
            }

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public bool IsAvailable(int x, int y, int w, int h)
            {
                // this checks if new adv could be added without conflect

                // checks that the position is within the grid
                if (x > grid_width || y > grid_height) return false;

                // checks that the adv is within the grid
                if ((x + w - 1) > grid_width || (y + h - 1) > grid_height) return false;

                // loop through all points in grid
                foreach (Point p in point)
                {
                    // if this position is occupied return false
                    if (p.x == x && p.y == y) return false;
                    // if this position falls within the width *and* height of a point return false:
                    else if ((x >= p.x) && (x < (p.x + p.width)) && (y >= p.y) && (y < (p.y + p.height))) return false;

                    else
                    {
                        // this checks that the adv area does note fall within the area of another ad

                        int x_max, x_min, y_max, y_min;
                        int px_max, px_min, py_max, py_min;

                        x_max = x + w;
                        x_min = x;
                        y_max = y + h;
                        y_min = y;

                        px_max = p.x + p.width;
                        px_min = p.x;
                        py_max = p.y + p.height;
                        py_min = p.y;

                        if (!(((x_min >= px_max) || (x_max <= px_min)) || ((y_min >= py_max) || (y_max <= py_min)))) return false;
                    }
                }

                return true;
            }

            /// <summary/>
            public int Width { get { return grid_width; } }

            /// <summary/>
            public int Height { get { return grid_height; } }
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public class Point
        {
            /// <summary/>
            public int id;

            /// <summary/>
            public bool occupied;

            /// <summary/>
            public int x, y, width, height;

            /// <summary/>
            public string html, style;

            ////////////////////////////////////////////////////////////////////////////

            /// <summary>
            ///
            /// </summary>
            public Point(int id, string html, int x, int y, int width, int height, string style)
            {
                this.id = id;
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.html = html;
                this.style = style;
            }

            /// <summary/>
            public int X { get { return x; } }

            /// <summary/>
            public int Y { get { return y; } }

            /// <summary/>
            public int Width { get { return width; } }

            /// <summary/>
            public int Height { get { return height; } }

            /// <summary/>
            public int Id { get { return id; } }

            /// <summary/>
            public string Html { get { return html; } }

            /// <summary/>
            public string Style { get { return style; } }
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}
