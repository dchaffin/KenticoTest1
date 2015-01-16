using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;

[Security(Resource = "CMS.Categories", Permission = "Read")]
public partial class CMSModules_Membership_Pages_Users_User_Edit_Categories : CMSUsersPage
{
    protected int userId = 0;


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);
        CurrentMaster.PanelBody.CssClass = "";

        userId = QueryHelper.GetInteger("userid", 0);

        CategoriesElem.UserID = userId;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (userId > 0)
        {
            // Check that only global administrator can edit global administrator's accouns
            UserInfo ui = UserInfoProvider.GetUserInfo(userId);
            CheckUserAvaibleOnSite(ui);
            EditedObject = ui;
        }

        Panel pnlContent = ControlsHelper.GetChildControl(Page, typeof(Panel), "pnlContent") as Panel;
        if (pnlContent != null)
        {
            pnlContent.CssClass = "";
        }
    }
}