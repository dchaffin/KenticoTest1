using System;

using CMS.UIControls;

public partial class CMSModules_MessageBoards_Content_Properties_Header : CMSContentMessageBoardsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("board.header.messageboards");
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_MessageBoards/module.png");
    }
}