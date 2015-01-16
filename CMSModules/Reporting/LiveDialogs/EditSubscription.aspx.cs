using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;

[Title(ResourceString = "reportsubscription.new", ImageUrl = "CMSModules/CMS_Reporting/Subscription24.png")]
public partial class CMSModules_Reporting_LiveDialogs_EditSubscription : CMSLiveModalPage
{
    #region "Variables"

    bool liveEdit = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (liveEdit)
        {
            CurrentMaster.Title.TitleText = GetString("reportsubscription.edit");
        }

        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.close");
    }


    protected override void CreateChildControls()
    {
        String mode = QueryHelper.GetString("mode", String.Empty);
        liveEdit = (mode.ToLowerCSafe() == "liveedit");
        subEdit.SimpleMode = !liveEdit;

        base.CreateChildControls();
    }


    protected void btnOK_Click(object sender, EventArgs ea)
    {
        if (subEdit.Save())
        {
            String saveMsg = String.Format(GetString("subscription.confirmation"), subEdit.Report.ReportDisplayName);
            String alert = liveEdit ? "" : "wopener.window.alert('" + GetString("reportsubscription.subscribed") + "')";
            ScriptHelper.RegisterStartupScript(Page, typeof(String), "CloseScript", ScriptHelper.GetScript("CloseDialog();" + alert));
        }
    }

    #endregion
}
