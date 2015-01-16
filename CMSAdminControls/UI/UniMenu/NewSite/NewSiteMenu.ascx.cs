using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;

public partial class CMSAdminControls_UI_UniMenu_NewSite_NewSiteMenu : CMSUserControl
{
    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        const int bigButtonMinimalWidth = 40;
        const int smallButtonMinimalWidth = 65;

        // Big buttons
        MenuItem newItem = new MenuItem();
        newItem.Text = GetString("general.new");
        newItem.Tooltip = GetString("documents.newtooltip");
        newItem.CssClass = "BigButton";
        newItem.OnClientClick = "if (!NewItem()) return;";
        newItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/New.png");
        newItem.ImageAltText = GetString("documents.newtooltip");
        newItem.ImageAlign = ImageAlign.Top;
        newItem.MinimalWidth = bigButtonMinimalWidth;
        buttonsBig.Buttons.Add(newItem);

        MenuItem deleteItem = new MenuItem();
        deleteItem.Text = GetString("general.delete");
        deleteItem.Tooltip = GetString("documents.deletetooltip");
        deleteItem.CssClass = "BigButton";
        deleteItem.OnClientClick = "if (!DeleteItem()) return;";
        deleteItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Delete.png");
        deleteItem.ImageAltText = GetString("documents.deletetooltip");
        deleteItem.ImageAlign = ImageAlign.Top;
        deleteItem.MinimalWidth = bigButtonMinimalWidth;
        buttonsBig.Buttons.Add(deleteItem);

        // Small buttons
        MenuItem upItem = new MenuItem();
        upItem.Text = GetString("general.up");
        upItem.Tooltip = GetString("documents.uptooltip");
        upItem.CssClass = "SmallButton";
        upItem.OnClientClick = "if (!MoveUp()) return;";
        upItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Up.png");
        upItem.ImageAltText = GetString("documents.uptooltip");
        upItem.ImageAlign = ImageAlign.AbsMiddle;
        upItem.MinimalWidth = smallButtonMinimalWidth;
        buttonsSmall.Buttons.Add(upItem);

        MenuItem downItem = new MenuItem();
        downItem.Text = GetString("general.down");
        downItem.Tooltip = GetString("documents.downtooltip");
        downItem.CssClass = "SmallButton";
        downItem.OnClientClick = "if (!MoveDown()) return;";
        downItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/Down.png");
        downItem.ImageAltText = GetString("documents.downtooltip");
        downItem.ImageAlign = ImageAlign.AbsMiddle;
        downItem.MinimalWidth = smallButtonMinimalWidth;
        buttonsSmall.Buttons.Add(downItem);
    }

    #endregion
}