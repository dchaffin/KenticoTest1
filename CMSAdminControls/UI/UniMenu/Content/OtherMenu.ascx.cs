using System;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.GlobalHelper;

using MenuItem = CMS.UIControls.UniMenuConfig.Item;

public partial class CMSAdminControls_UI_UniMenu_Content_OtherMenu : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Search button
        MenuItem searchItem = new MenuItem();
        searchItem.Text = GetString("contentmenu.search");
        searchItem.Tooltip = GetString("contentmenu.searchtooltip");
        searchItem.CssClass = "BigButton";
        searchItem.OnClientClick = "OpenSearch()";
        searchItem.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/search.png");
        searchItem.ImageAltText = GetString("contentmenu.searchtooltip");
        searchItem.ImageAlign = ImageAlign.Top;
        searchItem.MinimalWidth = 48;
        buttons.Buttons.Add(searchItem);

        // Maximize button - maximize button is not supported
        //MenuItem fullScreen = new MenuItem();
        //fullScreen.Caption = GetString("contentmenu.maximize");
        //fullScreen.Tooltip = GetString("contentmenu.maximizetooltip");
        //fullScreen.CssClass = "ModeButton";
        //fullScreen.OnClientClick = "FullScreen()";
        //fullScreen.ImagePath = GetImageUrl("CMSModules/CMS_Content/Menu/FullScreen.png");
        //buttons.Buttons.Add(fullScreen);
    }
}