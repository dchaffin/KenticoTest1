using System;

using CMS.CMSHelper;
using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_MyDesk_MyProfile_MyProfile_ChangePassword : CMSMyProfilePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if ((CMSContext.CurrentUser == null) || (!CMSContext.CurrentUser.IsAuthorizedPerUIElement("CMS.MyDesk", "MyProfile.ChangePassword")))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.MyDesk", "MyProfile.ChangePassword");
        }

        ucChangePassword.OnPasswordChange += PasswordChanged;
    }


    public void PasswordChanged(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshWarningHeader" + ClientID, "window.top.HideWarning();", true);
    }
}