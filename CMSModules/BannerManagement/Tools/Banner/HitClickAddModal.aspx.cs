using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;

[Title(Text = "{%GetResourceString(QueryString.addnumberres)%}", ImageUrl = "CMSModules/CMS_BannerManagement/add24.png")]

public partial class CMSModules_BannerManagement_Tools_Banner_HitClickAddModal : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string script = @"

function isInteger(value) {
    if ((undefined === value) || (null === value) || (value == '') || (parseInt(value) != value)) {
        return false;
    }
    return value % 1 == 0;
}

function add(txtControlID, validationControlID, numberControlID)
{
    var valueToAdd = $j('#' + txtControlID).val();

    if (!isInteger(valueToAdd))
    {
        $j('#' + validationControlID).show();
        return false;
    }
    wopener.addNumberHitsClicks(numberControlID, valueToAdd);
    return CloseDialog();
}
";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "add", script, true);
        ScriptHelper.RegisterJQuery(Page);

        lblNumberToAdd.ResourceString = HTMLHelper.HTMLEncode(QueryHelper.GetString("numbertoaddres", ""));

        btnAdd.Text = GetString("general.add");
        btnClose.Text = GetString("general.close");

        txtNumberToAdd.Attributes["onkeyup"] = string.Format("$j('#{0}').hide();", lblFormatError.ClientID);

        string numberControlID = QueryHelper.GetString("numbercontrolid", "");

        // function addNumberHitsClicks is defined in HitsClicksControl.ascx
        btnAdd.OnClientClick = string.Format("return add('{0}', '{1}', '{2}')", txtNumberToAdd.ClientID, lblFormatError.ClientID, numberControlID);
    }
}
