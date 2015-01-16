using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Roles_Role_Edit : CMSGroupRolesPage
{
    protected string parameters = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        int roleId = QueryHelper.GetInteger("roleid", 0);
        int groupId = QueryHelper.GetInteger("groupid", 0);

        parameters = "?roleid=" + roleId + "&groupid=" + groupId;
    }
}