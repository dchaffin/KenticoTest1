using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Widgets_Dialogs_WidgetProperties_Buttons : CMSDeskPage
{
    /// <summary>
    /// OnInit override - do not require site.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        RequireSite = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        SetBrowserClass();
        bool isInline = QueryHelper.GetBoolean("inline", false);
        // set button text
        btnOk.Text = GetString("general.ok");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");

        chkRefresh.Text = GetString("Widget.Properties.Refresh");

        if (isInline)
        {
            btnApply.Visible = false;
            chkRefresh.Visible = false;
        }

        ltlScript.Text += ScriptHelper.GetScript("function GetRefreshStatus() { var refresh= document.getElementById('" + chkRefresh.ClientID + @"');
        if (refresh != null) {
            return refresh.checked;
        }
        return false;         
        }");

        btnCancel.OnClientClick = "Close(); return false;";
        btnApply.OnClientClick = "Apply(); return false;";
        btnOk.OnClientClick = "Save(); return false;";
    }
}