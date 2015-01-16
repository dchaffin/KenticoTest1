using System;

using CMS.UIControls;

public partial class CMSModules_Forums_Content_Properties_Header : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("forum.header.forum");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Forums_Forum/object.png");
        CurrentMaster.FrameResizer.Visible = false;
    }
}