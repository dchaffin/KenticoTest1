using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.GlobalHelper;

public partial class CMSAdminControls_UI_Macros_MacroDesignerMenu : CMSContextMenuControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        itemMoveUp.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Up.png");
        itemMoveUp.Text = ResHelper.GetString("General.MoveUp");
        itemMoveUp.Attributes.Add("onclick", "MoveDesignerGroup(GetContextMenuParameter('" + ContextMenu.MenuID + "').split('@')[0], '" + ResHelper.GetString("macrodesigner.cannotmoveup") + "');");

        itemMoveDown.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/UniGrid/Actions/Down.png");
        itemMoveDown.Text = ResHelper.GetString("General.MoveDown");
        itemMoveDown.Attributes.Add("onclick", "MoveDesignerGroup(GetContextMenuParameter('" + ContextMenu.MenuID + "').split('@')[1], '" + ResHelper.GetString("macrodesigner.cannotmovedown") + "');");

        itemMoveToParent.ImageUrl = UIHelper.GetImageUrl(Page, "Design/Controls/MacroDesigner/MoveToParent.png");
        itemMoveToParent.Text = ResHelper.GetString("macrodesigner.movetoparent");
        itemMoveToParent.Attributes.Add("onclick", "MoveDesignerGroup(GetContextMenuParameter('" + ContextMenu.MenuID + "').split('@')[2], '" + ResHelper.GetString("macrodesigner.cannotmovetoparent") + "');");
    }
}