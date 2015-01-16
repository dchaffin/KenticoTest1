using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_PortalEngine_UI_PageLayouts_PageLayout_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Administration-PageLayout_List.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Layout/object.png");
        CurrentMaster.Title.HelpTopicName = "page_layouts_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Administration-PageLayout_List.NewLayout");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("PageLayout_New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Layout/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        gridLayouts.OnAction += new OnActionEventHandler(UniGridModules_OnAction);
        gridLayouts.ZeroRowsText = GetString("general.nodatafound");
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridModules_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("PageLayout_Frameset.aspx?layoutId=" + actionArgument.ToString());
        }
        else if (actionName == "delete")
        {
            LayoutInfoProvider.DeleteLayoutInfo(Convert.ToInt32(actionArgument));
        }
    }
}