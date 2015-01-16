using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSModules_Widgets_Controls_WidgetMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = CMSContext.CurrentUser.PreferredUICultureCode;

        // Properties
        imgProperties.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Properties.png");
        lblProperties.Text = ResHelper.GetString("WebPartMenu.IconProperties", culture);
        pnlProperties.Attributes.Add("onclick", "ContextConfigureWidget(GetContextMenuParameter('widgetMenu'));");

        // Up
        imgUp.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Up.png");
        lblUp.Text = ResHelper.GetString("WebPartMenu.IconUp", culture);
        pnlUp.Attributes.Add("onclick", "ContextMoveWidgetUp(GetContextMenuParameter('widgetMenu'));");

        // Down
        imgDown.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Down.png");
        lblDown.Text = ResHelper.GetString("WebPartMenu.IconDown", culture);
        pnlDown.Attributes.Add("onclick", "ContextMoveWidgetDown(GetContextMenuParameter('widgetMenu'));");

        // Clone
        imgClone.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Clonewidget.png");
        lblClone.Text = ResHelper.GetString("widgets.widgetMenu.clone", culture);
        pnlClone.Attributes.Add("onclick", "ContextCloneWidget(GetContextMenuParameter('widgetMenu'));");

        // Delete
        imgDelete.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Delete.png");
        lblDelete.Text = ResHelper.GetString("general.remove", culture);
        pnlDelete.Attributes.Add("onclick", "ContextRemoveWidget(GetContextMenuParameter('widgetMenu'));");

        // Top
        lblTop.Text = ResHelper.GetString("UpMenu.IconTop", culture);
        pnlTop.Attributes.Add("onclick", "ContextMoveWidgetTop(GetContextMenuParameter('widgetMenu'));");

        // Bottom
        lblBottom.Text = ResHelper.GetString("DownMenu.IconBottom", culture);
        pnlBottom.Attributes.Add("onclick", "ContextMoveWidgetBottom(GetContextMenuParameter('widgetMenu'));");
    }
}