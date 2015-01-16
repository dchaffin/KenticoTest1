using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_SystemTables_Pages_Development_SystemTable_Edit_Query_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("classid", 0);

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("DocumentType_Edit_Query_List.btnNew");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("Edit_Query_Edit.aspx?classid=" + classId.ToString());
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Query/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        // Set the query editor control
        classEditQuery.ClassID = classId;
        classEditQuery.EditPageUrl = "Edit_Query_Edit.aspx";
    }
}