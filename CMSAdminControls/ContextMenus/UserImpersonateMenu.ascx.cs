using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSAdminControls_ContextMenus_UserImpersonateMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        pnlImpersonateUser.Visible = true;
        imgImpersonate.ImageUrl = UIHelper.GetImageUrl(Page, "Objects/CMS_User/headerImpersonate.png");
        imgImpersonate.AlternateText = ResHelper.GetString("users.impersonate");
        pnlImpersonateUser.Attributes.Add("onclick", "userImpersonateShowDialog()");

        string originalUserName = String.Empty;
        if (RequestHelper.IsFormsAuthentication())
        {
            // Get original user name
            originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("OriginalUserName"), "");
        }
        else
        {
            // Get impersonated user name
            originalUserName = ValidationHelper.GetString(SessionHelper.GetValue("ImpersonateUserName"), "");
        }

        // Show only if user is impersonated
        if (!String.IsNullOrEmpty(originalUserName))
        {
            pnlReverseImpersonate.Visible = true;
            imgReverseImpersonate.ImageUrl = UIHelper.GetImageUrl(Page, "Objects/CMS_User/headerCancelImpersonation.png");
            imgImpersonate.AlternateText = ResHelper.GetString("users.cancelimpersonation");
            pnlReverseImpersonate.Attributes.Add("onclick", "CancelImpersonation()");
        }
    }
}