using System;
using System.Web;
using System.Xml;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace Ia.Model.Design
{
    ////////////////////////////////////////////////////////////////////////////

    /// <summary publish="true">
    /// ASP.NET design related support class.
    /// </summary>
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

    public class Gv
    {
        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public Gv() { }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        ///
        /// </summary>
        public static void Verify()
        {
        }

        ////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewTextTemplate : ITemplate
{
    string _id;

    public DynamicGridViewTextTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Label l = new Label();
        l.ID = _id + "_l";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewTextEditTemplate : ITemplate
{
    string _id;
    DataRow _r;

    public DynamicGridViewTextEditTemplate(DataRow r)
    {
        _id = r["id"].ToString();
        _r = r;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        TextBox tb = new TextBox();
        tb.ID = _id + "_update_tb";
        tb.DataBinding += new EventHandler(this.tb_DataBind);

        if (_r["text_mode"] != null && _r["text_mode"].ToString().Length > 0)
        {
            if (_r["text_mode"].ToString().ToLower() == "multiline") tb.TextMode = TextBoxMode.MultiLine;
            else if (_r["text_mode"].ToString().ToLower() == "password") tb.TextMode = TextBoxMode.Password;
            else if (_r["text_mode"].ToString().ToLower() == "singleline") tb.TextMode = TextBoxMode.SingleLine;
        }
        if (_r["row"] != null && _r["row"].ToString().Length > 0) tb.Rows = int.Parse(_r["row"].ToString()) / 2;
        if (_r["column"] != null && _r["column"].ToString().Length > 0) tb.Columns = int.Parse(_r["column"].ToString()) / 2;
        if (_r["max_length"] != null && _r["max_length"].ToString().Length > 0) tb.MaxLength = int.Parse(_r["max_length"].ToString());

        container.Controls.Add(tb);
    }

    private void tb_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewDdlTemplate : ITemplate
{
    string _id;

    public DynamicGridViewDdlTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Label l = new Label();
        l.ID = _id + "_l";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewDdlEditTemplate : ITemplate
{
    string _id;
    string _data_xpath, _data_text_field, _data_value_field;

    public DynamicGridViewDdlEditTemplate(string id, string data_xpath, string data_text_field, string data_value_field)
    {
        _id = id;
        _data_xpath = data_xpath;
        _data_text_field = data_text_field;
        _data_value_field = data_value_field;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        DropDownList ddl = new DropDownList();
        ddl.ID = _id + "_update_ddl";
        ddl.DataSourceID = _id + "_xds";
        ddl.DataTextField = _data_text_field;
        ddl.DataValueField = _data_value_field;
        ddl.DataBinding += new EventHandler(this.ddl_DataBind);
        container.Controls.Add(ddl);
    }

    private void ddl_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewDbDdlEditTemplate : ITemplate
{
    string _id;
    string _data_text_field, _data_value_field;
    Hashtable _ht;

    public DynamicGridViewDbDdlEditTemplate(string id, Hashtable ht, string data_text_field, string data_value_field)
    {
        _id = id;
        _data_text_field = data_text_field;
        _data_value_field = data_value_field;
        _ht = ht;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        DropDownList ddl = new DropDownList();
        ddl.ID = _id + "_update_ddl";
        ddl.DataTextField = _data_text_field;
        ddl.DataValueField = _data_value_field;
        ddl.DataBinding += new EventHandler(this.ddl_DataBind);

        foreach (string v in _ht.Keys)
        {
            if (v.Contains(_id))
            {
                ddl.Items.Add(new ListItem(_ht[v].ToString(), v.Replace(_id + "|", "")));
            }
        }

        container.Controls.Add(ddl);
    }

    private void ddl_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewRblYesNoTemplate : ITemplate
{
    string _id;

    public DynamicGridViewRblYesNoTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Label l = new Label();
        l.ID = _id + "_l";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewRblYesNoEditTemplate : ITemplate
{
    string _id;

    public DynamicGridViewRblYesNoEditTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        RadioButtonList rbl = new RadioButtonList();
        rbl.ID = _id + "_update_rbl";
        rbl.Items.Add(new ListItem(Ia.Model.Default.YesNo(true), "1")); // yes
        rbl.Items.Add(new ListItem(Ia.Model.Default.YesNo(false), "0")); // no
        rbl.DataBinding += new EventHandler(this.rbl_DataBind);
        rbl.RepeatDirection = RepeatDirection.Horizontal;
        rbl.CssClass = "yesno";

        container.Controls.Add(rbl);
    }

    private void rbl_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewUrlTemplate : ITemplate
{
    string _id, _text, _url;

    public DynamicGridViewUrlTemplate(string id, string text, string url)
    {
        _id = id;
        _text = text;
        _url = url;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        HyperLink hl = new HyperLink();
        hl.ID = _id + "_hl";
        hl.Text = _text;
        hl.NavigateUrl = _url;
        hl.DataBinding += new EventHandler(this.hl_DataBind);
        container.Controls.Add(hl);
    }

    private void hl_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewUrlEditTemplate : ITemplate
{
    string _id, _text, _url;

    public DynamicGridViewUrlEditTemplate(string id, string text, string url)
    {
        _id = id;
        _text = text;
        _url = url;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        HyperLink hl = new HyperLink();
        hl.ID = _id + "_update_hl";
        hl.Text = _text;
        hl.NavigateUrl = _url;
        hl.DataBinding += new EventHandler(this.hl_DataBind);
        container.Controls.Add(hl);
    }

    private void hl_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewPhotoTemplate : ITemplate
{
    string _id;

    public DynamicGridViewPhotoTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Image i = new Image();
        i.ID = _id + "_i";
        i.DataBinding += new EventHandler(this.i_DataBind);
        container.Controls.Add(i);
    }

    private void i_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicGridViewPhotoEditTemplate : ITemplate
{
    string _id;

    public DynamicGridViewPhotoEditTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        HyperLink hl = new HyperLink();
        hl.ID = _id + "_update_hl";
        hl.DataBinding += new EventHandler(this.hl_DataBind);
        container.Controls.Add(hl);

        Label l = new Label();
        l.ID = _id + "_update_l";
        l.Text = "Click photo to update";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void hl_DataBind(Object sender, EventArgs e)
    {
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
    }
}

////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////










/*
////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewTextTemplate : ITemplate
{
    string _id;

    public DynamicDetailsViewTextTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Label l = new Label();
        l.ID = _id + "_l";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
        //Label l = (Label)sender;
        //DetailsViewRow row = (DetailsViewRow)l.NamingContainer;
        //l.Text = DataBinder.Eval(row.DataItem, _id).ToString();
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewTextEditTemplate : ITemplate
{
    string _id;

    public DynamicDetailsViewTextEditTemplate(string id)
    {
        _id = id;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        TextBox tb = new TextBox();
        tb.ID = _id + "_update_tb";
        tb.DataBinding += new EventHandler(this.tb_DataBind);
        container.Controls.Add(tb);
    }

    private void tb_DataBind(Object sender, EventArgs e)
    {
        //TextBox tb = (TextBox)sender;
        //DetailsViewRow row = (DetailsViewRow)tb.NamingContainer;
        //tb.Text = DataBinder.Eval(row.DataItem, _id).ToString();
    }
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewDdlTemplate : ITemplate
{
    string _id;
    //string _data_xpath, _data_text_field, _data_value_field;

    public DynamicDetailsViewDdlTemplate(string id/ *, string data_xpath, string data_text_field, string data_value_field* /)
    {
        _id = id;
        //_data_xpath = data_xpath;
        //_data_text_field = data_text_field;
        //_data_value_field = data_value_field;
    }

    public void InstantiateIn(System.Web.UI.Control container)
    {
        Label l = new Label();
        l.ID = _id + "_l";
        l.DataBinding += new EventHandler(this.l_DataBind);
        container.Controls.Add(l);
    }

    private void l_DataBind(Object sender, EventArgs e)
    {
        /*
        string s;

        Label l = (Label)sender;
        DetailsViewRow row = (DetailsViewRow)l.NamingContainer;

        s = DataBinder.Eval(row.DataItem, _id).ToString();

        if (s.Length > 0) ddl.SelectedIndex = int.Parse(s);
        */

/*
Label l = (Label)sender;
DetailsViewRow row = (DetailsViewRow)l.NamingContainer;
l.Text = DataBinder.Eval(row.DataItem, _id).ToString();
* /
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewDdlEditTemplate : ITemplate
{
string _id;
string _data_xpath, _data_text_field, _data_value_field;

public DynamicDetailsViewDdlEditTemplate(string id, string data_xpath, string data_text_field, string data_value_field)
{
_id = id;
_data_xpath = data_xpath;
_data_text_field = data_text_field;
_data_value_field = data_value_field;
}

public void InstantiateIn(System.Web.UI.Control container)
{
DropDownList ddl = new DropDownList();
ddl.ID = _id + "_update_ddl";
ddl.DataSourceID = _id + "_xds";
ddl.DataTextField = _data_text_field;
ddl.DataValueField = _data_value_field;
ddl.DataBinding += new EventHandler(this.ddl_DataBind);
container.Controls.Add(ddl);
}

private void ddl_DataBind(Object sender, EventArgs e)
{
/*
string s;

DropDownList ddl = (DropDownList)sender;
DetailsViewRow row = (DetailsViewRow)ddl.NamingContainer;

s = DataBinder.Eval(row.DataItem, _id).ToString();

if (s.Length > 0)
{
    ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(s));
}
* /
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewDbDdlEditTemplate : ITemplate
{
string _id;
string _data_text_field, _data_value_field;
Hashtable _ht;

public DynamicDetailsViewDbDdlEditTemplate(string id, Hashtable ht, string data_text_field, string data_value_field)
{
_id = id;
_data_text_field = data_text_field;
_data_value_field = data_value_field;
_ht = ht;
}

public void InstantiateIn(System.Web.UI.Control container)
{
DropDownList ddl = new DropDownList();
ddl.ID = _id + "_update_ddl";
ddl.DataTextField = _data_text_field;
ddl.DataValueField = _data_value_field;
ddl.DataBinding += new EventHandler(this.ddl_DataBind);

foreach (string v in _ht.Keys)
{
    if (v.Contains(_id))
    {
        ddl.Items.Add(new ListItem(_ht[v].ToString(), v.Replace(_id + "|", "")));
    }
}

container.Controls.Add(ddl);
}

private void ddl_DataBind(Object sender, EventArgs e)
{
/*
string s;

DropDownList ddl = (DropDownList)sender;
DetailsViewRow row = (DetailsViewRow)ddl.NamingContainer;

s = DataBinder.Eval(row.DataItem, _id).ToString();

if (s.Length > 0)
{
    ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(s));
}
* /
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewRblYesNoTemplate : ITemplate
{
string _id;

public DynamicDetailsViewRblYesNoTemplate(string id)
{
_id = id;
}

public void InstantiateIn(System.Web.UI.Control container)
{
Label l = new Label();
l.ID = _id + "_l";
l.DataBinding += new EventHandler(this.l_DataBind);
container.Controls.Add(l);
}

private void l_DataBind(Object sender, EventArgs e)
{
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewRblYesNoEditTemplate : ITemplate
{
string _id;

public DynamicDetailsViewRblYesNoEditTemplate(string id)
{
_id = id;
}

public void InstantiateIn(System.Web.UI.Control container)
{
RadioButtonList rbl = new RadioButtonList();
rbl.ID = _id + "_update_rbl";
rbl.DataBinding += new EventHandler(this.rbl_DataBind);
container.Controls.Add(rbl);
}

private void rbl_DataBind(Object sender, EventArgs e)
{
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewUrlTemplate : ITemplate
{
string _id, _text, _url;

public DynamicDetailsViewUrlTemplate(string id, string text, string url)
{
_id = id;
_text = text;
_url = url;
}

public void InstantiateIn(System.Web.UI.Control container)
{
HyperLink hl = new HyperLink();
hl.ID = _id + "_hl";
hl.Text = _text;
hl.NavigateUrl = _url;
//hl.Target = "_blank";
hl.DataBinding += new EventHandler(this.hl_DataBind);
container.Controls.Add(hl);
}

private void hl_DataBind(Object sender, EventArgs e)
{
/*
HyperLink hpl = (HyperLink)sender;
DetailsViewRow row = (DetailsViewRow)hpl.NamingContainer;
hpl.NavigateUrl = DataBinder.Eval(row.DataItem, _idURL).ToString();
hpl.Text = "<div class=\"Post\"><div class=\"PostTitle\">" + DataBinder.Eval(row.DataItem, _idText).ToString() + "</div></div>";
* /
}
}

////////////////////////////////////////////////////////////////////////////

/// <summary>
///
/// </summary>
public class DynamicDetailsViewUrlEditTemplate : ITemplate
{
string _id, _text, _url;

public DynamicDetailsViewUrlEditTemplate(string id, string text, string url)
{
_id = id;
_text = text;
_url = url;
}

public void InstantiateIn(System.Web.UI.Control container)
{
HyperLink hl = new HyperLink();
hl.ID = _id + "_update_hl";
hl.Text = _text;
hl.NavigateUrl = _url;
//hl.Target = "_blank";
hl.DataBinding += new EventHandler(this.hl_DataBind);
container.Controls.Add(hl);

/*
Literal li = new Literal();
li.Text = @"<br/>";
container.Controls.Add(li);
*/

/*
// below: link to the photo editing page
hl = new HyperLink();
hl.ID = _id + "_update_hl";
hl.Text = "Update";
hl.NavigateUrl = "photo.aspx?id=" + _id;
//onclick="photo('<%# DataBinder.Eval(Container.DataItem,"id").ToString() %>');return false;" href=""
hl.DataBinding += new EventHandler(this.hl_DataBind);
container.Controls.Add(hl);
*/

/*
Label l = new Label();
l.ID = _id + "_l";
l.Text = @"<a href="""" onclick=""photo('1');return false;"">Update</a>";
l.DataBinding += new EventHandler(this.hl_DataBind);
container.Controls.Add(l);
* /
}

private void hl_DataBind(Object sender, EventArgs e)
{
/*
HyperLink hpl = (HyperLink)sender;
DetailsViewRow row = (DetailsViewRow)hpl.NamingContainer;
hpl.NavigateUrl = DataBinder.Eval(row.DataItem, _idURL).ToString();
hpl.Text = "<div class=\"Post\"><div class=\"PostTitle\">" + DataBinder.Eval(row.DataItem, _idText).ToString() + "</div></div>";
* /
}
}

*/

////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
