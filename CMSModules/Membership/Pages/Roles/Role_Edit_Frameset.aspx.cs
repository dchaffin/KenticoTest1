using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Membership_Pages_Roles_Role_Edit_Frameset : CMSRolesPage
{
    protected string parameters = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        parameters += "?roleid=" + QueryHelper.GetString("roleid", String.Empty);

        if (SiteID != 0)
        {
            parameters += "&siteid=" + SiteID;
        }
        else if (SelectedSiteID != 0)
        {
            parameters += "&selectedsiteid=" + SelectedSiteID;
        }
    }
}