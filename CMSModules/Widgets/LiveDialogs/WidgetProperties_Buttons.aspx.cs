using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Widgets_LiveDialogs_WidgetProperties_Buttons : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // set button text
        btnOk.Text = GetString("general.ok");
        btnApply.Text = GetString("general.apply");
        btnCancel.Text = GetString("general.cancel");
        bool isInline = QueryHelper.GetBoolean("inline", false);

        btnCancel.OnClientClick = "Close(); return false;";
        btnApply.OnClientClick = "Apply(); return false;";
        btnOk.OnClientClick = "Save(); return false;";

        if (isInline)
        {
            btnApply.Visible = false;
        }

        RegisterDialogCSSLink();
        SetLiveDialogClass();
    }
}