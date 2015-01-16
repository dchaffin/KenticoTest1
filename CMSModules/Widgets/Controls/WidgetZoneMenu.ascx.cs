using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;

public partial class CMSModules_Widgets_Controls_WidgetZoneMenu : CMSAbstractPortalUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Use UI culture for strings
        string culture = CMSContext.CurrentUser.PreferredUICultureCode;

        // Main menu
        imgNewWebPart.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Add.png");
        lblNewWebPart.Text = ResHelper.GetString("ZoneMenu.IconNewWidget", culture);
        imgNewWebPart.AlternateText = lblNewWebPart.Text;
        pnlNewWebPart.Attributes.Add("onclick", "ContextNewWidget(GetContextMenuParameter('widgetZoneMenu'));");

        // Properties
        imgConfigureZone.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Properties.png");
        lblConfigureZone.Text = ResHelper.GetString("ZoneMenu.IconConfigureWebpartZone", culture);
        imgConfigureZone.AlternateText = lblConfigureZone.Text;
        pnlConfigureZone.Attributes.Add("onclick", "ContextConfigureWebPartZone(GetContextMenuParameter('widgetZoneMenu'));");

        // Delete all widgets
        imgDelete.ImageUrl = GetImageUrl("CMSModules/CMS_PortalEngine/ContextMenu/Delete.png");
        lblDelete.Text = ResHelper.GetString("ZoneMenu.RemoveAllWidgets", culture);
        imgDelete.AlternateText = lblDelete.Text;
        pnlDelete.Attributes.Add("onclick", "ContextRemoveAllWidgets(GetContextMenuParameter('widgetZoneMenu'));");
    }
}