using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Modules_Pages_Development_Module_List : SiteManagerPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.Title.TitleText = GetString("Administration-Module_List.Title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/CMS_Module/object.png");
        CurrentMaster.Title.HelpTopicName = "modules_list";
        CurrentMaster.Title.HelpName = "helpTopic";

        // New item link
        string[,] actions = new string[1,6];
        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = GetString("Administration-Module_List.NewModule");
        actions[0, 2] = null;
        actions[0, 3] = ResolveUrl("Module_New.aspx");
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("Objects/CMS_Module/add.png");
        CurrentMaster.HeaderActions.Actions = actions;

        UniGridModules.OnAction += new OnActionEventHandler(UniGridModules_OnAction);
        UniGridModules.ZeroRowsText = GetString("general.nodatafound");
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
            URLHelper.Redirect("Module_Edit_Frameset.aspx?moduleID=" + actionArgument.ToString());
        }
        else if (actionName == "delete")
        {
            ResourceInfoProvider.DeleteResourceInfo(Convert.ToInt32(actionArgument));
        }
    }
}