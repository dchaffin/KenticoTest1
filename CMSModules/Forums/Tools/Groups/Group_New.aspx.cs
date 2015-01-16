using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Forums_Tools_Groups_Group_New : CMSForumsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        groupNewElem.OnSaved += new EventHandler(groupNewElem_OnSaved);
        groupNewElem.IsLiveSite = false;
        InitializeMasterPage();
    }


    protected void groupNewElem_OnSaved(object sender, EventArgs e)
    {
        URLHelper.Redirect("Group_Frameset.aspx?groupId=" + groupNewElem.GroupID);
    }


    /// <summary>
    /// Initializes Master Page.
    /// </summary>
    protected void InitializeMasterPage()
    {
        // Set title and help
        Title = "Forum - New group";
        CurrentMaster.Title.HelpTopicName = "new_forum_group";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Initializes page title control		
        string[,] tabs = new string[2,3];
        tabs[0, 0] = GetString("Group_General.GroupList");
        tabs[0, 1] = "~/CMSModules/Forums/Tools/Groups/Group_List.aspx";
        tabs[0, 2] = "";
        tabs[1, 0] = GetString("Group_General.NewGroup");
        tabs[1, 1] = "";
        tabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = tabs;

        // Set title label
        CurrentMaster.Title.TitleText = GetString("Group_General.NewGroup");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_ForumGroup/new.png");
    }
}