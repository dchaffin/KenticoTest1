using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.CMSHelper;
using CMS.PortalEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.ExtendedControls;

using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_PortalEngine_CMSPages_OnSiteEdit_PageNotFound : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (CMSContext.ViewMode != ViewModeEnum.EditLive)
        {
            // Try skip IIS http errors
            Response.TrySkipIisCustomErrors = true;

            // Set page not found state
            Response.StatusCode = 404;

            // Set preferred content culture
            SetLiveCulture();
        }
        else if (SettingsKeyProvider.GetBoolValue(CMSContext.CurrentSiteName + ".CMSAllowOnSiteEditing"))
        {
            CMSAbstractPortalUserControl editToolbar = (CMSAbstractPortalUserControl)Page.LoadUserControl("~/CMSModules/PortalEngine/Controls/OnsiteEdit/EditToolbar.ascx");
            editToolbar.ID = "editToolbar";
            editToolbar.ShortID = "et";
            plcMain.Controls.Add(editToolbar);
        }

        titleElem.TitleText = GetString("404.Header");
        titleElem.TitleImage = GetImageUrl("Others/Messages/error.png");

        lblInfo.Text = String.Format(GetString("404.Info"), URLHelper.CurrentURL + " (" + CMSContext.PreferredCultureCode + ")");
        lblRootDoc.Text = "<a href=\"" + URLHelper.GetApplicationUrl() + "\" target=\"_self\">" + HTMLHelper.HTMLEncode(GetString("onsiteedit.rootredirect")) + "<a>";
    }
}
