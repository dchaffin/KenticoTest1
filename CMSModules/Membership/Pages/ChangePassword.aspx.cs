using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;

public partial class CMSModules_Membership_Pages_ChangePassword : CMSModalPage
{
    /// <summary>
    /// Page load event
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        SetCulture();

        PageTitle title = CurrentMaster.Title;
        title.TitleImage = GetImageUrl("Objects/CMS_User/password.png");
        title.TitleText = GetString("myaccount.changepassword");

        btnClose.Text = GetString("General.Close");

        ucChangePassword.OnPasswordChange += PasswordChanged;
        ucChangePassword.ForceDifferentPassword = true;
        ucChangePassword.IsLiveSite = false;
    }


    /// <summary>
    /// PreRender event
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        RegisterEscScript();
        RegisterModalPageScripts();
    }

    /// <summary>
    /// Password changed event
    /// </summary>
    public void PasswordChanged(object sender, EventArgs e)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), "RefreshWarningHeader" + ClientID, "if(wopener){wopener.top.HideWarning();}", true);
    }
}