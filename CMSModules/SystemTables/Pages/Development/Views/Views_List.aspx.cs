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

public partial class CMSModules_SystemTables_Pages_Development_Views_Views_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare the new object header element
        string[,] actions = new string[2,7];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        string query = "?new=0";
        query += "&hash=" + QueryHelper.GetHash(query);
        actions[0, 3] = "~/CMSModules/SystemTables/Pages/Development/Views/View_Edit.aspx" + query;
        actions[0, 1] = ResHelper.GetString("sysdev.views.createview");
        actions[0, 5] = GetImageUrl("/Objects/CMS_SystemTable/view.png");

        actions[1, 0] = HeaderActions.TYPE_LINKBUTTON;
        actions[1, 1] = ResHelper.GetString("sysdev.views.refreshviews");
        actions[1, 5] = GetImageUrl("/Objects/CMS_SystemTable/refresh.png");
        actions[1, 6] = "refreshallviews";

        CurrentMaster.HeaderActions.Actions = actions;
        CurrentMaster.HeaderActions.ActionPerformed += new CommandEventHandler(HeaderActions_ActionPerformed);

        lstSQL.Views = true;
        lstSQL.EditPage = "ViewEdit_Frameset.aspx";
    }


    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "refreshallviews")
        {
            lstSQL.RefresViews();
            ShowConfirmation(GetString("systbl.views.allviewsrefreshed"));
        }
    }
}