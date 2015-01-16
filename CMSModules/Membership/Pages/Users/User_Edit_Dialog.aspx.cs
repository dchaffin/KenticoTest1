using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Dialog : CMSUsersPage
{
    protected string viewpage = "about:blank";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Register scripts
        ScriptHelper.RegisterWOpenerScript(Page);

        // Get user ID and user name
        int userId = QueryHelper.GetInteger("userid", 0);
        string currentUser = Functions.GetFormattedUserName(UserInfoProvider.GetUserNameById(userId));

        // Set title
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_User/object.png");
        CurrentMaster.Title.TitleText = currentUser;

        // Open customer edit frameset in popup mode
        viewpage = ResolveUrl("~/CMSModules/Membership/Pages/Users/User_Edit_Frameset.aspx?popup=1&userid=" + userId);

        // Fill space
        CurrentMaster.PanelContent.Style.Add("width", "100%");
        CurrentMaster.PanelContent.Style.Add("height", "100%");

        // Localize close button
        btnClose.Text = GetString("general.close");
    }
}
