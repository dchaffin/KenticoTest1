using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;

public partial class CMSModules_AbuseReport_AbuseReport_List : CMSAbuseReportPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Set the page title
        CurrentMaster.Title.TitleText = GetString("abuse.reportabuse");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_AbuseReport/object.png");
        CurrentMaster.Title.HelpTopicName = "abusereport_list";
        
        ucAbuseReportList.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(ucAbuseReportList_OnCheckPermissions);
    }


    /// <summary>
    /// Check permission.
    /// </summary>
    /// <param name="permissionType">Permission type</param>
    /// <param name="sender">Sender</param>
    private void ucAbuseReportList_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.AbuseReport", permissionType))
        {
            sender.StopProcessing = true;
            RedirectToAccessDenied("CMS.AbuseReport", permissionType);
        }
    }

    #endregion
}