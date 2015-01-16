using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Forums_Tools_Groups_Group_General : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int groupID = QueryHelper.GetInteger("groupid", 0);
        ForumContext.CheckSite(groupID, 0, 0);

        groupEdit.GroupID = groupID;
        groupEdit.IsLiveSite = false;
    }
}