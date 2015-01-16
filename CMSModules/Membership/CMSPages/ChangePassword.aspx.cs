using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

public partial class CMSModules_Membership_CMSPages_ChangePassword : CMSLiveModalPage
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

        ucChangePassword.ForceDifferentPassword = true;
        ucChangePassword.IsLiveSite = true;
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
}