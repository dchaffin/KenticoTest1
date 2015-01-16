using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;


[Title(ResourceString = "reportsubscription.new", ImageUrl = "CMSModules/CMS_Reporting/Subscription24.png", HelpTopic = "report_subscribe")]
public partial class CMSModules_Reporting_Dialogs_EditSubscription : CMSModalPage
{
    #region "Variables"

    bool edit = false;

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        edit = (QueryHelper.GetString("mode", String.Empty).ToLowerCSafe() == "edit");
        if (edit)
        {
            subEdit.SimpleMode = false;
            CurrentMaster.Title.TitleText = GetString("reportsubscription.edit");
            CurrentMaster.Title.HelpTopicName = "report_subscription_edit";
        }

        base.CreateChildControls();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.close");
    }


    protected void btnOK_Click(object sender, EventArgs ea)
    {
        if (subEdit.Save())
        {
            String saveMsg = String.Format(GetString("subscription.confirmation"), subEdit.Report.ReportDisplayName);
            String alertScript = edit ? "" : "wopener.window.alert('" + GetString("reportsubscription.subscribed") + "');";
            String refreshScript = edit ? "if (wopener.refreshCurrentPage != null) wopener.refreshCurrentPage();" : "";
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "CloseScript", ScriptHelper.GetScript(refreshScript + "CloseDialog();" + alertScript));
        }
    }

    #endregion
}
