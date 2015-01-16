using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.UIControls;

[Tabs("CMS.MessageBoards", "", "boardsContent")]
public partial class CMSModules_MessageBoards_Tools_Boards_Header : CMSMessageBoardPage
{
    #region "Variables"

    private int mGroupId = 0;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        mGroupId = QueryHelper.GetInteger("groupid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Intialize the control
        SetupControl();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControl()
    {
        // Set the page title when existing category is being edited
        CurrentMaster.Title.TitleText = GetString("board.header.messageboards");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Board_Board/object.png");
        CurrentMaster.Title.HelpName = "helpTopic";
        CurrentMaster.Title.HelpTopicName = "CMS_MessageBoards_Messages";
    }

    #endregion
}