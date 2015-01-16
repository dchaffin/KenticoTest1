using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.SettingsProvider;
using CMS.CMSHelper;

public partial class CMSAdminControls_UI_Development_DevTools : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSPage page = Parent.Page as CMSPage;
        bool developmentMode = (page != null) ? page.CurrentMaster.DevelopmentMode : SettingsKeyProvider.DevelopmentMode;
        if (developmentMode && CMSContext.CurrentUser.UserSiteManagerAdmin)
        {
            // Debug
            lnkDebug.NavigateUrl = "~/CMSModules/System/Debug/System_ViewRequest.aspx?guid=" + DebugHelper.CurrentRequestLogs.RequestGUID;
            lnkDebug.Target = "_blank";

            // Localization
            btnLocalize.HorizontalPosition = HorizontalPositionEnum.Right;
            btnLocalize.OffsetY = -20;
            btnLocalize.OffsetX = 1;

            btnLocalize.MouseButton = MouseButtonEnum.Both;
            btnLocalize.MenuControlPath = "~/CMSAdminControls/UI/Development/Localize.ascx";
            btnLocalize.Image.ImageUrl = GetImageUrl("Objects/CMS_UICulture/list.png");
            btnLocalize.Image.CausesValidation = false;

            imgDebug.ImageUrl = GetImageUrl("CMSModules/CMS_System/debug.png");
            imgDebug.AlternateText = GetString("Administration-System.Debug");
        }
        else
        {
            Visible = false;
        }
    }
}
