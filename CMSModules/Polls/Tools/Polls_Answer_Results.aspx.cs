using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(FormObjectType.BIZFORM, "formId")]
public partial class CMSModules_Polls_Tools_Polls_Answer_Results : CMSModalPage
{
    #region "Variables"

    private CMSAdminControl bizFormData = null;

    #endregion


    #region "Methods"

    protected void Page_Init(object sender, EventArgs e)
    {
        // Check license
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Polls);
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Polls", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Polls");
        }

        // Check site availability
        if (!ResourceSiteInfoProvider.IsResourceOnSite("CMS.Form", CMSContext.CurrentSiteName))
        {
            RedirectToResourceNotAvailableOnSite("CMS.Form");
        }

        CurrentUserInfo user = CMSContext.CurrentUser;

        // Check permissions for CMS Desk -> Tools -> Polls
        if (!user.IsAuthorizedPerUIElement("CMS.Tools", "Tools.Content") || !user.IsAuthorizedPerUIElement("CMS.Tools", "Polls"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "Polls");
        }

        // Check permissions for site polls
        if (!user.IsAuthorizedPerResource("CMS.Polls", CMSAdminControl.PERMISSION_READ))
        {
            RedirectToCMSDeskAccessDenied("CMS.Polls", "Read");
        }

        // Check permissions for forms
        if (!user.IsAuthorizedPerResource("CMS.Form", "ReadData"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Form", "ReadData");
        }
        
        // Load BizForm selector if BizForms module is available
        if (ModuleEntry.IsModuleLoaded(ModuleEntry.BIZFORM) && ResourceSiteInfoProvider.IsResourceOnSite(ModuleEntry.BIZFORM, CMSContext.CurrentSiteName))
        {
            bizFormData = this.LoadUserControl("~/CMSModules/BizForms/Controls/BizFormEditData.ascx") as CMSAdminControl;
            bizFormData.ShortID = "bizFormData";
            bizFormData.SetValue("ShowNewRecordButton", false);
            plcBizForm.Controls.Add(bizFormData);
            bizFormData.Visible = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("polls_answer_records.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Polls/module.png");
        CurrentMaster.Title.ShowCloseButton = true;
        CurrentMaster.Title.ShowFullScreenButton = true;
    }

    #endregion
}