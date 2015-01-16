using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSAdminControls_UI_UniGrid_Controls_UpDownMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string menuId = ContextMenu.MenuID;

        iTop.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Top.png");
        iTop.Text = GetString("General.Top");
        iTop.OnClientClick = "MoveNode('top',GetContextMenuParameter('" + menuId + "'))";

        iUp.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Up.png");
        iUp.Text = GetString("General.Up");
        iUp.OnClientClick = "MoveNode('up',GetContextMenuParameter('" + menuId + "'))";

        iDown.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Down.png");
        iDown.Text = GetString("General.Down");
        iDown.OnClientClick = "MoveNode('down',GetContextMenuParameter('" + menuId + "'))";

        iBottom.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Bottom.png");
        iBottom.Text = GetString("General.Bottom");
        iBottom.OnClientClick = "MoveNode('bottom',GetContextMenuParameter('" + menuId + "'))";
    }
}
