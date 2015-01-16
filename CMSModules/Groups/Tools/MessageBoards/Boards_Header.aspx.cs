using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_MessageBoards_Boards_Header : CMSGroupMessageBoardsPage
{
    private int mGroupId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Intialize the control
        SetupControl();
    }


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        mGroupId = QueryHelper.GetInteger("groupid", 0);

        // initializes breadcrumbs 		
        string[,] pageTitleTabs = new string[1,3];
        pageTitleTabs[0, 0] = GetString("board.header.messageboards");
        pageTitleTabs[0, 1] = "";
        pageTitleTabs[0, 2] = "";

        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        CurrentMaster.Title.HelpTopicName = "CMS_MessageBoards_Messages";
        CurrentMaster.Title.HelpName = "helpTopic";

        InitalizeMenu();
    }


    /// <summary>
    /// Initialize the tab control on the master page.
    /// </summary>
    private void InitalizeMenu()
    {
        // Collect tabs data
        string[,] tabs = new string[2,4];
        tabs[0, 0] = GetString("board.header.messages");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'CMS_MessageBoards_Messages');";
        tabs[0, 2] = "Messages/Message_List.aspx" + ((mGroupId > 0) ? "?groupid=" + mGroupId : "");

        tabs[1, 0] = GetString("board.header.boards");
        tabs[1, 1] = "SetHelpTopic('helpTopic', 'CMS_MessageBoards_Boards');";
        tabs[1, 2] = "Boards/Board_List.aspx" + ((mGroupId > 0) ? "?groupid=" + mGroupId : "");

        // Set the target iFrame
        CurrentMaster.Tabs.UrlTarget = "boardsContent";

        // Assign tabs data
        CurrentMaster.Tabs.Tabs = tabs;
    }

    #endregion
}