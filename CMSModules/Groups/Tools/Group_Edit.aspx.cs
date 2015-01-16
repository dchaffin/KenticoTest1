using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Group_Edit : CMSGroupPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get groupid
        int groupId = QueryHelper.GetInteger("groupid", 0);
        CheckGroupPermissions(groupId, CMSAdminControl.PERMISSION_READ);
    }
}