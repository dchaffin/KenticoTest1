using System;
using System.Collections;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_Dialogs_General_ViewModeMenu : CMSUserControl
{
    /// <summary>
    /// Returns currently selected tab view mode.
    /// </summary>
    public DialogViewModeEnum SelectedViewMode
    {
        get
        {
            string viewMode = hdnLastSelectedTab.Value.Trim().ToLowerCSafe();

            // Get view mode
            return CMSDialogHelper.GetDialogViewMode(viewMode);
        }
        set
        {
            hdnLastSelectedTab.Value = CMSDialogHelper.GetDialogViewMode(value);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Select view mode as specified
            int index = 0;
            switch (SelectedViewMode)
            {
                case DialogViewModeEnum.TilesView:
                    index = 1;
                    break;

                case DialogViewModeEnum.ThumbnailsView:
                    index = 2;
                    break;
            }

            // List
            menuBtnList.IconUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/modelist.png", IsLiveSite));
            menuBtnList.Tooltip = GetString("dialogs.viewmode.list.desc");
            menuBtnList.OnClickJavascript = "SetLastViewAction('list'); RaiseHiddenPostBack();";
            menuBtnList.Text = GetString("dialogs.viewmode.list");
            menuBtnList.Active = (index == 0);

            // Tiles
            menuBtnTiles.IconUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/modetiles.png", IsLiveSite));
            menuBtnTiles.Tooltip = GetString("dialogs.viewmode.tiles.desc");
            menuBtnTiles.OnClickJavascript = "SetLastViewAction('tiles'); RaiseHiddenPostBack();";
            menuBtnTiles.Text = GetString("dialogs.viewmode.tiles");
            menuBtnTiles.Active = (index == 1);

            // Thumbnails
            menuBtnThumbs.IconUrl = ResolveUrl(GetImageUrl("CMSModules/CMS_Content/Dialogs/modethumbnails.png", IsLiveSite));
            menuBtnThumbs.Tooltip = GetString("dialogs.viewmode.thumbnails.desc");
            menuBtnThumbs.OnClickJavascript = "SetLastViewAction('thumbnails'); RaiseHiddenPostBack();";
            menuBtnThumbs.Text = GetString("dialogs.viewmode.thumbnails");
            menuBtnThumbs.Active = (index == 2);

            // Set last view function
            string setLastView =
@"
function SetLastViewAction(viewMode) {
    var lastView = document.getElementById('" + hdnLastSelectedTab.ClientID + @"');
    if ((lastView!=null) && (viewMode!=null)) {
        lastView.value = viewMode;
    }
}
";

            ltlScript.Text += ScriptHelper.GetScript(setLastView);
        }
    }

}